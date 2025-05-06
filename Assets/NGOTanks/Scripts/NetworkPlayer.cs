using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace NGOTanks
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField] Transform Canon;
        Rigidbody rb;

        [SerializeField] TextMeshProUGUI txt_PlayerName;
        [SerializeField] Slider healthSlider;
        [SerializeField] Transform hudroot;

        [SerializeField] Bullet BulletPrefab;
        [SerializeField] Transform Spawnpos;

        bool isDead = false;

        [SerializeField] MeshRenderer[] renderers;

        [SerializeField] Material Red;
        [SerializeField] Material Blue;
        [SerializeField] Material Green;
        [SerializeField] Material Yellow;

        GameplayUI UImanager;

        NetworkVariable<PlayerData> playerData = new NetworkVariable<PlayerData>();
        NetworkVariable<int> curruntHealth = new NetworkVariable<int>();

        [SerializeField] List<PlayerClassAttribute> playerClasses;
        PlayerClassAttribute playerClass;

        float abilityTime = 0f;
        bool win = false;

        public PlayerData pData => playerData.Value;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public override void OnNetworkSpawn()
        {
            UImanager = FindAnyObjectByType<GameplayUI>();
            base.OnNetworkSpawn();
            playerData.OnValueChanged += OnPlayerDataChanged;
            curruntHealth.OnValueChanged += OnHealthChanged;

            if (IsLocalPlayer)
            {
                SetPlayerDataServerRpc(NetworkingManager.Instance.localPlayerData);
            }
            else
            {
                SetPlayerProberties();
            }
            if (IsServer)
            {
                curruntHealth.Value = playerClass.maxHealth;
            }
            NetworkingManager.Instance.AddPlayer(this);
        }

        private void OnHealthChanged(int previousValue, int newValue)
        {
            healthSlider.value = 1.0f * curruntHealth.Value / playerClass.maxHealth;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsLocalPlayer && !isDead && !win && playerClass)
            {
                if (abilityTime < playerClass.cooldown) abilityTime += Time.deltaTime;
                // Get the input
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");
                bool right = Input.GetKey(KeyCode.RightArrow);
                bool left = Input.GetKey(KeyCode.LeftArrow);
                bool shoot = Input.GetKeyDown(KeyCode.Space);
                bool ability = (abilityTime >= playerClass.cooldown) && Input.GetKeyDown(KeyCode.Q);
                // Move the player
                Vector3 move = new Vector3(horizontal * playerClass.movingSpeed * Time.deltaTime, 0, vertical * playerClass.movingSpeed * Time.deltaTime);
                rb.MovePosition(rb.position + move);
                if (right || left)
                {
                    float rotationAngle = 0;
                    if (right)
                    {
                        rotationAngle += playerClass.canonSpeed * Time.deltaTime;
                    }
                    if (left)
                    {
                        rotationAngle -= playerClass.canonSpeed * Time.deltaTime;
                    }
                    Canon.Rotate(Vector3.up * rotationAngle);
                }
                if (shoot)
                {
                    ShootServerRPC(Spawnpos.position, Spawnpos.rotation);
                }
                if (ability)
                {
                    abilityTime = 0.0f;
                    UImanager.ResetAbility();
                    AbilityServerRPC(transform.position, transform.rotation);
                }
            }
            hudroot.LookAt(Camera.main.transform);
            hudroot.rotation = Quaternion.Euler(-hudroot.rotation.eulerAngles.x, 0.0f, hudroot.rotation.eulerAngles.z);
        }

        void SetPlayerProberties()
        {
            playerClass = playerClasses.Find(x => x.pClass == pData.p_classId);
            UImanager.TeamAbility(NetworkingManager.Instance.localPlayerData.p_classId, playerClass.cooldown);
            txt_PlayerName.spriteAsset = playerClass.icon;
            string color = (pData.p_classId == pClass.Tank) ? "#5555FFFF" : "#FF5555FF";
            txt_PlayerName.text = $"<sprite=0 color={color}> " + pData.PlayerName.ToString();
            Material mat = Yellow;
            switch (pData.teamId)
            {
                case Team.Red:
                    mat = Red;
                    break;
                case Team.Green:
                    mat = Green;
                    break;
                case Team.Blue:
                    mat = Blue;
                    break;
                case Team.Yellow:
                    mat = Yellow;
                    break;
            }
            foreach (var mesh in renderers)
            {
                mesh.material = mat;
            }
            //...
            OnHealthChanged(0, curruntHealth.Value);
        }

        #region Server RPCs
        [ServerRpc]
        public void SetPlayerDataServerRpc(PlayerData _playerData)
        {
            // Check if the name is valid
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Player name cannot be empty");
                return;
            }
            // Set the player name
            playerData.Value = _playerData;
        }
        [ServerRpc]
        void ShootServerRPC(Vector3 position, Quaternion rotation)
        {
            Bullet bullet = Instantiate(BulletPrefab, position, rotation);
            bullet.init(OwnerClientId, pData.teamId, playerClass.damage, playerClass.bulletSpeed);
            ShootClientRPC(position, rotation);
        }

        [ServerRpc]
        private void AbilityServerRPC(Vector3 position, Quaternion rotation)
        {
            GameObject ability = Instantiate(playerClass.abilityPrefab, position, rotation);
            if (playerClass.pClass == pClass.Dps)
            {
                ability.GetComponent<Bomb>().init(OwnerClientId, pData.teamId, playerClass.damage, playerClass.stayTime, pData.teamId == NetworkingManager.Instance.localPlayerData.teamId);
            }
            else
            {
                ability.GetComponent<HealZone>().init(OwnerClientId, pData.teamId, playerClass.damage, playerClass.effectTime, playerClass.stayTime, pData.teamId == NetworkingManager.Instance.localPlayerData.teamId);
            }
            AbilityClientRPC(position, rotation);
        }
        #endregion

        public void ApplyDamage(int damage, ulong killer)
        {
            if (!IsServer)
            {
                Debug.LogWarning("Apply Damage should only be called on the server");
                return;
            }
            if (isDead)
            {
                Debug.Log("el darb fe el mit haram");
                return;
            }
            //if (damage > 0) ...
            curruntHealth.Value = Mathf.Clamp(curruntHealth.Value-damage,0,playerClass.maxHealth);
            if (curruntHealth.Value == 0)
            {
                KillClientRPC(killer);
                NetworkPlayer killerplayer = NetworkingManager.Instance.GetPlayer(killer);
                int i = 0;
                win = true;
                while (win)
                {
                    if(i == NetworkingManager.Instance.GetPlayerCount()) break;
                    NetworkPlayer player = NetworkingManager.Instance.GetPlayerbyIndex(i);
                    if (!player.isDead)
                    {
                        if(player.pData.teamId != killerplayer.pData.teamId) win = false;
                    }
                    i++;
                }
                if (win) 
                {
                    gameendClientRpc(killerplayer.pData.teamId);
                }
            }
            if (curruntHealth.Value == playerClass.maxHealth)
            {
                //...
                Debug.Log("seb le gherak ya 3m");
            }
        }

        #region ClintRPCs
        [ClientRpc]
        void ShootClientRPC(Vector3 position, Quaternion rotation)
        {
            if (!NetworkingManager.Instance.IsHost)
            {
                Bullet bullet = Instantiate(BulletPrefab, position, rotation);
                bullet.init(OwnerClientId, pData.teamId, playerClass.damage, playerClass.bulletSpeed);
            }
        }

        [ClientRpc]
        private void AbilityClientRPC(Vector3 position, Quaternion rotation)
        {
            if (!NetworkingManager.Instance.IsHost)
            {
                GameObject ability = Instantiate(playerClass.abilityPrefab, position, rotation);
                if (playerClass.pClass == pClass.Dps)
                {
                    ability.GetComponent<Bomb>().init(OwnerClientId, pData.teamId, playerClass.damage, playerClass.stayTime, pData.teamId == NetworkingManager.Instance.localPlayerData.teamId);
                }
                else
                {
                    ability.GetComponent<HealZone>().init(OwnerClientId, pData.teamId, playerClass.damage, playerClass.effectTime, playerClass.stayTime, pData.teamId == NetworkingManager.Instance.localPlayerData.teamId);
                }
            }
        }

        [ClientRpc]
        void KillClientRPC(ulong killer)
        {
            isDead = true;
            string p1 = OwnerClientId == NetworkingManager.Instance.LocalClientId ? "you" : pData.PlayerName.ToString();
            string p2 = NetworkingManager.Instance.LocalClientId == NetworkingManager.Instance.GetPlayer(killer).OwnerClientId ? "you" : NetworkingManager.Instance.GetPlayer(killer).pData.PlayerName.ToString();
            Debug.Log($"<i><color={NetworkingManager.Instance.GetPlayer(killer).pData.teamId.ToString()}>{p2}</color></i> killed <i><color={pData.teamId.ToString()}>{p1}</color></i>");
            UImanager.AddKill($"<i><color={NetworkingManager.Instance.GetPlayer(killer).pData.teamId.ToString()}>{p2}</color></i> killed <i><color={pData.teamId.ToString()}>{p1}</color></i>");
        }

        [ClientRpc]
        void gameendClientRpc(Team team)
        {
            UImanager.Win($"Team <i><color={team.ToString()}>{team.ToString()}</color></i> Won!");
            Destroy(gameObject);
            NetworkManager.Shutdown();
            NetworkingManager.Instance.NetMgr_ServerClosed();
        }
        #endregion

        #region Hooks
        private void OnPlayerDataChanged(PlayerData previousValue, PlayerData newValue)
        {
            SetPlayerProberties();
        }
        #endregion

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            playerData.OnValueChanged -= OnPlayerDataChanged;
            curruntHealth.OnValueChanged -= OnHealthChanged;
            NetworkingManager.Instance.RemovePlayer(OwnerClientId);
        }
    }
}
