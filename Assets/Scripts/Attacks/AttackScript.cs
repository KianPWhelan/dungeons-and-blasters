using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
public class AttackScript : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public Attack attack;

    public float attackDuration;

    public Vector3 localStartingPosition;

    public bool destroyOnHit;

    public bool canHitSameTargetMoreThanOnce;

    [HideInInspector]
    public string validTag;

    [HideInInspector]
    public float startingTime;

    [HideInInspector]
    public List<GameObject> hitList = new List<GameObject>();

    [HideInInspector]
    public int parentId;

    public virtual void Start()
    {
        transform.SetParent(PhotonView.Find(parentId).transform);
        hitList = new List<GameObject>();
        startingTime = Time.time;
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.GetComponentInChildren<Rotater>().transform.rotation;
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

            if(destroyOnHit)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        parentId = (int)instantiationData[0];
        validTag = (string)instantiationData[1];
    }
}
