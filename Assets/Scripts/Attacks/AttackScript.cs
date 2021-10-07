using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
public class AttackScript : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public Attack attack;

    public List<Attack> subAttacks = new List<Attack>();

    public bool subAttacksOnHit;

    public bool subAttacksOnEnd;

    public float attackDuration;

    public Vector3 localStartingPosition;

    public bool destroyOnHit;

    public bool canHitSameTargetMoreThanOnce;

    public GameObject visualEndEffect;

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

    public virtual void Start()
    {
        if(parentId != 0)
        {
            transform.SetParent(PhotonView.Find(parentId).transform);
            transform.position = transform.parent.position;
            transform.rotation = transform.parent.GetComponentInChildren<Rotater>().transform.rotation;
        }
        
        hitList = new List<GameObject>();
        startingTime = Time.time;
        transform.localPosition += localStartingPosition; //= gameObject.transform.localPosition + localStartingPosition;\
    }

    public override void OnEnable()
    {
        hitList = new List<GameObject>();
        startingTime = Time.time;
        transform.localPosition += localStartingPosition; //= gameObject.transform.localPosition + localStartingPosition;\
    }

    public override void OnDisable()
    {
        attack = null;
        validTag = null;
    }

    public void Update()
    {
        Tick();
    }

    public virtual void Tick()
    {
        if (startingTime + attackDuration <= Time.time)
        {
            Debug.Log("here");
            PhotonNetwork.Destroy(gameObject);

            if(subAttacksOnEnd)
            {
                SpawnSubAttacks();
            }
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
        if(attack != null && validTag != null && (other.tag == validTag || validTag == "none") && (!hitList.Contains(other.gameObject) || canHitSameTargetMoreThanOnce))
        {
            Debug.Log("Collision with valid tag");
            attack.ApplyEffects(other.gameObject, validTag);
            hitList.Add(other.gameObject);
            // SetCollisionNormal(other);

            if (subAttacksOnHit)
            {
                SpawnSubAttacks();
            }

            if(destroyOnHit)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;

        if(instantiationData[0] != null)
        {
            parentId = (int)instantiationData[0];
        }
        
        validTag = (string)instantiationData[1];
    }

    public void SpawnSubAttacks()
    {
        foreach(Attack attack in subAttacks)
        {
            if(transform.parent != null)
            {
                attack.PerformAttack(transform.parent.gameObject, validTag);
            }

            else
            {
                attack.PerformAttack(transform.position, transform.rotation, validTag);
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
        var fx = Instantiate(visualEndEffect, transform.position, transform.rotation);
        Destroy(fx, 5f);
    }
}
