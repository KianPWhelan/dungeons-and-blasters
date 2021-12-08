using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class StartGame : MonoBehaviour
{
    private GameManager gameManager;
    private NetworkRunner runner;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        runner = NetworkRunner.GetRunnerForGameObject(gameObject);
        //StartGameSequence();
    }

    public void StartGameSequence()
    {
        gameManager.SendRoomDataToClients();
        runner.SessionInfo.IsOpen = false;
    }
}
