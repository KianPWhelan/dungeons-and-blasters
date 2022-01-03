using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[OrderAfter(typeof(EnemyGeneric))]
public class SquadManager : NetworkBehaviour
{
    public static SquadManager instance;

    public List<Squad> squads = new List<Squad>();

    public override void Spawned()
    {
        instance = this;
    }

    public override void FixedUpdateNetwork()
    {
        List<Squad> squadsToDelete = new List<Squad>();

        foreach(Squad squad in squads)
        {
            if(squad.units.Count <= 0)
            {
                squadsToDelete.Add(squad);
            }
        }

        foreach(Squad squad in squadsToDelete)
        {
            DeleteSquad(squad);
        }
    }

    public void CreateSquad(List<EnemyGeneric> units)
    {
        if(!Object.HasStateAuthority)
        {
            List<NetworkObject> temp = new List<NetworkObject>();

            foreach(EnemyGeneric e in units)
            {
                temp.Add(e.Object);
            }

            RPC_CreateSquad(temp.ToArray());
            return;
        }

        Squad squad = new Squad();

        foreach(EnemyGeneric e in units)
        {
            e.AssignSquad(squad);
        }

        squads.Add(squad);
    }

    public void DeleteSquad(Squad squad)
    {
        squads.Remove(squad);
        squad.Delete();
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_CreateSquad(NetworkObject[] units)
    {
        List<EnemyGeneric> e = new List<EnemyGeneric>();
        
        foreach(NetworkObject o in units)
        {
            e.Add(o.GetComponent<EnemyGeneric>());
        }

        CreateSquad(e);
    }
}
