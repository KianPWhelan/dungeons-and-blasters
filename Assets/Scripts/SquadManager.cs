using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[OrderAfter(typeof(EnemyGeneric))]
public class SquadManager : NetworkBehaviour
{
    public static SquadManager instance;

    public List<Squad> squads = new List<Squad>();

    public List<Squad> squadsToDelete = new List<Squad>();

    public override void Spawned()
    {
        instance = this;
    }

    public override void FixedUpdateNetwork()
    {
        squadsToDelete = new List<Squad>();

        foreach(Squad squad in squads)
        {
            if(squad.units.Count <= 0)
            {
                squadsToDelete.Add(squad);
            }

            else
            {
                squad.Process();
            }
        }

        foreach(Squad squad in squadsToDelete)
        {
            DeleteSquad(squad);
        }
    }

    public void CreateSquad(List<EnemyGeneric> units, Vector3 destination)
    {
        if(!Object.HasStateAuthority)
        {
            List<NetworkObject> temp = new List<NetworkObject>();

            foreach(EnemyGeneric e in units)
            {
                temp.Add(e.Object);
            }

            RPC_CreateSquad(temp.ToArray(), destination);
            return;
        }

        Squad squad = new Squad();

        foreach(EnemyGeneric e in units)
        {
            e.AssignSquad(squad);
        }

        squad.Start();
        squad.SetDestination(destination);

        squads.Add(squad);
    }

    public void DeleteSquad(Squad squad)
    {
        squads.Remove(squad);
        squad.Delete();
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_CreateSquad(NetworkObject[] units, Vector3 destination)
    {
        List<EnemyGeneric> e = new List<EnemyGeneric>();
        
        foreach(NetworkObject o in units)
        {
            e.Add(o.GetComponent<EnemyGeneric>());
        }

        CreateSquad(e, destination);
    }
}
