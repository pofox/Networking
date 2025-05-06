using TMPro;
using UnityEngine;

namespace NGOTanks
{
    [CreateAssetMenu]
    public class PlayerClassAttribute : ScriptableObject
    {
        public pClass pClass;
        public int maxHealth;
        public float movingSpeed;
        public float canonSpeed;
        public int damage;
        public float bulletSpeed;
        public TMP_SpriteAsset icon;
        [Header("Ability")]
        public GameObject abilityPrefab;
        public int abilityDamage;
        public float stayTime;
        public float effectTime;
        public float cooldown;
    }
}
