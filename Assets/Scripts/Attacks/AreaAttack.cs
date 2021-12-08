using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class AreaAttack : AttackComponent
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

    [Networked]
    public Vector3 networkedVelocity { get; set; }
    private Vector3 predictedVelocity;
    public Vector3 velocity
    {
        get => Object.IsPredictedSpawn ? predictedVelocity : networkedVelocity;
        set { if (Object.IsPredictedSpawn) predictedVelocity = value; else networkedVelocity = value; }
    }

    private List<GameObject> hitList = new List<GameObject>();

    private int numHits;

    public AreaAttackSettings settings;

    [System.Serializable]
    public class AreaAttackSettings
    {
        public Attack attack;

        public List<Attack> subAttacks = new List<Attack>();
        public bool subAttacksOnEnd;
        public bool subAttacksOnHit;
        public bool subAttacksAtSurfaceNormal;

        public Vector3 offset;

        public bool followOwner;
        public float radius;
        public LayerMask hitMask;
        public bool infinitePierce;
        public int numPierces;
        public bool canMultiHitTarget;
        public float multiHitCooldown;
        public bool ignoreObstacles;
        public bool applyEffectsOnEnd;
        public float lifetime;
        public float gravityStrength;
    }

    private Vector3 hitPoint;
    private Vector3 hitNormal;
    private NetworkObject owner;

    public override void InitNetworkState(string validTag, float damageMod, object destination, NetworkObject owner = null, int weaponIndex = 0, int attackIndex = 0, bool isAlt = false)
    {
        base.InitNetworkState(validTag, damageMod, destination);
        //Object = GetComponent<NetworkObject>();
        //Debug.LogWarning("Object:");
        //Debug.LogWarning(Object);
        //lifeTimer = TickTimer.CreateFromSeconds(Runner, settings.lifetime);
        //velocity = settings.speed * transform.forward;
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
        if (!Object.HasStateAuthority)
        {
            return;
        }

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
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }

        if (!lifeTimer.Expired(Runner))
        {
            UpdateAttack();
        }

        else
        {
            // TODO: Ending effects/subattacks
            hitPoint = transform.position;
            ApplyEffectsOnEnd();
            SubAttacksOnEnd();
            DestroyAttack();
        }
    }

    private void UpdateAttack()
    {
        List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
        Runner.LagCompensation.OverlapSphere(transform.position, settings.radius, Object.InputAuthority, hits, settings.hitMask.value, options: HitOptions.IncludePhysX);
        ProcessHits(hits);

        if (settings.gravityStrength > 0)
        {
            velocity = new Vector3(velocity.x, velocity.y - (9.8f * settings.gravityStrength * Runner.DeltaTime), velocity.z);
            transform.LookAt(transform.position + velocity * Runner.DeltaTime);

            try
            {
                transform.position += velocity * Runner.DeltaTime;
            }

            catch
            { }
        }

        if (settings.followOwner)
        {
            transform.position = owner.transform.position;
            transform.rotation = owner.transform.rotation;

            // Adjust position to offset
            transform.position += (transform.forward * settings.offset.z) + (transform.right * settings.offset.x) + (transform.up * settings.offset.y);
        }
    }

    private void ProcessHits(List<LagCompensatedHit> hits)
    {
        var hitArray = hits.ToArray();
        System.Array.Sort(hitArray, delegate (LagCompensatedHit hit1, LagCompensatedHit hit2) { return hit1.Distance.CompareTo(hit2.Distance); });

        foreach (LagCompensatedHit hit in hitArray)
        {
            //Debug.Log((hit.Hitbox != null) + " " + (hit.GameObject.tag == validTag) + " " + (hit.Hitbox.Root.Object.InputAuthority != Object.InputAuthority) + " " + (!hitList.Contains(hit.GameObject)));
            // TODO: Stop projectile from hitting caster if caster has same target tag
            if (hit.Hitbox != null && hit.Hitbox.Root.tag == validTag/* && hit.Hitbox.Root.Object.InputAuthority != Object.InputAuthority */&& !hitList.Contains(hit.Hitbox.Root.gameObject))
            {
                Debug.Log("Hit valid target");

                if (settings.attack.canCrit && (!settings.attack.critOnCritBoxesOnly || (hit.Hitbox.Root.tag == "Enemy" && hit.Hitbox.Root.GetComponent<EnemyGeneric>().critBox == hit.Hitbox)))
                {
                    CalculateCrit(settings.attack);
                }

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
                    DestroyAttack();
                }
            }

            else if (hit.Collider != null && !settings.ignoreObstacles)
            {
                // TODO: Ending effects/subattacks
                hitNormal = hit.Normal;
                hitPoint = hit.Point;
                SubAttacksOnEnd();
                ApplyEffectsOnEnd();
                DestroyAttack();
            }
        }
    }

    private void DestroyAttack()
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
