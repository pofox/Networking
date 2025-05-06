using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NGOTanks
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField playerNameInput;
        [SerializeField] TMP_Dropdown DD_Team;
        [SerializeField] TMP_Dropdown DD_class;

        private void Start()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            List<TMP_Dropdown.OptionData> team_options = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> class_options = new List<TMP_Dropdown.OptionData>();
            string[] team_names = System.Enum.GetNames(typeof(Team));
            string[] class_names = System.Enum.GetNames(typeof(pClass));
            foreach (string team_name in team_names)
            {
                team_options.Add(new TMP_Dropdown.OptionData { text = team_name});
            }
            foreach (string class_name in class_names)
            {
                class_options.Add(new TMP_Dropdown.OptionData { text = class_name });
            }
            DD_Team.options = team_options;
            DD_class.options = class_options;
        }

        public void onServerStarted()
        {
            NetworkingManager.Instance.StartServer();
        }
        public void onClientStarted()
        {
            if (!string.IsNullOrEmpty(playerNameInput.text))
            {
                GetPlayerData();
                NetworkingManager.Instance.StartClient();
            }
        }

        public void onHostStarted()
        {
            if (!string.IsNullOrEmpty(playerNameInput.text))
            {
                GetPlayerData();
                NetworkingManager.Instance.StartHost();
            }
        }

        void GetPlayerData()
        {
            PlayerData playerData = new PlayerData();
            playerData.PlayerName = playerNameInput.text;
            playerData.teamId = (Team)Enum.Parse(typeof(Team), DD_Team.captionText.text);
            playerData.p_classId = (pClass)Enum.Parse(typeof(pClass), DD_class.captionText.text);
            NetworkingManager.Instance.updatePlayerData(playerData);
        }
    }
}
