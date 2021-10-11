using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;


namespace Com.OfTomorrowInc.DMShooter
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        public Vector3 playerSpawnLocation;

        public GameObject dungeonMasterPrefab;

        public BoolVariable isDungeonMaster;

        public float gameTime;

        public FloatVariable time;

        private float masterTime;

        #region Photon Callbacks


        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Launcher");
        }


        #endregion


        #region Public Methods

        public void Start()
        {
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
                PhotonNetwork.Instantiate(this.playerPrefab.name, playerSpawnLocation, Quaternion.identity, 0);
            }
            else if (Controller.LocalPlayerInstance == null && DungeonMasterController.LocalPlayerInstance == null && isDungeonMaster.runtimeValue)
            {
                Debug.LogFormat("We are Instantiating DungeonMaster from {0}", Application.loadedLevelName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.dungeonMasterPrefab.name, new Vector3(0f, 50f, 0f), Quaternion.identity, 0);
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
        }

        [PunRPC]
        public void SetTimeAll(float time)
        {
            gameTime = time;
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


        #endregion
    }
}
