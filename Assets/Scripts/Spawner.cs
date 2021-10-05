using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.Serialization;

public class Spawner : NetworkBehaviour
{
    NetworkVariable<int> objToReturn = new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone, ReadPermission = NetworkVariablePermission.Everyone });
    NetworkVariable<bool> ready = new NetworkVariable<bool>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone, ReadPermission = NetworkVariablePermission.Everyone });

    public GameObject Spawn(GameObject obj, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (IsServer)
        {
            var newObj = Instantiate(obj, position, rotation, parent);
            newObj.GetComponent<NetworkObject>().Spawn();
            return newObj;
        }

        ready.Value = false;
        SpawnServerRpc(obj.name, position, rotation, parent.GetInstanceID());
        while (!ready.Value) { }
        return (GameObject)Helpers.FindObjectFromInstanceID(objToReturn.Value);
    }

    public GameObject Spawn(GameObject obj, Vector3 position, Quaternion rotation)
    {
        if (IsServer)
        {
            Debug.Log("Spawning on server");
            Debug.Log(obj.name);
            var newObj = Instantiate(obj, position, rotation);
            newObj.GetComponent<NetworkObject>().Spawn();
            return newObj;
        }

        ready.Value = false;
        SpawnServerRpc(obj.name, position, rotation);
        while (!ready.Value) { }
        Debug.Log(objToReturn.Value);
        return (GameObject)Helpers.FindObjectFromInstanceID(objToReturn.Value);
    }

    public GameObject Spawn(string obj, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (IsServer)
        {
            Debug.Log("Spawning on server");
            Debug.Log(obj);
            GameObject newObj = (GameObject)Instantiate(Resources.Load(obj), position, rotation);
            newObj.GetComponent<NetworkObject>().Spawn();
            return newObj;
        }

        ready.Value = false;
        SpawnServerRpc(obj, position, rotation);
        while (!ready.Value) { }
        return (GameObject)Helpers.FindObjectFromInstanceID(objToReturn.Value);
    }

    public GameObject Spawn(string obj, Vector3 position, Quaternion rotation)
    {
        if (IsServer)
        {
            Debug.Log("Spawning on server");
            Debug.Log(obj);
            GameObject newObj = (GameObject)Instantiate(Resources.Load(obj), position, rotation);
            Debug.Log(newObj);
            newObj.GetComponent<NetworkObject>().Spawn();
            return newObj;
        }

        ready.Value = false;
        SpawnServerRpc(obj, position, rotation);
        while (!ready.Value) { }
        return (GameObject)Helpers.FindObjectFromInstanceID(objToReturn.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnServerRpc(string objName, Vector3 position, Quaternion rotation, int parentId)
    {
        GameObject parent = (GameObject)Helpers.FindObjectFromInstanceID(parentId);
        var obj = Spawn(objName, position, rotation, parent.transform);
        SendIdClientRpc(obj.GetInstanceID());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnServerRpc(string objName, Vector3 position, Quaternion rotation)
    {
        Debug.Log(objName);
        var obj = Spawn(objName, position, rotation);
        Debug.Log(obj.GetInstanceID());
        SendIdClientRpc(obj.GetInstanceID());
    }

    [ClientRpc]
    public void SendIdClientRpc(int id)
    {
        objToReturn.Value = id;
        ready.Value = true;
        Debug.Log("ready");
    }
}
