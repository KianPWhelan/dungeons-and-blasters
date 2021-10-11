using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileAttackScript : AttackScript
{
    public float speed;

    public bool canGoThroughObjects;

    public override void Start()
    {
        base.Start();
        transform.SetParent(null, true);
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

            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
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

            if(photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }   
        }
    }
}
