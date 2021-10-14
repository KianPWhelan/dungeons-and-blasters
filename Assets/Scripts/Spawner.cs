using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Spawns the object locally across all clients
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="info"></param>
    public void Spawn(string objectName, Vector3 position, Quaternion rotation, object[] info, float delay)
    {
        photonView.RPC("LocalInstantiate", RpcTarget.All, objectName, position, rotation, info, delay);
    }

    [PunRPC]
    private void LocalInstantiate(string objectName, Vector3 position, Quaternion rotation, object[] info, float delay)
    {
        Debug.Log("Spawning " + objectName);
        GameObject newObject = (GameObject)Instantiate(Resources.Load(objectName), position, rotation);
        newObject.SetActive(false);

        if(newObject.TryGetComponent(out AttackScript a))
        {
            if (info[0] != null)
            {
                a.parentId = (int)info[0];
            }

            a.validTag = (string)info[1];
            a.damageMod = (float)info[2];

            if (info[3] != null)
            {
                a.destination = (Vector3)info[3];
            }

            else
            {
                a.destination = Vector3.negativeInfinity;
            }
        }

        StartCoroutine(Helpers.Timeout(
            () =>
            {
                newObject.SetActive(true);
            },
            delay
        ));
    }
}
