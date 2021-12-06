using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerSpawnerScript : SimulationBehaviour, IPlayerJoined
{
    [SerializeField]
    private NetworkObject playerPrefab;

    [SerializeField]
    private NetworkObject dmPrefab;

    [SerializeField]
    private BoolVariable isDungeonMaster;

    void IPlayerJoined.PlayerJoined(PlayerRef player)
    {
        Debug.Log("Player " + player + " joined the game");

        if (isDungeonMaster.runtimeValue)
        {
            var position = new Vector3(0, 50, 0);
            Runner.Spawn(dmPrefab, position, Quaternion.identity, player);
        }

        else
        {
            var position = new Vector3(0, 10, 0);
            Runner.Spawn(playerPrefab, position, Quaternion.identity, player);
        }
    }
}
