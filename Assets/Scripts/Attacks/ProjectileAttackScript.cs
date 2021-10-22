using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileAttackScript : AttackScript
{
    public float speed;

    public bool canGoThroughObjects;

    [Tooltip("If projectile has a specific destination, whether the projectile should re evaluate it's rotation after adjusting its position offset")]
    public bool reevluateRotationAfterLocalPositionOffset;

    public bool homing;

    public float homingStrength;

    private Rigidbody rigidbody;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private Transform target;

    public override void Start()
    {
        base.Start();
        transform.SetParent(null, true);

        if(destination.x != Vector3.negativeInfinity.x)
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

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * speed;

        if(homing)
        {
            var temp = Helpers.FindClosest(transform, validTag);

            if(temp.TryGetComponent(out EnemyGeneric e) && e.homingPoint != null)
            {
                target = e.homingPoint.transform;
            }

            else
            {
                target = temp.transform;
            }
        }
    }

    public override void Tick()
    {
        if (startingTime + attackDuration <= Time.time)
        {
            // Debug.Log("here");

            if (subAttacksOnEnd)
            {
                SpawnSubAttacks();
            }

            if(applyEffectsOnEnd)
            {
                attack.ApplyEffects(null, validTag, transform.position, transform.rotation, damageMod);
            }

            Destroy(gameObject);
        }

        else
        {
            // transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Debug.Log("bruh");
        if(!canGoThroughObjects && (other.tag == "Wall" || other.tag == "Ground"))
        {
            SetCollisionNormal(other);

            if (subAttacksOnEnd)
            {
                SpawnSubAttacks();
            }

            if(applyEffectsOnEnd)
            {
                attack.ApplyEffects(other.gameObject, validTag, transform.position, transform.rotation, damageMod);
            }

            Destroy(gameObject);
        }
    }

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(rigidbody.position);
    //        stream.SendNext(rigidbody.rotation);
    //        stream.SendNext(rigidbody.velocity);
    //    }
    //    else
    //    {
    //        networkPosition = (Vector3)stream.ReceiveNext();
    //        networkRotation = (Quaternion)stream.ReceiveNext();
    //        rigidbody.velocity = (Vector3)stream.ReceiveNext();

    //        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
    //        networkPosition += rigidbody.velocity * lag;
    //    }
    //}

    public void FixedUpdate()
    {
        if(homing)
        {
            rigidbody.velocity = transform.forward * speed;

            //Now Turn the Rocket towards the Target
            var rocketTargetRot = Quaternion.LookRotation(target.position - transform.position);

            rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, rocketTargetRot, homingStrength));
        }
    }
}
