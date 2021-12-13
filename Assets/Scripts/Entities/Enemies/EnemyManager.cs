using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EnemyManager : NetworkBehaviour
{
    public static List<NetworkObject> enemies = new List<NetworkObject>();

    public override void Spawned()
    {
        FindAllEnemies();
    }

    //[Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void AddEnemy(NetworkObject enemy)
    {
        RPC_FindEnemy(enemy);
    }
    
    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    private void RPC_FindEnemy(NetworkObject enemy)
    {
        enemies.Add(enemy);
    }

    //public void SendFindEnemiesCommand()
    //{
    //    if(Runner.IsServer)
    //    {
    //        RPC_FindAllEnemies();
    //    }
    //}

    //[Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    //private void RPC_FindAllEnemies()
    //{
    //    FindAllEnemies();
    //}

    private void FindAllEnemies()
    {
        enemies = new List<NetworkObject>();
        var temp = FindObjectsOfType<EnemyGeneric>();

        foreach (EnemyGeneric e in temp)
        {
            enemies.Add(e.GetComponent<NetworkObject>());
            Debug.Log("Adding " + e.name);
        }
    }
}
