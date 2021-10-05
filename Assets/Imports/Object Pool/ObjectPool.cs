#region Description
// -----------------------------------------------------------------------------------------
// An object pool for the provided game object that reuses objects that have already been instantiated
// Right click in project view and select Object Pool from menu to create
// Set prefab to the desired object to spawn, and optionally set the parent object for the instantiated prefabs
// If no parent is defined, one will be automatically generated in the form "{prefab.name}ObjectPool"
// -----------------------------------------------------------------------------------------
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

[CreateAssetMenu(fileName = "ObjectPool", menuName = "Object Pool")]
public class ObjectPool : ScriptableObject
{
    [Tooltip("Object for this pool")]
    [SerializeField]
    private GameObject prefab;

    [Tooltip("Object pool prefab for network syncing")]
    [SerializeField]
    private GameObject poolPrefab;

    [Tooltip("(Optional) Parent to place objects under, will create a new parent if none is defined")]
    [SerializeField]
    private Transform parent;

    [Tooltip("Currently instantiated objects")]
    [SerializeField]
    private List<GameObject> pool = new List<GameObject>();

    private Spawner spawner;

    /// <summary>
    /// Remove missing game objects when enabled
    /// </summary>
    public void OnEnable()
    {
        var objs = pool.FindAll(x => !x);

        foreach(GameObject obj in objs)
        {
            pool.Remove(obj);
        }
    }

    /// <summary>
    /// Spawn an object from this pool, returns spawned object
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if(!spawner)
        {
            spawner = FindObjectOfType<Spawner>();
        }

        // Find available object
        GameObject obj = pool.Find(x => !x.activeInHierarchy);
        
        // If no available object, instantiate new one
        if(!obj)
        {
            obj = InstantiateObject(position, rotation, parent);
        }
        // If available object found, set it to active and set its transform
        else
        {
            if (parent)
            {
                Debug.Log("here1");
                obj.transform.SetParent(parent);
            }
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
        }

        return obj;
    }

    /// <summary>
    /// Despawn an object in the pool for reusing
    /// </summary>
    /// <param name="obj"></param>
    public void Despawn(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(this.parent);
        obj.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        obj.transform.localRotation = Quaternion.EulerAngles(0.0f, 0.0f, 0.0f);
    }

    /// <summary>
    /// Initializes settings for object pool for programmatic object pool instantiation
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    public void Initialize(GameObject prefab, Transform parent=null)
    {
        this.prefab = prefab;
        this.parent = parent;
    }

    /// <summary>
    /// Instantiates a new object and adds it to the object pool
    /// </summary>
    private GameObject InstantiateObject(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        // If parent has not yet been defined, create a new parent object
        if (!this.parent)
        {
            Debug.Log("Here generating pool");
            this.parent = spawner.Spawn(poolPrefab, new Vector3(0.0f, 0.0f, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)).transform;
            this.parent.name = prefab.name + "ObjectPool";
            Debug.Log(this.parent.name);
        }

        GameObject obj;

        if(parent)
        {
            Debug.Log(parent.name);
            obj = Instantiate(prefab, parent);
            obj.transform.SetPositionAndRotation(position, rotation);
            pool.Add(obj);
            return obj;
        }

        // Instantiate object and add it to the list
        obj = Instantiate(prefab, this.parent);
        obj.transform.SetPositionAndRotation(position, rotation);
        pool.Add(obj);
        return obj;
    }
}
