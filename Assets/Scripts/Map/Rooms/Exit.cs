using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Exit : NetworkBehaviour
{
    public GameObject visual;

    public Collider collider;

    public static Exit instance;

    public void Start()
    {
        instance = this;
    }

    public void Activate()
    {
        RPC_Activate();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(!Object.HasStateAuthority)
        {
            return;
        }

        if(collision.gameObject.tag == "Player" && collision.gameObject.TryGetComponent(out NetworkCharacterController cc))
        {
            TeleportPlayerToVoid(cc);
        }
    }

    private void TeleportPlayerToVoid(NetworkCharacterController cc)
    {
        cc.Teleport(new Vector3(2000, 0, 0));
        cc.Config.Gravity = Vector3.zero;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void RPC_Activate()
    {
        visual.SetActive(true);
        collider.enabled = true;
    }
}
