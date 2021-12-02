using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Spawner : NetworkBehaviour
{
    /// <summary>
    /// Spawns the object locally across all clients
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="info"></param>
    public void Spawn(GameObject prefab, Vector3 position, Quaternion rotation, object[] info, float delay, int ownerId, NetworkObject owner)
    {
        //if (!PhotonView.Find(ownerId).IsMine)
        //{
        //    return;
        //}

        //NetworkInstantiate(obj, position, rotation, info, delay, ownerId);

        // photonView.RPC("LocalInstantiate", RpcTarget.All, objectName, position, rotation, info, delay, ownerId);

        Debug.Log("Destination");
        Debug.Log(info[3]);

        PlayerRef inputAuth = Object.InputAuthority;

        if(owner != null)
        {
            inputAuth = owner.InputAuthority;
        }

        StartCoroutine(Helpers.Timeout(
            () =>
            {
                Runner.Spawn(prefab.GetComponent<NetworkObject>(), position, rotation, inputAuth, (Runner, obj) =>
                {
                    obj.GetComponent<AttackComponent>().InitNetworkState((string)info[1], (float)info[2], info[3], owner, (int)info[4], (int)info[5], (bool)info[6]);
                });
            },
            delay
        ));
    }

    public void Spawn(GameObject prefab, GameObject self, object[] info, float delay, int ownerId, NetworkObject owner)
    {
        //if (!PhotonView.Find(ownerId).IsMine)
        //{
        //    return;
        //}

        //NetworkInstantiate(obj, position, rotation, info, delay, ownerId);

        // photonView.RPC("LocalInstantiate", RpcTarget.All, objectName, position, rotation, info, delay, ownerId);

        //Debug.Log("Bruh");
        //Debug.Log(self.transform.position);
        //Debug.Log(self.transform.rotation);
        //Debug.Log(self.name);

        //if(useOverrideRotation)
        //{
        //    StartCoroutine(Helpers.Timeout(
        //        () =>
        //        {
        //            Runner.Spawn(prefab.GetComponent<NetworkObject>(), self.transform.position, rotation, Object.InputAuthority, (Runner, obj) =>
        //            {
        //                obj.GetComponent<AttackComponent>().InitNetworkState((string)info[1], (float)info[2], info[3]);
        //            });
        //        },
        //        delay
        //    ));
        //}

        //else
        //{
        PlayerRef inputAuth = Object.InputAuthority;

        if (owner != null)
        {
            inputAuth = owner.InputAuthority;
        }

        StartCoroutine(Helpers.Timeout(
                () =>
                {
                    Runner.Spawn(prefab.GetComponent<NetworkObject>(), self.transform.position, self.transform.rotation, inputAuth, (Runner, obj) =>
                    {
                        obj.GetComponent<AttackComponent>().InitNetworkState((string)info[1], (float)info[2], info[3], owner, (int)info[4], (int)info[5], (bool)info[6]);
                    });
                },
                delay
            ));
        //}
    }

    //[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    //private void NetworkInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, object[] info, float delay, int ownerId)
    //{
    //    StartCoroutine(Helpers.Timeout(
    //        () =>
    //        {
    //            Runner.Spawn(prefab.GetComponent<NetworkObject>(), position, rotation, Object.InputAuthority, (Runner, obj) =>
    //            {
    //                prefab.GetComponent<AttackComponent>().InitNetworkState((string)info[1], (float)info[2]);
    //            });
    //        },
    //        delay
    //    ));
    //}

    //[PunRPC]
    //private void LocalInstantiate(string objectName, Vector3 position, Quaternion rotation, object[] info, float delay, int ownerId)
    //{
    //    // Debug.Log("Spawning " + objectName);
    //    GameObject newObject = (GameObject)Instantiate(Resources.Load(objectName), position, rotation);
    //    newObject.SetActive(false);

    //    if(newObject.TryGetComponent(out AttackScriptDeprecated a))
    //    {
    //        a.ownerId = ownerId;

    //        if (info[0] != null)
    //        {
    //            a.parentId = (int)info[0];
    //        }

    //        a.validTag = (string)info[1];
    //        a.damageMod = (float)info[2];

    //        if (info[3] != null)
    //        {
    //            a.destination = (Vector3)info[3];
    //        }

    //        else
    //        {
    //            a.destination = Vector3.negativeInfinity;
    //        }
    //    }

    //    StartCoroutine(Helpers.Timeout(
    //        () =>
    //        {
    //            newObject.SetActive(true);
    //        },
    //        delay
    //    ));
    //}
}
