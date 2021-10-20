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

    public virtual void Start()
    {
        Debug.Log("Starting " + name);

        if(parentId != 0)
        {
            transform.SetParent(PhotonView.Find(parentId).transform);
            transform.position = transform.parent.position;
            transform.rotation = transform.parent.GetComponentInChildren<Rotater>().transform.rotation;
            Debug.Log("Parent set to " + transform.parent.name);
        }

        if(transform.parent != null && transform.parent.gameObject.GetPhotonView().IsMine)
        {
            isMine = true;
        }
        
        hitList = new List<GameObject>();
        startingTime = Time.time;

        accuracyOffset = Random.insideUnitCircle * spread;

        transform.localPosition += localStartingPosition; //= gameObject.transform.localPosition + localStartingPosition;
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
        transform.localPosition += localStartingPosition; //= gameObject.transform.localPosition + localStartingPosition;\
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
        if(attack != null && validTag != null && (other.tag == validTag || validTag == "none") && (!hitList.Contains(other.gameObject) || canHitSameTargetMoreThanOnce))
        {
            Debug.Log("Collision with valid tag");
            attack.ApplyEffects(other.gameObject, validTag, transform.position, transform.rotation, damageMod);
            hitList.Add(other.gameObject);
            // SetCollisionNormal(other);

            if (subAttacksOnHit)
            {
                SpawnSubAttacks();
            }

            if(destroyOnHit)
            {
                Destroy(gameObject);
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
            if(transform.parent != null)
            {
                attack.PerformAttack(transform.parent.gameObject, 0f, validTag);
            }

            else
            {
                attack.PerformAttack(transform.position, transform.rotation, damageMod, validTag);
            }
        }
    }

    // TODO: fix to make end effects spawn at normal to hit point
    public void SetCollisionNormal(Collider other)
    {
        var point = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        var pos = transform.position - (transform.forward * 1f);
        Debug.Log(pos);
        Debug.Log(point);
        var rayDirection = pos - point;
        Debug.Log(rayDirection);
        RaycastHit hit;

        if (Physics.Raycast(pos, rayDirection, out hit))
        {
            Debug.Log(hit.transform.gameObject.name);
            collisionNormal = hit.normal;
        }

        collisionPoint = point;
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
}
