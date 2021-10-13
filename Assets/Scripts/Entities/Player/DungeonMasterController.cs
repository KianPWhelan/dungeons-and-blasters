using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class DungeonMasterController : MonoBehaviourPunCallbacks
{
    [Tooltip("Panning speed of the master")]
    [SerializeField]
    private FloatVariable panSpeed;

    [Tooltip("Zooming speed of the master")]
    [SerializeField]
    private FloatVariable zoomStep;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    public List<GameObject> enemyPrefabs;

    [SerializeField]
    private int currentSelection = 0;

    private Rigidbody body;

    private float startHeight;

    private Camera camera;

    private GameObject canvas;

    public Dictionary<GameObject, float> cooldowns = new Dictionary<GameObject, float>();

    public void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        canvas = gameObject.GetComponentInChildren<Canvas>().gameObject;

        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
            canvas.SetActive(false);
        }

        body = GetComponent<Rigidbody>();
        startHeight = transform.position.y;
        camera = gameObject.GetComponentInChildren<Camera>();

        var prefabs = Resources.LoadAll("", typeof(GameObject));
        // Debug.Log(prefabs.Length);

        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject p = (GameObject)prefabs[i];
            // Debug.Log(p.name);
            if(p.TryGetComponent(out EnemyGeneric c))
            {
                enemyPrefabs.Add(p);
                cooldowns.Add(p, -100000);
            }
        }

        var selectionText = canvas.transform.Find("Current Selection Text");
        selectionText.GetComponent<Text>().text = "Current Selection: " + enemyPrefabs[currentSelection].name;

#if UNITY_5_4_OR_NEWER
        // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
    }

    public void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInputs();
        }
    }

    public void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            ProcessMovement();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game has ended");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    /// <summary>
    /// Check for key presses from 
    /// </summary>
    private void ProcessInputs()
    {
        var scroll = Input.mouseScrollDelta.y;

        if(scroll < 0)
        {
            body.transform.position = body.transform.position + new Vector3(0f, zoomStep.runtimeValue, 0f);
        }

        else if(scroll > 0)
        {
            body.transform.position = body.transform.position + new Vector3(0f, -zoomStep.runtimeValue, 0f);
        }
        var change = panSpeed.initialValue + body.transform.position.y - startHeight;
        // Debug.Log(change);
        panSpeed.runtimeValue = change;

        if (Input.GetKeyDown(KeyCode.C))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = !Cursor.visible;
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            SpawnEnemy();
        }

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            DespawnEnemy();
        }

        if(Input.GetKey(KeyCode.E))
        {
            if(currentSelection < enemyPrefabs.Count - 1)
            {
                currentSelection++;
                var selectionText = canvas.transform.Find("Current Selection Text");
                selectionText.GetComponent<Text>().text = "Current Selection: " + enemyPrefabs[currentSelection].name;
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(currentSelection > 0)
            {
                currentSelection--;
                var selectionText = canvas.transform.Find("Current Selection Text");
                selectionText.GetComponent<Text>().text = "Current Selection: " + enemyPrefabs[currentSelection].name;
            }
        }
    }

    private void ProcessMovement()
    {
        var moveX = Input.GetAxis("Horizontal");
        var moveY = Input.GetAxis("Vertical");
        body.velocity = new Vector3(moveX * panSpeed.runtimeValue, 0f, moveY * panSpeed.runtimeValue);
    }

    private void SpawnEnemy()
    {
        GameObject enemyToSpawn = enemyPrefabs[currentSelection];
        float cooldown = enemyToSpawn.GetComponent<EnemyGeneric>().cooldown;

        if (cooldowns[enemyToSpawn] + cooldown > Time.time)
        {
            Debug.Log(enemyToSpawn.name + " is on cooldown for " + (Time.time - (cooldowns[enemyToSpawn] + cooldown)));
            return;
        }

        else
        {
            cooldowns[enemyToSpawn] = Time.time;
        }

        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000))
        {
            Debug.Log(hit.transform.name);
            Debug.Log("hit");
            PhotonNetwork.Instantiate(enemyToSpawn.name, hit.transform.position, hit.transform.rotation, 0);
        }
    }

    private void DespawnEnemy()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            Debug.Log(hit.transform.name);
            Debug.Log("hit");
            if(hit.collider.gameObject.TryGetComponent(out EnemyGeneric c))
            {
                PhotonNetwork.Destroy(hit.collider.gameObject);
            }
        }
    }

#if UNITY_5_4_OR_NEWER
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }

    /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
    void OnLevelWasLoaded(int level)
    {
        this.CalledOnLevelWasLoaded(level);
    }

    public override void OnDisable()
    {
        // Always call the base to remove callbacks
        base.OnDisable();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
#endif

    void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 50f))
        {
            transform.position = new Vector3(0f, 50f, 0f);
        }
    }
}
