using UnityEngine;

namespace NGOTanks
{
    public class Bullet : MonoBehaviour
    {
        float speed = 30.0f;
        int Damage = 10;
        Rigidbody rb;
        ulong shooterid;
        Team teamId;

        public void init(ulong _shooterid, Team _teamId, int _damage, float _speed)
        {
            shooterid = _shooterid;
            teamId = _teamId;
            Damage = _damage;
            speed = _speed;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (TryGetComponent<Rigidbody>(out rb))
            {
                rb.linearVelocity = transform.forward * speed;
            }
            Destroy(gameObject, 3.0f);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Tank"))
            {
                if (other.TryGetComponent(out NetworkPlayer player))
                {
                    if (player.pData.teamId == teamId) return;
                    if (NetworkingManager.Instance.IsServer)
                    {
                        player.ApplyDamage(Damage,shooterid);
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
