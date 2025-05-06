using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NGOTanks
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] Transform[] SpawnPositions;
        int currentSpawnIndex;
        [SerializeField] NetworkObject PlayerPrefab;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            currentSpawnIndex = 0;
            NetworkingManager.Instance.SceneManager.OnLoadComplete += NetSceneMgr_LoadCompleted;
            if (NetworkingManager.Instance.IsHost)
            {
                SpawnNextPlayer(NetworkingManager.Instance.LocalClientId);
            }
        }

        private void NetSceneMgr_LoadCompleted(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (sceneName == NetworkingManager.SceneName_Gameplay)
            {
                SpawnNextPlayer(clientId);
            }
        }

        void SpawnNextPlayer(ulong clientId)
        {
            if (!NetworkingManager.Instance.IsServer) return;
            // Get the next spawn position
            NetworkObject netPlayer = Instantiate(PlayerPrefab, SpawnPositions[currentSpawnIndex]);
            netPlayer.transform.localScale = Vector3.one;
            netPlayer.SpawnAsPlayerObject(clientId);

            currentSpawnIndex++;
            currentSpawnIndex %= SpawnPositions.Length;
        }

        private void OnDestroy()
        {
            if (NetworkingManager.Instance)
            {
                if(NetworkingManager.Instance.SceneManager != null)
                {
                    NetworkingManager.Instance.SceneManager.OnLoadComplete -= NetSceneMgr_LoadCompleted;
                }
            }
        }
    }
}
