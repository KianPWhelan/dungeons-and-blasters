using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Movement))]
public class Controller : MonoBehaviourPunCallbacks
{
    private Movement movement;
    private PlayerCamera playerCam;
    private Rotater rotater;
    private WeaponHolder weaponHolder;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

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
        movement = gameObject.GetComponent<Movement>();
        playerCam = gameObject.transform.GetComponentInChildren<PlayerCamera>();
        rotater = gameObject.transform.GetComponentInChildren<Rotater>();
        weaponHolder = gameObject.GetComponent<WeaponHolder>();

        if(!photonView.IsMine)
        {
            playerCam.gameObject.SetActive(false);
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
        Debug.Log("Game has ended");
        // TODO: Send to game over screen
        PhotonNetwork.LeaveRoom();
#if UNITY_EDITOR
        // UnityEditor.EditorApplication.isPlaying = false;
#endif
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

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Weapon button pressed");
            weaponHolder.UseWeapon(0);
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void ProcessMovement()
    {
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");
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
