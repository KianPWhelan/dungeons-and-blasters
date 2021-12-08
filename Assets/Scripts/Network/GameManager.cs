using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : NetworkBehaviour
{
    private Database database;
    [SerializeField]
    private RoomGenerator roomGen;

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
    }

    private void MoveToStartPoint()
    {
        Vector3 startPoint = roomGen.GetStartPoint();

    }
}
