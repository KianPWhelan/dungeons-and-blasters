using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackScript : MonoBehaviour
{
    public float attackDuration;

    public Vector3 localStartingPosition;

    public bool destroyOnHit = false;

    public bool canHitSameTargetMoreThanOnce = false;

    private string validTag;

    private Attack attack;

    private float startingTime;

    private List<GameObject> hitList = new List<GameObject>();

    public void OnEnable()
    {
        hitList = new List<GameObject>();
        startingTime = Time.time;
        gameObject.transform.position += localStartingPosition;
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
        if (startingTime + attackDuration > Time.time)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetAttack(Attack attack)
    {
        this.attack = attack;
    }

    public void SetValidTag(string tags)
    {
        validTag = tag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(attack != null && validTag != null && (other.tag == validTag || other.tag == "none") && !hitList.Contains(other.gameObject))
        {
            attack.ApplyEffects(other.gameObject, validTag);
            hitList.Add(other.gameObject);

            if(destroyOnHit)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
