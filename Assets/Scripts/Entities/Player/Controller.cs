using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Com.OfTomorrowInc.DMShooter;

[RequireComponent(typeof(Movement))]
public class Controller : MonoBehaviourPunCallbacks
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

    // private GameManager gameManager;

    public void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;
        }

        playerCam = gameObject.transform.GetComponentInChildren<PlayerCamera>();

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        movement = gameObject.GetComponent<Movement>();
        
        rotater = gameObject.transform.GetComponentInChildren<Rotater>();
        weaponHolder = gameObject.GetComponent<WeaponHolder>();
        // gameManager = FindObjectOfType<GameManager>();
        nametag.text = photonView.Owner.NickName;

        if(!photonView.IsMine)
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
        if(photonView.IsMine)
        {
            ProcessInputs();
        }

        RotateNametag();
    }

    public void FixedUpdate()
    {
        if(photonView.IsMine)
        {
            ProcessMovement();
        }
    }

    public void GameOver()
    {
        if (photonView.IsMine)
        {
            Debug.Log("Game has ended");
            // TODO: Send to game over screen
            // PhotonNetwork.LeaveRoom();
            PhotonNetwork.Destroy(gameObject);
#if UNITY_EDITOR
            // UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    private void RotateNametag()
    {
        // If this is not the local player, rotate its nametag towards the local player
        if (LocalPlayerInstance != null && !photonView.IsMine && !isDungeonMaster.runtimeValue)
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.Jump();   
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Weapon button pressed");
            weaponHolder.UseWeapon(0);
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
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }
    }
}
