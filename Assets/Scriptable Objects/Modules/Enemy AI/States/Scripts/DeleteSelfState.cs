using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

[CreateAssetMenu(menuName = "States/DeleteSelfState")]
public class DeleteSelfState : State
{


    public override void OnEnter(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
        if(self.GetPhotonView().IsMine)
        {
            PhotonNetwork.Destroy(self);
        }
    }

    public override void OnExit(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
    }

    public override void Tick(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
    }
}
