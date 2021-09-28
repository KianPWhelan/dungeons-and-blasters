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

[CreateAssetMenu(fileName = "ObjectPool", menuName = "Object Pool")]
public class ObjectPool : ScriptableObject
{
    [Tooltip("Object for this pool")]
    [SerializeField]
    private GameObject prefab;

    [Tooltip("(Optional) Parent to place objects under, will create a new parent if none is defined")]
    [SerializeField]
    private Transform parent;

    [Tooltip("Currently instantiated objects")]
    [SerializeField]
    private List<GameObject> pool = new List<GameObject>();

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
    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        // Find available object
        GameObject obj = pool.Find(x => !x.activeInHierarchy);
        
        // If no available object, instantiate new one
        if(!obj)
        {
            obj = InstantiateObject(position, rotation);
        } 
        // If available object found, set it to active and set its transform
        else
        {
            obj.SetActive(true);
            obj.transform.SetPositionAndRotation(position, rotation);
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
    private GameObject InstantiateObject(Vector3 position, Quaternion rotation)
    {
        // If parent has not yet been defined, create a new parent object
        if (!parent)
        {
            parent = new GameObject(prefab.name + "ObjectPool").transform;
        }

        // Instantiate object and add it to the list
        GameObject obj = Instantiate(prefab, parent);
        obj.transform.SetPositionAndRotation(position, rotation);
        pool.Add(obj);
        return obj;
    }
}
