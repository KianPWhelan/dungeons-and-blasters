using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Com.OfTomorrowInc.DMShooter;

[RequireComponent(typeof(Movement))]
public class Controller : NetworkBehaviour
{
    private Movement movement;
    public PlayerCamera playerCam;
    private Rotater rotater;
    private WeaponHolder weaponHolder;
    public GameObject canvas;
    public float sensitivity;
    public BoolVariable isDungeonMaster;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;
    public TextMesh nametag;

    private List<Weapon> weapons;

    private Weapon startingWeapon;

    private bool isStunned;

    private StatusEffects statusEffects;

    public GameEvent stopAttackingEvent;

    [HideInInspector]
    public float timeOfLastTp;

    // private GameManager gameManager;

    public void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (Object.HasInputAuthority)
        {
            LocalPlayerInstance = this.gameObject;
        }

        playerCam = gameObject.transform.GetComponentInChildren<PlayerCamera>();

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);

        weapons = new List<Weapon>(Resources.FindObjectsOfTypeAll<Weapon>());
        // TODO: weapon getting
        // startingWeapon = weapons.Find(x => x.name == (string)Object.Owner.CustomProperties["weapon"]);
    }

    public void Start()
    {
        movement = gameObject.GetComponent<Movement>();
        
        rotater = gameObject.transform.GetComponentInChildren<Rotater>();
        weaponHolder = gameObject.GetComponent<WeaponHolder>();
        weaponHolder.AddWeapon(startingWeapon, "Enemy");
        // gameManager = FindObjectOfType<GameManager>();
        // TODO: name
        // nametag.text = photonView.Owner.NickName;
        statusEffects = GetComponent<StatusEffects>();

        if(!Object.HasInputAuthority)
        {
            playerCam.gameObject.SetActive(false);
            canvas.SetActive(false);
        }

        else
        {
            nametag.gameObject.SetActive(false);
        }

        if(isDungeonMaster.runtimeValue)
        {
            nametag.transform.localScale *= 2;
        }

#if UNITY_5_4_OR_NEWER
        // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
    }

    public void Update()
    {
        isStunned = statusEffects.GetIsStunned();

        if(Object.HasInputAuthority)
        {
            ProcessInputs();
        }

        RotateNametag();
    }

    public void FixedUpdate()
    {
        if(Object.HasInputAuthority)
        {
            ProcessMovement();
        }
    }

    public void GameOver()
    {
        if (Object.HasInputAuthority)
        {
            Debug.Log("Game has ended");
            // TODO: Send to game over screen
            // PhotonNetwork.LeaveRoom();
            Runner.Despawn(Object);
#if UNITY_EDITOR
            // UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    private void RotateNametag()
    {
        // If this is not the local player, rotate its nametag towards the local player
        if (LocalPlayerInstance != null && !Object.HasInputAuthority && !isDungeonMaster.runtimeValue)
        {
            nametag.transform.LookAt(LocalPlayerInstance.transform.position);
            nametag.transform.Rotate(new Vector3(0, 180, 0));
        }

        else if(isDungeonMaster.runtimeValue)
        {
            nametag.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            nametag.transform.position = transform.position + Vector3.forward * 1.5f;
        }
    }

    /// <summary>
    /// Check for key presses from 
    /// </summary>
    private void ProcessInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isStunned)
        {
            movement.Jump();   
        }

        if (Input.GetKey(KeyCode.Mouse0) && !isStunned)
        {
            // Debug.Log("Weapon button pressed");
            weaponHolder.UseWeapon(0);
        }

        if(Input.GetKeyUp(KeyCode.Mouse0) || isStunned)
        {
            Debug.Log("Firing mouse up event");
            stopAttackingEvent.Raise();
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            // Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            sensitivity += 0.1f;
        }

        if(Input.GetKeyDown(KeyCode.N) && sensitivity > 0)
        {
            sensitivity -= 0.1f;
        }
    }

    private void ProcessMovement()
    {
        if(isStunned)
        {
            return;
        }

        var mouseX = Input.GetAxis("Mouse X") * sensitivity;
        var mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        movement.Rotate(mouseX, 0.0f);
        rotater.Rotate(mouseX, mouseY);
        var moveX = Input.GetAxis("Horizontal");
        var moveY = Input.GetAxis("Vertical");
        movement.Move(moveX, moveY);
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

    public void OnDisable()
    {
        // Always call the base to remove callbacks
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
#endif

    void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }
    }
}
