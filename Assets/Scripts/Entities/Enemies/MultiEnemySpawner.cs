using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Com.OfTomorrowInc.DMShooter;

public class MultiEnemySpawner : EnemyGeneric
{
    public GameObject enemyToSpawn;
    public int numToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        if(!gameObject.GetPhotonView().IsMine)
        {
            return;
        }

        string name = enemyToSpawn.name;

        for(int i = 0; i < numToSpawn; i++)
        {
            GameManager.enemies.Add(PhotonNetwork.Instantiate(name, transform.position, Quaternion.identity));
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
