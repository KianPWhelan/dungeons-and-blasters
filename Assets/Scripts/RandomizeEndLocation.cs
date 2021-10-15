using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RandomizeEndLocation : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public List<Vector3> locations = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            int index = Random.Range(0, locations.Count);
            gameObject.transform.position = locations[index];
        }
    }
}
