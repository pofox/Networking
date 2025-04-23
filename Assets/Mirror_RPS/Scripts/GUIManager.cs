using TMPro;
using UnityEngine;

namespace RPS
{
    public class GUIManager : MonoBehaviour
    {
        [SerializeField] TMP_InputField playerNameInput;

        public void onServerStarted()
        {
            NetworkingManeger.Instance.StartServer();
        }
        public void onClientStarted()
        {
            if (!string.IsNullOrEmpty(playerNameInput.text))
            {
                NetworkingManeger.Instance.updatePlayerName(playerNameInput.text);
                NetworkingManeger.Instance.StartClient();
            }
        }

        public void onHostStarted()
        {
            if (!string.IsNullOrEmpty(playerNameInput.text))
            {
                NetworkingManeger.Instance.updatePlayerName(playerNameInput.text);
                NetworkingManeger.Instance.StartHost();
            }
        }
    }
}
