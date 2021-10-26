using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanAttackScript : AttackScript
{
    public float maxLength = 100f;

    public float radius = 0.1f;

    public bool canGoThroughObjects;

    public bool piercing;

    public bool endOnPlayerStopAttacking;

    [Tooltip("The VoxelStaticBeam prefab this attack uses, should be a child of the attack object")]
    public GameObject voxelStaticBean;

    [Tooltip("Should the beam follow it's caster during it's lifespan, or should it stay where it was fired?")]
    public bool followCaster;

    [Tooltip("If projectile has a specific destination, whether the projectile should re evaluate it's rotation after adjusting its position offset")]
    public bool reevluateRotationAfterLocalPositionOffset;

    private VoxelBeamStatic beanController;

    private string parentTag;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        parentTag = transform.parent.tag;

        if(!followCaster)
        {
            transform.SetParent(null);
        }

        if (destination.x != Vector3.negativeInfinity.x)
        {
            Debug.Log("DESTINATION");
            Debug.Log(destination);
            transform.rotation = Quaternion.LookRotation((destination - transform.position).normalized);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + localRotationPosition + accuracyOffset);
            transform.position += transform.forward * localStartingPosition.z;
            transform.position += transform.right * localStartingPosition.x;
            transform.position += transform.up * localStartingPosition.y;

            if (reevluateRotationAfterLocalPositionOffset)
            {
                transform.rotation = Quaternion.LookRotation((destination - transform.position).normalized);
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + localRotationPosition + accuracyOffset);
            }
        }

        if(voxelStaticBean != null)
        {
            beanController = voxelStaticBean.GetComponent<VoxelBeamStatic>();
            beanController.beamLength = maxLength;
            beanController.ignoreList.Add(parentTag, "");

            if(canGoThroughObjects)
            {
                beanController.ignoreList.Add("Wall", "");
                beanController.ignoreList.Add("Ground", "");
            }

            if(piercing)
            {
                beanController.ignoreList.Add(validTag, "");
            }
        }
    }

    public override void Tick()
    {
        if(transform.parent != null)
        {
            transform.rotation = rotater.transform.rotation;
        }

        if (startingTime + attackDuration <= Time.time)
        {
            // Debug.Log("here");

            if (subAttacksOnEnd)
            {
                SpawnSubAttacks();
            }

            if (applyEffectsOnEnd)
            {
                attack.ApplyEffects(null, validTag, transform.position, transform.rotation, damageMod);
            }

            Destroy(gameObject);
            return;
        }

        RaycastHit[] hits;

        Ray ray = new Ray(transform.position, transform.forward);

        hits = Physics.SphereCastAll(ray, radius, maxLength);

        System.Array.Sort(hits, delegate (RaycastHit hit1, RaycastHit hit2) { return hit1.distance.CompareTo(hit2.distance); });

        foreach(RaycastHit hit in hits)
        {
            ProcessHit(hit.collider);

            if (!canGoThroughObjects && (hit.collider.tag == "Wall" || hit.collider.tag == "Ground"))
            {
                break;
            }

            if (!piercing && hit.collider.tag == validTag)
            {
                break;
            }
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        return;
    }

    private void ProcessHit(Collider other)
    {
        Debug.Log("Valid tag: " + validTag + "  Other tag: " + other.tag + "  Other name: " + other.name);
        Debug.Log(attack);
        if (attack != null && validTag != null && (other.tag == validTag || validTag == "none") && (!hitList.Contains(other.gameObject)))
        {
            Debug.Log("Collision with valid tag");

            float crit = 1f;

            if (attack.canCrit && other.TryGetComponent(out EnemyGeneric e))
            {
                int roll = Random.Range(0, 1000);
                Debug.Log("Roll: " + roll);

                if ((other == e.critBox || !attack.critOnCritBoxesOnly) && roll <= attack.critChance)
                {
                    crit = attack.critMultiplier;
                }
            }

            attack.ApplyEffects(other.gameObject, validTag, transform.position, transform.rotation, damageMod * crit);
            hitList.Add(other.gameObject);
            SetCollisionNormal(other);

            if (other.TryGetComponent(out Knockback k))
            {
                k.ApplyKnockback(transform, attack.knockbackStrength);
            }

            if (subAttacksOnHit)
            {
                SpawnSubAttacks();
            }

            if (destroyOnHit)
            {
                Destroy(gameObject);
            }

            if (canHitSameTargetMoreThanOnce)
            {
                StartCoroutine(RemoveFromHitList(other.gameObject, multiHitCooldown));
            }
        }

        if (!canGoThroughObjects && (other.tag == "Wall" || other.tag == "Ground"))
        {
            SetCollisionNormal(other);

            // TODO: Make bouncing not sucky
            //if (bouncing && numBounces > 0)
            //{
            //    rigidbody.velocity = Vector3.Reflect(rigidbody.velocity, collisionNormal) * bouncingStrength;
            //    numBounces--;
            //    selfCollider.enabled = false;
            //    StartCoroutine(EnableCollision(colliionDisableTime));
            //    return;
            //}

            if (subAttacksOnEnd)
            {
                SpawnSubAttacks();
            }

            if (applyEffectsOnEnd)
            {
                attack.ApplyEffects(other.gameObject, validTag, transform.position, transform.rotation, damageMod);
            }

            // Destroy(gameObject);
        }
    }

    public void EndBeam()
    {
        if(!endOnPlayerStopAttacking)
        {
            return;
        }

        if (subAttacksOnEnd)
        {
            SpawnSubAttacks();
        }

        if (applyEffectsOnEnd)
        {
            attack.ApplyEffects(null, validTag, transform.position, transform.rotation, damageMod);
        }

        Destroy(gameObject, 0.01f);
    }
}
