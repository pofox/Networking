using TMPro;
using UnityEngine;

namespace NGOTanks
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField playerNameInput;

        public void onServerStarted()
        {
            NetworkingManager.Instance.StartServer();
        }
        public void onClientStarted()
        {
            if (!string.IsNullOrEmpty(playerNameInput.text))
            {
                //NetworkingManager.Instance.updatePlayerName(playerNameInput.text);
                NetworkingManager.Instance.StartClient();
            }
        }

        public void onHostStarted()
        {
            if (!string.IsNullOrEmpty(playerNameInput.text))
            {
                //NetworkingManeger.Instance.updatePlayerName(playerNameInput.text);
                NetworkingManager.Instance.StartHost();
            }
        }
    }
}
