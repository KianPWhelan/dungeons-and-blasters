using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bean : AttackComponent
{
    [Networked]
    [HideInInspector]
    public TickTimer networkedLifeTimer { get; set; }
    private TickTimer predictedLifeTimer;
    private TickTimer lifeTimer
    {
        get => Object.IsPredictedSpawn ? predictedLifeTimer : networkedLifeTimer;
        set { if (Object.IsPredictedSpawn) predictedLifeTimer = value; else networkedLifeTimer = value; }
    }

    public BeanSettings settings;

    [System.Serializable]
    public class BeanSettings
    {
        public Attack attack;

        public List<Attack> subAttacks = new List<Attack>();
        public bool subAttacksOnEnd;
        public bool subAttacksOnHit;
        public bool subAttacksOnTip;
        public bool subAttacksAtSurfaceNormal;

        public bool applyEffectsOnEnd;
        public bool applyEffectsOnTip;

        public Vector3 offset;
        public Vector3 rotationOffset;
        public bool worldSpaceRotation;
        public bool reevaluateDestinationAfterOffset;

        public bool canReflect;
        public int numReflections;
        public bool canReflectOffValidTarget;
        public bool reflectOffValidTargetOnly;
        public bool applyEffectsOnReflect;
        public bool subAttacksOnReflect;
        public bool autoTargetOnReflect;

        public bool followOwner;
        public float length;
        public float spread;
        public bool spreadAffectsReflection;
        public bool spreadOnlyAffectsReflection;
        public LayerMask hitMask;
        public bool infinitePierce;
        public int numPierces;
        public bool canMultiHitTarget;
        public float multiHitCooldown;
        public bool ignoreObstacles;
        public bool directControlByOwner;
        
        public float lifetime;
    }

    private NetworkObject owner;
    private Weapon ownerWeapon;
    private Vector3 hitPoint;
    private Vector3 hitNormal;
    private List<GameObject> hitList = new List<GameObject>();
    private int weaponIndex;
    private int attackIndex;

    private int numHits;
    private int numReflects;

    private Vector3 lastForward;

    private Vector3 lastReflection = Vector3.negativeInfinity;

    private LineRenderer lineRenderer;

    private List<Vector3> linePoints;

    private Dictionary<int, Vector3> reflectionAccuracyOffsets = new Dictionary<int, Vector3>();

    private bool isAlt;

    public override void InitNetworkState(string validTag, float damageMod, object destination, NetworkObject owner = null, int weaponIndex = 0, int attackIndex = 0, bool isAlt = false)
    {
        base.InitNetworkState(validTag, damageMod, destination, owner);

        this.validTag = validTag;
        this.damageMod = damageMod;

        if (destination != null)
        {
            Debug.Log("Destination recieved: " + destination);
            this.destination = (Vector3)destination;
            useDestination = true;
            Debug.Log(useDestination);
        }

        this.owner = owner;
        this.weaponIndex = weaponIndex;
        this.attackIndex = attackIndex;
        this.isAlt = isAlt;

        if(settings.directControlByOwner)
        {
            Object.AssignInputAuthority(owner.InputAuthority);
            ownerWeapon = owner.GetComponent<WeaponHolder>().GetWeapons()[weaponIndex];
        }
    }

    public override void Spawned()
    {
        Debug.Log("Spawned " + useDestination);
        // Create lifetimer
        lifeTimer = TickTimer.CreateFromSeconds(Runner, settings.lifetime);

        // Perform initial direction rotation
        if (useDestination)
        {
            Debug.Log("Using destination");
            transform.rotation = Quaternion.LookRotation((destination - transform.position).normalized);
        }

        // Calculate spread
        Vector3 accuracyOffset = Random.insideUnitCircle * settings.spread;

        if(settings.spreadOnlyAffectsReflection)
        {
            accuracyOffset = Vector3.zero;
        }

        // Adjust position to offset
        transform.position += (transform.forward * settings.offset.z) + (transform.right * settings.offset.x) + (transform.up * settings.offset.y);

        // Adjust rotation to offset
        if (settings.worldSpaceRotation)
        {
            transform.rotation = Quaternion.Euler(settings.rotationOffset + accuracyOffset);
        }

        else
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + settings.rotationOffset + accuracyOffset);
        }

        // Reevaluate rotation towards directions
        if (settings.reevaluateDestinationAfterOffset)
        {
            Debug.Log("reevaluating direction");
            transform.rotation = Quaternion.LookRotation((destination - transform.position).normalized);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + accuracyOffset);
        }

        TryGetComponent(out lineRenderer);
    }

    public override void FixedUpdateNetwork()
    {
        if(settings.directControlByOwner)
        {
            if(GetInput(out PlayerInput input))
            {
                if (!isAlt && !input.IsDown(PlayerInput.BUTTON_FIRE))
                {
                    Debug.Log("Fire button up");
                    ownerWeapon.attacks[attackIndex].isInUse = false;
                    ApplyEffectsOnEnd();
                    SubAttacksOnEnd();
                    DestroyBean();
                    return;
                }

                if (isAlt && !input.IsDown(PlayerInput.BUTTON_FIRE_ALT))
                {
                    ownerWeapon.alternateAttackWeapon.attacks[attackIndex].isInUse = false;
                    ApplyEffectsOnEnd();
                    SubAttacksOnEnd();
                    DestroyBean();
                    return;
                }
            }

            if(!isAlt && ownerWeapon.overheat.onCooldown)
            {
                ownerWeapon.attacks[attackIndex].isInUse = false;
                ApplyEffectsOnEnd();
                SubAttacksOnEnd();
                DestroyBean();
                return;
            }

            else if (isAlt && ownerWeapon.alternateAttackWeapon.overheat.onCooldown)
            {
                ownerWeapon.alternateAttackWeapon.attacks[attackIndex].isInUse = false;
                ApplyEffectsOnEnd();
                SubAttacksOnEnd();
                DestroyBean();
                return;
            }
        }

        if (!lifeTimer.Expired(Runner))
        {
            UpdateBean();
        }

        else
        {
            // TODO: Ending effects/subattacks
            hitPoint = transform.position + transform.forward * settings.length;
            ApplyEffectsOnEnd();
            SubAttacksOnEnd();
            DestroyBean();
        }
    }

    private void UpdateBean()
    {
        linePoints = new List<Vector3>();
        numHits = 0;
        numReflects = 0;

        if (owner != null && settings.followOwner)
        {
            transform.position = owner.transform.position;
            transform.rotation = owner.transform.rotation;
        }

        lastForward = transform.position + transform.forward * settings.length;
        lastReflection = transform.forward;

        linePoints.Add(transform.position);

        List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
        //Debug.Log("Position" + transform.position + " Rotation" + transform.rotation.eulerAngles);
        Runner.LagCompensation.RaycastAll(transform.position, transform.forward, settings.length, Object.InputAuthority, hits, settings.hitMask.value, options: HitOptions.IncludePhysX);
        var point = ProcessHits(hits);

        if (point.x == Mathf.NegativeInfinity)
        {
            hitPoint = lastForward;
        }

        else
        {
            hitPoint = point;
        }

        linePoints.Add(hitPoint);

        //foreach(Vector3 v in linePoints)
        //{
        //    Debug.Log(v);
        //}

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = linePoints.Count;
            lineRenderer.SetPositions(linePoints.ToArray());
        }
    }

    private Vector3 ProcessHits(List<LagCompensatedHit> hits)
    {
        var hitArray = hits.ToArray();
        System.Array.Sort(hitArray, delegate (LagCompensatedHit hit1, LagCompensatedHit hit2) { return hit1.Distance.CompareTo(hit2.Distance); });

        foreach (LagCompensatedHit hit in hitArray)
        {
            //Debug.Log((hit.Hitbox != null) + " " + (hit.GameObject.tag == validTag) + " " + (hit.Hitbox.Root.Object.InputAuthority != Object.InputAuthority) + " " + (!hitList.Contains(hit.GameObject)));
            // TODO: Stop projectile from hitting caster if caster has same target tag
            if (hit.Hitbox != null && hit.Hitbox.Root.tag == validTag/* && hit.Hitbox.Root.Object.InputAuthority != Object.InputAuthority */&& !hitList.Contains(hit.Hitbox.Root.gameObject))
            {
                //Debug.Log("Hit valid target");
                settings.attack.ApplyEffects(hit.Hitbox.Root.gameObject, validTag, damageMod: damageMod);
                numHits++;
                hitList.Add(hit.Hitbox.Root.gameObject);
                hitNormal = hit.Normal;
                hitPoint = hit.Point;
                SubAttacksOnHit();

                if (settings.canMultiHitTarget)
                {
                    StartCoroutine(RemoveFromHitListDelay(hit.Hitbox.Root.gameObject));
                }

                if (numHits > settings.numPierces && !settings.infinitePierce)
                {
                    // TODO: Ending effects/subattacks
                    //DestroyBean();
                    if(settings.canReflect && settings.canReflectOffValidTarget && numReflects < settings.numReflections)
                    {
                        return PerformReflection(hit);
                    }

                    return hit.Point;
                }
            }

            else if(hit.Hitbox != null && hit.Hitbox.Root.tag == validTag)
            {
                numHits++;

                if (numHits > settings.numPierces && !settings.infinitePierce)
                {
                    // TODO: Ending effects/subattacks
                    //DestroyBean();

                    if (settings.canReflect && settings.canReflectOffValidTarget && numReflects < settings.numReflections)
                    {
                        return PerformReflection(hit);
                    }

                    return hit.Point;
                }
            }

            else if (hit.Collider != null && !settings.ignoreObstacles && hit.Collider.tag != "Player")
            {
                // TODO: Ending effects/subattacks
                //Debug.Log("Hit Collider");
                hitNormal = hit.Normal;
                hitPoint = hit.Point;
                SubAttacksOnTip();
                ApplyEffectsOnTip();
                //DestroyBean();

                if(settings.canReflect && !settings.reflectOffValidTargetOnly && numReflects < settings.numReflections)
                {
                    return PerformReflection(hit);
                }

                return hit.Point;
            }
        }

        return Vector3.negativeInfinity;
    }

    private Vector3 PerformReflection(LagCompensatedHit hit)
    {
        //Debug.Log("Reflecting " + numReflects);
        ApplyEffectsOnReflect();
        SubAttacksOnReflect();
        numReflects++;
        linePoints.Add(hit.Point);
        Vector3 reflection;
        Vector3 accuracyOffset;

        if (!reflectionAccuracyOffsets.ContainsKey(numReflects))
        {
            accuracyOffset = Random.insideUnitCircle * settings.spread;
            reflectionAccuracyOffsets.Add(numReflects, accuracyOffset);
        }
        
        else
        {
            accuracyOffset = reflectionAccuracyOffsets[numReflects];
        }

        if(!settings.spreadAffectsReflection)
        {
            accuracyOffset = Vector3.zero;
        }

        if (lastReflection.x == Mathf.NegativeInfinity)
        {
            reflection = Vector3.Reflect(transform.forward, hit.Normal).normalized;
            reflection += accuracyOffset;
            lastReflection = reflection;
            //Debug.Log(lastReflection);
        }

        else
        {
            reflection = Vector3.Reflect(lastReflection, hit.Normal).normalized;
            reflection += accuracyOffset;
            lastReflection = reflection;
            //Debug.Log(lastReflection);
        }
        
        lastForward = hit.Point + reflection * settings.length;
        List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
        //Debug.Log("Position" + transform.position + " Rotation" + transform.rotation.eulerAngles);
        Runner.LagCompensation.RaycastAll(hit.Point + 0.2f * reflection, reflection, settings.length, Object.InputAuthority, hits, settings.hitMask.value, options: HitOptions.IncludePhysX);
        var point = ProcessHits(hits);
        return point;
    }

    private void DestroyBean()
    {
        Runner.Despawn(Object);
    }

    private void ApplyEffectsOnEnd()
    {
        if (settings.applyEffectsOnEnd)
        {
            settings.attack.ApplyEffects(null, validTag, hitPoint, transform.rotation, damageMod);
        }
    }

    private void ApplyEffectsOnTip()
    {
        if (settings.applyEffectsOnTip)
        {
            settings.attack.ApplyEffects(null, validTag, hitPoint, transform.rotation, damageMod);
        }
    }

    private void ApplyEffectsOnReflect()
    {
        if (settings.applyEffectsOnReflect)
        {
            settings.attack.ApplyEffects(null, validTag, hitPoint, transform.rotation, damageMod);
        }
    }

    private void SubAttacksOnHit()
    {
        if (settings.subAttacksOnHit)
        {
            SpawnSubAttacks();
        }
    }

    private void SubAttacksOnEnd()
    {
        if (settings.subAttacksOnEnd)
        {
            SpawnSubAttacks();
        }
    }

    private void SubAttacksOnTip()
    {
        if (settings.subAttacksOnTip)
        {
            SpawnSubAttacks();
        }
    }

    private void SubAttacksOnReflect()
    {
        if (settings.subAttacksOnReflect)
        {
            SpawnSubAttacks();
        }
    }

    private void SpawnSubAttacks()
    {
        foreach (Attack attack in settings.subAttacks)
        {
            if (settings.subAttacksAtSurfaceNormal)
            {
                Debug.Log("Spawning sub attacks");
                attack.PerformAttack(hitPoint, transform.rotation, damageMod, 0, hitPoint + hitNormal, validTag, 0f);
            }

            else
            {
                attack.PerformAttack(hitPoint, transform.rotation, damageMod, 0, targetTag: validTag, delay: 0f);
            }
        }
    }

    private IEnumerator RemoveFromHitListDelay(GameObject hitObj)
    {
        yield return new WaitForSeconds(settings.multiHitCooldown);
        hitList.Remove(hitObj);
    }
}
