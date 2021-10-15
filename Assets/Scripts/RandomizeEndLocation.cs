using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeEndLocation : MonoBehaviour
{
    [SerializeField]
    public List<Vector3> locations = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        int index = Random.Range(0, locations.Count);
        gameObject.transform.position = locations[index];
    }
}
