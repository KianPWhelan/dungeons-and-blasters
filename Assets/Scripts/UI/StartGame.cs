using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        //StartGameSequence();
    }

    public void StartGameSequence()
    {
        gameManager.SendRoomDataToClients();
    }
}
