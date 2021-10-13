using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileAttackScript : AttackScript
{
    public float speed;

    public bool canGoThroughObjects;

    private Rigidbody rigidbody;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    public override void Start()
    {
        base.Start();
        transform.SetParent(null, true);

        if(destination.x != Vector3.negativeInfinity.x)
        {
            Debug.Log("In Re rotate");
            transform.rotation = Quaternion.LookRotation((destination - transform.position).normalized);
        }

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * speed;
    }

    public override void Tick()
    {
        if (startingTime + attackDuration <= Time.time)
        {
            Debug.Log("here");

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
        if(!canGoThroughObjects && other.tag == "Wall" || other.tag == "Ground")
        {
            // SetCollisionNormal(other);

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

    //public void FixedUpdate()
    //{
    //    if (!photonView.IsMine)
    //    {
    //        rigidbody.position = Vector3.MoveTowards(rigidbody.position, networkPosition, Time.fixedDeltaTime);
    //        rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
    //    }
    //}
}
