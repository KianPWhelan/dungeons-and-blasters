using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

[RequireComponent(typeof(Collider))]
public class AttackScript : MonoBehaviour
{
    public ObjectPool pool;

    public float attackDuration;

    public Vector3 localStartingPosition;

    public bool destroyOnHit;

    public bool canHitSameTargetMoreThanOnce;

    private string validTag;

    private Attack attack;

    private float startingTime;

    private List<GameObject> hitList = new List<GameObject>();

    public void Start()
    {
        //FindObjectOfType<Spawner>().Spawn(gameObject.GetComponent<NetworkObject>());
        hitList = new List<GameObject>();
        startingTime = Time.time;
        transform.localPosition += localStartingPosition; //= gameObject.transform.localPosition + localStartingPosition;\
    }

    public void OnEnable()
    {
        hitList = new List<GameObject>();
        startingTime = Time.time;
        transform.localPosition += localStartingPosition; //= gameObject.transform.localPosition + localStartingPosition;\
    }

    public void OnDisable()
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
            pool.Despawn(gameObject);
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Valid tag: " + validTag+ "  Other tag: " + other.tag + "  Other name: " + other.name);
        if(attack != null && validTag != null && (other.tag == validTag || validTag == "none") && (!hitList.Contains(other.gameObject) || canHitSameTargetMoreThanOnce))
        {
            Debug.Log("Collision with valid tag");
            attack.ApplyEffects(other.gameObject, validTag);
            hitList.Add(other.gameObject);

            if(destroyOnHit)
            {
                pool.Despawn(gameObject);
            }
        }
    }
}
