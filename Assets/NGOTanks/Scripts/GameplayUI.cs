using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NGOTanks
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] Transform killPanel;
        [SerializeField] GameObject killPrefab;
        [SerializeField] GameObject WinPanel;
        [SerializeField] TextMeshProUGUI Wintxt;
        [SerializeField] GameObject HealIcon;
        [SerializeField] GameObject BombIcon;
        [SerializeField] Transform AbilityBar;
        float abilityTime = 0;
        float abilityCooldown;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }
        public void AddKill(string killtxt)
        {
            GameObject kill = Instantiate(killPrefab);
            kill.transform.SetParent(killPanel);
            kill.GetComponentInChildren<TextMeshProUGUI>().text = killtxt;
            LayoutRebuilder.ForceRebuildLayoutImmediate(killPanel.GetComponent<RectTransform>());
            Destroy(kill, 3.0f);
        }

        public void Win(string wintxt)
        {
            Wintxt.text = wintxt;
            WinPanel.transform.DOScale(1, 1).SetEase(Ease.OutElastic);
        }

        public void TeamAbility(pClass _pClass, float _abilityCoolDown)
        {
            abilityCooldown = _abilityCoolDown;
            if (_pClass == pClass.Tank) HealIcon.gameObject.SetActive(true);
            else BombIcon.gameObject.SetActive(true);
        }

        public void ResetAbility()
        {
            abilityTime = 0;
        }

        // Update is called once per frame
        void Update()
        {
            abilityTime += Time.deltaTime;
            AbilityBar.localScale = new Vector3(1, 1 - Mathf.Clamp01(abilityTime / abilityCooldown), 1);
        }
    }
}
