using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttackScript : MonoBehaviour
{
    [Tooltip("The Attack ScriptableObject that this attack uses")]
    public Attack attack;

    [Tooltip("Attacks that will be spawned depending on the values of the next 2 parameters")]
    public List<Attack> subAttacks = new List<Attack>();

    [Tooltip("Spawn sub attacks on hitting a valid target")]
    public bool subAttacksOnHit;

    [Tooltip("Spawn sub attacks on end of attack (unless end of attack was a valid target hit)")]
    public bool subAttacksOnEnd;

    [Tooltip("Set direction of sub attack to normal of collision surface")]
    public bool subAttacksAtSurfaceNormal;

    [Tooltip("How long the attack can exist before despawning")]
    public float attackDuration;

    [Tooltip("The starting position of the attack relative to its parent")]
    public Vector3 localStartingPosition;

    [Tooltip("The starting rotation of the attack relative to its parent")]
    public Vector3 localRotationPosition;

    [Tooltip("Defines the angle of the cone that the attack can miss its intended target by")]
    public float spread;

    [Tooltip("Whether the attack should be destroyed on collision with valid target")]
    public bool destroyOnHit;

    [Tooltip("Whether the attack can hit the same target multiple times in its lifespan")]
    public bool canHitSameTargetMoreThanOnce;

    [Tooltip("If target can be hit multiple times, how often between hits")]
    public float multiHitCooldown;

    [Tooltip("Whether this attack should attempt to apply its effects when it ends even if it didnt hit a valid target")]
    public bool applyEffectsOnEnd;

    [Tooltip("Visual effect to be instantiated when the attack is destroyed")]
    public GameObject visualEndEffect;

    [Tooltip("Visual effect to be instantiated upon attack instantiation")]
    public GameObject visualStartEffect;

    [Tooltip("Position offset of visual start effect")]
    public Vector3 visualStartEffectPositionOffset;

    [Tooltip("Rotation offset of visual start effect")]
    public Vector3 visualStartEffectRotationOffset;

    [HideInInspector]
    public string validTag;

    [HideInInspector]
    public float startingTime;

    [HideInInspector]
    public List<GameObject> hitList = new List<GameObject>();

    [HideInInspector]
    public int parentId;

    [HideInInspector]
    public Vector3 collisionNormal;

    [HideInInspector]
    public Vector3 collisionPoint;

    [HideInInspector]
    public float damageMod;

    [HideInInspector]
    public Vector3 destination;

    [HideInInspector]
    public bool isMine = false;

    [HideInInspector]
    public Vector3 accuracyOffset;

    [HideInInspector]
    public Rotater rotater;

    public virtual void Start()
    {
        Debug.Log("Starting " + name);

        if(parentId != 0)
        {
            transform.SetParent(PhotonView.Find(parentId).transform);
            transform.position = transform.parent.position;
            rotater = transform.parent.GetComponentInChildren<Rotater>();
            transform.rotation = rotater.transform.rotation;
            Debug.Log("Parent set to " + transform.parent.name);
        }

        if(transform.parent != null && transform.parent.gameObject.GetPhotonView().IsMine)
        {
            isMine = true;
        }
        
        hitList = new List<GameObject>();
        startingTime = Time.time;

        accuracyOffset = Random.insideUnitCircle * spread;


        if (!(this is ProjectileAttackScript))
        {
            Debug.Log("whyhere");
            transform.localPosition += localStartingPosition; //= gameObject.transform.localPosition + localStartingPosition;
        }
            
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + localRotationPosition + accuracyOffset);

        if(visualStartEffect != null)
        {
            var fx = Instantiate(visualStartEffect, transform.position + visualStartEffectPositionOffset, Quaternion.Euler(transform.rotation.eulerAngles + visualStartEffectRotationOffset));
            Destroy(fx, 5f);
        }
    }

    public void OnEnable()
    {
        hitList = new List<GameObject>();
        startingTime = Time.time;
        // transform.localPosition += localStartingPosition; //= gameObject.transform.localPosition + localStartingPosition;\
    }

    //public void OnDisable()
    //{
    //    attack = null;
    //    validTag = null;
    //}

    public void Update()
    {
        Tick();
    }

    public virtual void Tick()
    {
        if (startingTime + attackDuration <= Time.time)
        {
            Debug.Log("here");

            if (subAttacksOnEnd)
            {
                SpawnSubAttacks();
            }

            if (applyEffectsOnEnd)
            {
                attack.ApplyEffects(null, validTag, transform.position, transform.rotation, damageMod);
            }

            Destroy(gameObject);
        }
    }

    public void SetAttack(Attack attack)
    {
        this.attack = attack;
    }

    public void SetValidTag(string tag)
    {
        validTag = tag;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("Valid tag: " + validTag+ "  Other tag: " + other.tag + "  Other name: " + other.name);
        Debug.Log(attack);
        if(attack != null && validTag != null && (other.tag == validTag || validTag == "none") && (!hitList.Contains(other.gameObject)))
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

            if(other.TryGetComponent(out Knockback k))
            {
                k.ApplyKnockback(transform, attack.knockbackStrength);
            }

            if (subAttacksOnHit)
            {
                SpawnSubAttacks();
            }

            if(destroyOnHit)
            {
                Destroy(gameObject);
            }

            if(canHitSameTargetMoreThanOnce)
            {
                StartCoroutine(RemoveFromHitList(other.gameObject, multiHitCooldown));
            }
        }
    }

    //public void OnPhotonInstantiate(PhotonMessageInfo info)
    //{
    //    object[] instantiationData = info.photonView.InstantiationData;

    //    if(instantiationData[0] != null)
    //    {
    //        parentId = (int)instantiationData[0];
    //    }
        
    //    validTag = (string)instantiationData[1];
    //    damageMod = (float)instantiationData[2];

    //    if(instantiationData[3] != null)
    //    {
    //        destination = (Vector3)instantiationData[3];
    //    }
        
    //    else
    //    {
    //        destination = Vector3.negativeInfinity;
    //    }
    //}

    public void SpawnSubAttacks()
    {
        foreach(Attack attack in subAttacks)
        {
            if(subAttacksAtSurfaceNormal)
            {
                if(transform.parent != null)
                {
                    attack.PerformAttack(transform.parent.gameObject, 0f, validTag, true, collisionPoint + collisionNormal);
                }

                else
                {
                    Debug.Log("here");
                    attack.PerformAttack(collisionPoint, transform.rotation, damageMod, validTag, 0f, collisionPoint + collisionNormal);
                }
            }

            else
            {
                if (transform.parent != null)
                {
                    attack.PerformAttack(transform.parent.gameObject, 0f, validTag);
                }

                else
                {
                    attack.PerformAttack(transform.position, transform.rotation, damageMod, validTag);
                }
            }
        }
    }

    // TODO: fix to make end effects spawn at normal to hit point
    public void SetCollisionNormal(Collider other)
    {
        var point = other.ClosestPointOnBounds(transform.position);
        var pos = transform.position - (transform.forward * 10f);
        Debug.Log(pos);
        Debug.Log(point);
        var rayDirection = pos - point;
        Debug.Log(rayDirection);
        RaycastHit hit;

        if (other.Raycast(new Ray(pos, point - pos), out hit, 1000))
        {
            Debug.Log(hit.transform.gameObject.name);
            collisionNormal = hit.normal;
            Debug.Log(collisionNormal);
        }

        collisionPoint = point;

        //var collisionPoint = other.ClosestPointOnBounds(transform.position);
        //var center = other.bounds.center;
        //Vector3 direction = collisionPoint - center;
        //collisionNormal = direction.normalized;
        //Debug.Log(collisionNormal);
    }

    public void OnDestroy()
    {
        Debug.Log("Destroying " + name);

        if(visualEndEffect)
        {
            var fx = Instantiate(visualEndEffect, transform.position, transform.rotation);
            Destroy(fx, 5f);
        }
    }

    public IEnumerator RemoveFromHitList(GameObject hit, float delay)
    {
        yield return new WaitForSeconds(delay);
        hitList.Remove(hit);
    }
}
