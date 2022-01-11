using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class DungeonMasterControllerDepricated : MonoBehaviourPunCallbacks
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

    public GameObject abilityContent;

    public GameObject activeAbilitiesSlot;

    private GameObject abilityButton;

    private GameObject useAbilityButton;

    [SerializeField]
    private int currentSelection = 0;

    private Rigidbody body;

    private float startHeight;

    [HideInInspector]
    public Camera camera;

    public GameObject canvas;

    public GameObject topPanel;

    private bool placeMode = false;

    public Dictionary<GameObject, float> cooldowns = new Dictionary<GameObject, float>();

    private UseAbility currentAbility;

    private Color originalButtonColor;

    public Dictionary<GameObject, int> charges = new Dictionary<GameObject, int>();

    public void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;
        }

        camera = gameObject.GetComponentInChildren<Camera>();

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {

        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
            canvas.SetActive(false);
            topPanel.SetActive(false);
        }

        body = GetComponent<Rigidbody>();
        startHeight = transform.position.y;


        LoadEnemyPrefabs();
        abilityButton = (GameObject)Resources.Load("DM Ability Button");
        useAbilityButton = (GameObject)Resources.Load("DM Use Ability Button");
        GenerateAbilityButtons();

        var selectionText = canvas.transform.Find("Current Selection Text");
        selectionText.GetComponent<Text>().text = "Current Selection: ";

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

    public void SetCurrentSelection(GameObject selection, UseAbility ability)
    {
        Button button;

        if (currentAbility != null)
        {
            button = currentAbility.GetComponent<Button>();
            button.image.color = originalButtonColor;
        }

        currentSelection = enemyPrefabs.FindIndex(x => x == selection);
        var selectionText = canvas.transform.Find("Current Selection Text");
        selectionText.GetComponent<Text>().text = "Current Selection: " + enemyPrefabs[currentSelection].name;
        placeMode = true;
        currentAbility = ability;

        //colors = currentAbility.GetComponent<Button>().colors;
        button = currentAbility.GetComponent<Button>();
        originalButtonColor = button.image.color;
        button.image.color = Color.green;
    }

    private void LoadEnemyPrefabs()
    {
        var prefabs = Resources.LoadAll("", typeof(GameObject));
        // Debug.Log(prefabs.Length);

        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject p = (GameObject)prefabs[i];
            // Debug.Log(p.name);
            if (p.TryGetComponent(out EnemyGeneric c) && !c.doNotUseAsAbility)
            {
                enemyPrefabs.Add(p);
                cooldowns.Add(p, -100000);
                if (c.charges != -1)
                {
                    charges.Add(p, c.charges);
                }
            }
        }
    }

    private void GenerateAbilityButtons()
    {
        foreach (GameObject enemy in enemyPrefabs)
        {
            var data = enemy.GetComponent<EnemyGeneric>();
            var button = Instantiate(abilityButton, abilityContent.transform);
            button.GetComponentInChildren<Text>().text = enemy.name + "\nSlots: " + data.slotSize + " Cooldown: " + data.cooldown.ToString("0.00"); ;
            button.GetComponent<SlotAbility>().controller = this;
            button.GetComponent<SlotAbility>().ability = enemy;
            button.GetComponent<SlotAbility>().slotPanel = activeAbilitiesSlot;
        }
    }

    /// <summary>
    /// Check for key presses from 
    /// </summary>
    private void ProcessInputs()
    {
        var scroll = Input.mouseScrollDelta.y;

        if (scroll < 0)
        {
            body.transform.position = body.transform.position + new Vector3(0f, zoomStep.runtimeValue, 0f);
        }

        else if (scroll > 0)
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

        if (Input.GetKeyDown(KeyCode.Mouse0) && placeMode)
        {
            SpawnEnemy();
            var selectionText = canvas.transform.Find("Current Selection Text");
            selectionText.GetComponent<Text>().text = "Current Selection: ";
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            // DespawnEnemy();
            placeMode = false;

            if (currentAbility != null)
            {
                var button = currentAbility.GetComponent<Button>();
                button.image.color = originalButtonColor;
            }
        }

        if (Input.GetKey(KeyCode.E))
        {
            if (currentSelection < enemyPrefabs.Count - 1)
            {
                currentSelection++;
                var selectionText = canvas.transform.Find("Current Selection Text");
                selectionText.GetComponent<Text>().text = "Current Selection: " + enemyPrefabs[currentSelection].name;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentSelection > 0)
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
        //float cooldown = enemyToSpawn.GetComponent<EnemyGeneric>().cooldown;

        if (currentAbility.cooldown > 0)
        {
            Debug.Log(enemyToSpawn.name + " is on cooldown for " + currentAbility.cooldown);
            return;
        }

        else
        {
            //cooldowns[enemyToSpawn] = Time.time;
            //currentAbility.cooldown = currentAbility.cooldownTime;
        }

        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000))
        {
            Debug.Log(hit.transform.name);
            Debug.Log("hit");

            var closestPlayer = Helpers.FindClosestVisible(hit.transform, "Player");

            if (closestPlayer == null || Vector3.Distance(hit.transform.position, closestPlayer.transform.position) > 12f)
            {
                if (charges.ContainsKey(enemyToSpawn) && charges[enemyToSpawn] == 0)
                {
                    return;
                }

                PhotonNetwork.Instantiate(enemyToSpawn.name, hit.transform.position, hit.transform.rotation, 0);
                cooldowns[enemyToSpawn] = Time.time;
                currentAbility.cooldown = currentAbility.cooldownTime;

                if (charges.ContainsKey(enemyToSpawn))
                {
                    charges[enemyToSpawn]--;
                }
            }

            else
            {
                Debug.Log("Player too close");
            }
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
            if (hit.collider.gameObject.TryGetComponent(out EnemyGeneric c))
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