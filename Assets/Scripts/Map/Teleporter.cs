using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Com.OfTomorrowInc.DMShooter;

public class Teleporter : MonoBehaviour
{
    public GameObject target;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && other.gameObject.GetPhotonView().IsMine)
        {
            var c = other.gameObject.GetComponent<Controller>();

            if(Time.time - c.timeOfLastTp < 3)
            {
                return;
            }

            else
            {
                c.timeOfLastTp = Time.time;
            }

            //target.transform.parent.parent.GetComponent<Room>().SendActivationCommand();
            //GameManager.single.SendRoomActivation(target.transform.parent.parent.GetComponent<Room>().gridSlot);
            other.transform.position = target.transform.position;
        }
    }
}
