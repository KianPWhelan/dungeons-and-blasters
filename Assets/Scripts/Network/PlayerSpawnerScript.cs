using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerSpawnerScript : SimulationBehaviour, IPlayerJoined
{
    [SerializeField]
    private NetworkObject playerPrefab;

    void IPlayerJoined.PlayerJoined(PlayerRef player)
    {
        Debug.Log("Player " + player + " joined the game");
        var position = new Vector3(0, 10, 0);
        Runner.Spawn(playerPrefab, position, Quaternion.identity, player);
    }
}
