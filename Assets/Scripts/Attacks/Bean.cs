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

        public bool followOwner;
        public float length;
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
    private Vector3 hitPoint;
    private Vector3 hitNormal;
    private List<GameObject> hitList = new List<GameObject>();

    private int numHits;

    private LineRenderer lineRenderer;

    public override void InitNetworkState(string validTag, float damageMod, object destination, NetworkObject owner = null)
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

        // Adjust position to offset
        transform.position += (transform.forward * settings.offset.z) + (transform.right * settings.offset.x) + (transform.up * settings.offset.y);

        TryGetComponent(out lineRenderer);
    }

    public override void FixedUpdateNetwork()
    {
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
        List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
        //Debug.Log("Position" + transform.position + " Rotation" + transform.rotation.eulerAngles);
        Runner.LagCompensation.RaycastAll(transform.position, transform.forward, settings.length, Object.InputAuthority, hits, settings.hitMask.value, options: HitOptions.IncludePhysX);
        var point = ProcessHits(hits);

        if(owner != null && settings.followOwner)
        {
            transform.position = owner.transform.position;
            transform.rotation = owner.transform.rotation;
        }

        if (point.x == Mathf.NegativeInfinity)
        {
            hitPoint = transform.position + transform.forward * settings.length;
        }

        else
        {
            hitPoint = point;
        }

        //Debug.Log(hitPoint);

        if (lineRenderer != null)
        {
            lineRenderer.SetPositions(new Vector3[2] { transform.position, hitPoint });
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
                    return hit.Point;
                }
            }

            else if (hit.Collider != null && !settings.ignoreObstacles)
            {
                // TODO: Ending effects/subattacks
                //Debug.Log("Hit Collider");
                hitNormal = hit.Normal;
                hitPoint = hit.Point;
                SubAttacksOnTip();
                ApplyEffectsOnTip();
                //DestroyBean();
                return hit.Point;
            }
        }

        return Vector3.negativeInfinity;
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
