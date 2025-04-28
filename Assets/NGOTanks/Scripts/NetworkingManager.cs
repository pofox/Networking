using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace NGOTanks
{
    public class NetworkingManager : NetworkManager
    {
        // Singleton instance
        public static NetworkingManager Instance { get; private set; }
        public const string SceneName_MainMenu = "MainMenu";
        public const string SceneName_Gameplay = "Gameplay";
        [SerializeField] NetworkObject PlayerPrefab;
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
        }


        private void NetMgr_ServerStarted()
        {
            //Load and sync online scene
            SceneManager.LoadScene(SceneName_Gameplay, LoadSceneMode.Single);
        }

        private void OnDestroy()
        {
            OnServerStarted -= NetMgr_ServerStarted;
        }
    }
}
