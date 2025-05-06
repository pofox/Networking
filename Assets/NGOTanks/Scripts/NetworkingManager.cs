using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace NGOTanks
{
    public class NetworkingManager : NetworkManager
    {
        // Singleton instance
        public static NetworkingManager Instance { get; private set; }
        public const string SceneName_MainMenu = "MainMenu";
        public const string SceneName_Gameplay = "Gameplay";
        [SerializeField] NetworkObject PlayerPrefab;
        Dictionary<ulong, NetworkPlayer> Players;

        public PlayerData localPlayerData;
        // Awake is called when the script instance is being loaded
        void Awake()
        {
            // Ensure only one instance of NetworkingManager exists
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            OnServerStarted += NetMgr_ServerStarted;
            Players = new Dictionary<ulong, NetworkPlayer>();
        }

        public void AddPlayer(NetworkPlayer player)
        {
            if (Players.ContainsKey(player.OwnerClientId)) return;
            Players.Add(player.OwnerClientId,player);
        }
        
        public NetworkPlayer GetPlayer(ulong playerId)
        {
            return Players[playerId];
        }
        
        public NetworkPlayer GetPlayerbyIndex(int index)
        {
            return Players.ToList()[index].Value;
        }

        public int GetPlayerCount()
        {
            return Players.Count;
        }

        public void RemovePlayer(ulong playerId)
        {
            if (Players.ContainsKey(playerId)) Players.Remove(playerId);
        }

        private void NetMgr_ServerStarted()
        {
            //Load and sync online scene
            SceneManager.LoadScene(SceneName_Gameplay, LoadSceneMode.Single);
        }

        public void NetMgr_ServerClosed()
        {
            Invoke("goToMainMenu", 5.0f);
        }

        private void goToMainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName_MainMenu, LoadSceneMode.Single);
        }

        public void updatePlayerData(PlayerData _playerData)
        {
            localPlayerData = _playerData;
        }

        private void OnDestroy()
        {
            OnServerStarted -= NetMgr_ServerStarted;
        }
    }
}
