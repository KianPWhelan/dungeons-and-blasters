using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

namespace Com.OfTomorrowInc.DMShooter
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        public Vector3 playerSpawnLocation;

        public GameObject dungeonMasterPrefab;

        public BoolVariable isDungeonMaster;

        public Map mapGen;

        public float gameTime;

        public FloatVariable time;

        //public int numPlayers;

        //public int numFinished;

        private float masterTime;

        #region Photon Callbacks

        private PlayerCamera currentActiveCamera;

        private bool isDM;

        public static List<GameObject> players;


        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Launcher");
        }


        #endregion


        #region Public Methods

        public void Awake()
        {
            LoadPrefabData();
        }

        public void Start()
        {
            players = new List<GameObject>();

            if(PhotonNetwork.IsMasterClient)
            {
                Debug.Log("We are the master client");
                gameTime = 0;
            }

            Debug.Log("Is Dungeon Master?: " + isDungeonMaster.runtimeValue);
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else if(Controller.LocalPlayerInstance == null && DungeonMasterController.LocalPlayerInstance == null && !isDungeonMaster.runtimeValue)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                var newPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, playerSpawnLocation, Quaternion.identity, 0);
                players.Add(newPlayer);
            }
            else if (Controller.LocalPlayerInstance == null && DungeonMasterController.LocalPlayerInstance == null && isDungeonMaster.runtimeValue)
            {
                Debug.LogFormat("We are Instantiating DungeonMaster from {0}", Application.loadedLevelName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.dungeonMasterPrefab.name, new Vector3(0f, 50f, 0f), Quaternion.identity, 0);
            }

            if(DungeonMasterController.LocalPlayerInstance != null)
            {
                photonView.RPC("GenerateMap", RpcTarget.All, photonView.Owner.CustomProperties["map"]);
                photonView.RPC("SendPlayersToStartPoint", RpcTarget.All);
            }
        }

        public void Update()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                masterTime += Time.deltaTime;
                photonView.RPC("SetTimeAll", RpcTarget.All, masterTime);
            }

            time.runtimeValue = gameTime;

            if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.IsOpen)
            {
                return;
            }

            if(!isDungeonMaster.runtimeValue && Controller.LocalPlayerInstance == null && (currentActiveCamera == null || !currentActiveCamera.gameObject.activeSelf))
            {
                Debug.Log("Finding player to observe");
                var players = GameObject.FindGameObjectsWithTag("Player");
                bool playerIsPlaying = false;

                foreach(GameObject player in players)
                {
                    if(player.activeSelf)
                    {
                        Debug.Log("Player found, setting active camera");
                        var cam = player.GetComponent<Controller>().playerCam;
                        cam.gameObject.SetActive(true);
                        player.GetComponent<Controller>().canvas.SetActive(true);
                        currentActiveCamera = cam;
                        playerIsPlaying = true;
                    }
                }

                if(!playerIsPlaying)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    isDungeonMaster.runtimeValue = false;
                    Debug.Log("No players playing");
                    photonView.RPC("GlobalCloseRoom", RpcTarget.All);
                }
            }
        }

        [PunRPC]
        public void SetTimeAll(float time)
        {
            gameTime = time;
        }

        [PunRPC]
        public void GlobalCloseRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        [PunRPC]
        public void GenerateMap(string mapJson)
        {
            mapGen.LoadMapFromJson(mapJson);
            mapGen.BuildMap();
        }

        [PunRPC]
        public void SendPlayersToStartPoint()
        {
            if(Controller.LocalPlayerInstance == null)
            {
                return;
            }

            Transform spawnPoint = mapGen.GetSpawnPoint();

            if(spawnPoint != null)
            {
                Controller.LocalPlayerInstance.transform.position = spawnPoint.transform.position;
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #region Photon Callbacks


        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom()"); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom()"); // seen when other disconnects


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        #endregion


        #endregion

        #region Private Methods


        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : SampleScene");
            PhotonNetwork.LoadLevel("SampleScene");
        }

        private void LoadPrefabData()
        {
            PrefabLoader.LoadEnemyPrefabBalanceData("enemy_data.txt");
        }

        #endregion
    }
}
