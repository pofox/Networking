using System.Collections.Generic;
using UnityEngine;

namespace NGOTanks
{
    public class Bomb : MonoBehaviour
    {
        int damage;
        float stayTime;
        Team team;
        List<NetworkPlayer> m_Players = new List<NetworkPlayer>();
        ulong killer;
        MeshRenderer meshRenderer;

        public void init(ulong _killer, Team _team, int _damage, float _stayTime, bool sameTeam)
        {
            meshRenderer.material.color = sameTeam ? Color.gray : Color.red;
            killer = _killer;
            team = _team;
            damage = _damage;
            stayTime = _stayTime;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        void Start()
        {
            Invoke("explode",stayTime);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void explode()
        {
            if (NetworkingManager.Instance.IsServer)
            {
                foreach (var player in m_Players) player.ApplyDamage(damage, killer);
            }
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<NetworkPlayer>(out NetworkPlayer player)) return;
            if (player.pData.teamId == team) return;
            if (m_Players.Contains(player)) return;
            m_Players.Add(player);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out NetworkPlayer player)) return;
            if (!m_Players.Contains(player)) return;
            m_Players.Remove(player);
        }
    }
}
