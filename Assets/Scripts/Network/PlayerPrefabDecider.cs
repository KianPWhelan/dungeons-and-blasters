using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerPrefabDecider : NetworkBehaviour
{
    [SerializeField]
    public NetworkObject playerPrefab;

    [SerializeField]
    public NetworkObject dmPrefab;

    [SerializeField]
    public BoolVariable isDungeonMaster;

    private bool pleaseDontDoThisAgain;

    public override void FixedUpdateNetwork()
    {
        if(Object.HasInputAuthority)
        {
            //pleaseDontDoThisAgain = true;

            if (isDungeonMaster.runtimeValue)
            {
                RPC_WhyTheHellIsThisTheOnlyWayToDoThis("true");
            }

            else
            {
                RPC_WhyTheHellIsThisTheOnlyWayToDoThis("false");
            }
        }
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_WhyTheHellIsThisTheOnlyWayToDoThis(string isDM/*Why tf can't this be a bool*/)
    {
        if (isDM == "true")
        {
            Debug.Log("why1");
            var position = new Vector3(0, 50, 0);
            PlayerSpawnerScript.dms.Add(Runner.Spawn(dmPrefab, position, Quaternion.identity, Object.InputAuthority));
        }

        else
        {
            Debug.Log("why2");
            var position = new Vector3(0, 10, 0);
            PlayerSpawnerScript.players.Add(Runner.Spawn(playerPrefab, position, Quaternion.identity, Object.InputAuthority));
        }

        Runner.Despawn(Object);
    }
}
