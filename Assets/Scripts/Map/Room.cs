using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Room : MonoBehaviour
{
    [Tooltip("Order is North South East West")]
    public GameObject[] doorPoints = new GameObject[4];

    public bool isStartingRoom;

    public GameObject spawnPoint;

    [Tooltip("Slot options for enemies. Do not preset the enemy game object")]
    public List<SlotOption> slotOptions = new List<SlotOption>();

    [HideInInspector]
    public SlotOption selection;

    [HideInInspector]
    public Vector2Int gridSlot;

    private bool hasActivated;

    [System.Serializable]
    public class EnemySlot
    {
        public SizeClasses size;
        public Vector3 location;

        [HideInInspector]
        public GameObject enemy;
    }

    [System.Serializable]
    public class SlotOption
    {
        public List<EnemySlot> slots = new List<EnemySlot>();

        public Dictionary<SizeClasses, int> GetNumberOfSlotsPerSize()
        {
            Dictionary<SizeClasses, int> res = new Dictionary<SizeClasses, int>();

            foreach(EnemySlot e in slots)
            {
                if(!res.ContainsKey(e.size))
                {
                    res.Add(e.size, 1);
                }

                else
                {
                    res[e.size]++;
                }
            }

            return res;
        }
    }

    public void PlaceEnemyInSlot(GameObject enemy, int slotIndex)
    {
        if(selection == null || selection.slots.Count <= 0)
        {
            return;
        }

        var enemySize = enemy.GetComponent<EnemyGeneric>().size;

        if (enemySize != selection.slots[slotIndex].size)
        {
            Debug.LogWarning("Cannot slot enemy of size " + enemySize + " into slot of size " + selection.slots[slotIndex].size);
            return;
        }

        selection.slots[slotIndex].enemy = enemy;
    }

    public void ActivateRoom()
    {
        if(hasActivated)
        {
            return;
        }

        Debug.Log("Activating " + name);
        hasActivated = true;

        foreach(GameObject door in doorPoints)
        {
            if(door != null)
            {
                door.SetActive(false);
            }
        }

        StartCoroutine(StartSpawnSequence());
    }

    //public void SendActivationCommand()
    //{
    //    photonView.RPC("ActivationCommand", RpcTarget.All);
    //}

    //[PunRPC]
    //public void ActivationCommand()
    //{
    //    ActivateRoom();
    //}

    private void SpawnAll()
    {
        if (DungeonMasterController.LocalPlayerInstance == null)
        {
            Debug.Log("Not DM");
            return;
        }

        Debug.Log("DM");

        foreach (EnemySlot slot in selection.slots)
        {
            if(slot.enemy != null)
            {
                Debug.Log("Spawning");
                PhotonNetwork.Instantiate(slot.enemy.name, transform.position + slot.location, Quaternion.identity);
            }
        }
    }

    private IEnumerator StartSpawnSequence()
    {
        yield return new WaitForSeconds(GlobalConstants.roomSpawnEnemiesDelay);
        SpawnAll();
    }
}
