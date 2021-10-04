using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class ConnectUI : MonoBehaviour
{
    [SerializeField]
    private NetworkManager networkManager;

    public void StartClient()
    {
        networkManager.StartClient();
    }
}
