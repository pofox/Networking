using System.Collections.Generic;
using UnityEngine;

namespace NGOTanks
{
    public class HealZone : MonoBehaviour
    {
        int deltaHealth;
        float healTime;
        float stayTime;
        Team team;
        List<NetworkPlayer> m_Players = new List<NetworkPlayer>();
        float time = 0.0f;
        ulong healer;
        MeshRenderer meshRenderer;

        public void init(ulong _healer,Team _team, int _deltaHealth, float _healTime, float _stayTime, bool sameTeam)
        {
            meshRenderer.material.color = sameTeam ? Color.green : Color.gray;
            healer = _healer;
            team = _team;
            deltaHealth = _deltaHealth;
            healTime = _healTime;
            stayTime = _stayTime;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        void Start()
        {
            Destroy(gameObject, stayTime);
        }

        // Update is called once per frame
        void Update()
        {
            if (!NetworkingManager.Instance.IsServer) return;
            time += Time.deltaTime;
            if (time < healTime) return;
            time = 0.0f;
            foreach (var player in m_Players) player.ApplyDamage(-deltaHealth, healer);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<NetworkPlayer>(out NetworkPlayer player)) return;
            if (player.pData.teamId != team) return;
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
