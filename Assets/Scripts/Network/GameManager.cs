using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : NetworkBehaviour
{
    private Database database;
    [SerializeField]
    private RoomGenerator roomGen;

    [SerializeField]
    private NetworkObject exitPrefab;

    public List<NetworkObject> players = new List<NetworkObject>();
    public List<NetworkObject> dms = new List<NetworkObject>();

    public void SendRoomDataToClients()
    {
        RPC_LoadRoom(System.Text.Encoding.UTF8.GetBytes(RoomSelect.selection));
    }

    //[Rpc(sources: RpcSources.All)]
    //public void RPC_AddPlayer(NetworkObject player)
    //{
    //    players.Add(player);
    //}

    public override void FixedUpdateNetwork()
    {
        if(CheckSpawnersDead() && Exit.instance != null)
        {
            Exit.instance.Activate();
        }
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    private void RPC_LoadRoom(byte[] roomData)
    {
        var roomJson = System.Text.Encoding.UTF8.GetString(roomData);
        roomGen.LoadRoomFromJson(roomJson);
        roomGen.SpawnAllEnemies();
        //var startPoint = roomGen.GetStartPoint();

        //if (Controller.LocalPlayerInstance != null)
        //{
        //    Controller.LocalPlayerInstance.transform.position = startPoint + Vector3.up * 2;
        //}

        //else if (DungeonMasterController.LocalPlayerInstance != null)
        //{
        //    StartCoroutine(CheckForNoEnemies());
        //    DungeonMasterController.LocalPlayerInstance.transform.position = startPoint + Vector3.up * 50;
        //}

        if(Object.HasStateAuthority)
        {
            MoveToStartPoint();
            SpawnExit();
        }
    }

    private void MoveToStartPoint()
    {
        Debug.Log("Moving players");
        Vector3 startPoint = roomGen.GetStartPoint();
        Debug.Log("Start Point: " + startPoint);

        foreach(NetworkObject player in PlayerSpawnerScript.players)
        {
            Debug.Log(player.name);
            var cc = player.GetComponent<NetworkCharacterController>();
            cc.Teleport(startPoint + Vector3.up * 5);
            cc.Velocity = Vector3.zero;
            player.GetComponent<PlayerMovement>().SetStartPoint(startPoint);
        }

        foreach(NetworkObject dm in PlayerSpawnerScript.dms)
        {
            dm.GetComponent<NetworkTransform>().Teleport(startPoint + Vector3.up * 50);
        }
    }

    private void SpawnExit()
    {
        Vector3 point = roomGen.GetAndDisableExit();
        Runner.Spawn(exitPrefab, point);
    }

    private bool CheckSpawnersDead()
    {
        foreach(Object o in EnemyManager.spawners)
        {
            if(o != null)
            {
                return false;
            }
        }

        return true;
    }
}
