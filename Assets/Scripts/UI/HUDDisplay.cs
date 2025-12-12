using System;
using __Workspaces.Alex.Scripts;
using InGameHandlers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Game;

namespace UI
{
    public class HUDDisplay : MonoBehaviour
    {
        [Header("References Display")]
        [SerializeField] private TextMeshProUGUI _tmpTimer;
        [SerializeField] private GameObject _tmpTimerAdd;
        [SerializeField] private TextMeshProUGUI _tmpDistance;
        [SerializeField] private Button _buttonItemIEM;
        [SerializeField] private Button _buttonItemNoOverheat;
        [SerializeField] private Button _buttonItemRepair;
        [SerializeField] private Image _imageCarHealth;
        [SerializeField] private Image _imageBoost;
        
        [Header("References Data")]
        [SerializeField] private TimerHandler _timerHandler;
        [SerializeField] private DistanceHandler _distanceHandler;

        public void UseItem(string itemName)
        {
            Enum.TryParse(itemName, true, out ItemType itemType);
            
            switch (itemType)
            {
                case ItemType.BigBlast:
                    _buttonItemIEM.interactable = false;
                    break;
                case ItemType.Overheat:
                    _buttonItemNoOverheat.interactable = false;
                    break;
                case ItemType.Repair:
                    _buttonItemRepair.interactable = false;
                    break;
            }
            EventBus.OnUsingItem?.Invoke(itemType);
        }
        
        private void Update()
        {
            if (_timerHandler)
            {
                float time = _timerHandler.Timer;

                int seconds = Mathf.FloorToInt(time);
                int tenth = Mathf.FloorToInt((time - seconds) * 10f);

                _tmpTimer.text = $"{seconds}:{tenth}";
            }
            if (_distanceHandler) _tmpDistance.text = _distanceHandler.Distance.ToString();
        }

        private void OnEnable()
        {
            EventBus.OnAddTimeToTimer += AddTimeToTimer;
            EventBus.OnCollectedItem += OnCollectedItem;
            EventBus.OnPlayerHealthChange += OnPlayerHealthChange;
            EventBus.OnPlayerBoostCooldown += OnPlayerBoostCooldown;
        }
        
        private void AddTimeToTimer()
        {
            _tmpTimerAdd.SetActive(true);
        }

        private void OnPlayerHealthChange(float currentHealth, float maxHealth)
        {
            _imageCarHealth.fillAmount = currentHealth / maxHealth;
        }
        
        private void OnPlayerBoostCooldown(float timer, float cooldown)
        {
            _imageBoost.fillAmount = timer / cooldown;
        }

        private void OnCollectedItem(Item item)
        {
            switch (item.ItemType)
            {
                case ItemType.BigBlast:
                    _buttonItemIEM.interactable = true;
                    break;
                case ItemType.Overheat:
                    _buttonItemNoOverheat.interactable = true;
                    break;
                case ItemType.Repair:
                    _buttonItemRepair.interactable = true;
                    break;
            }
        }

        private void OnDisable()
        {
            EventBus.OnAddTimeToTimer -= AddTimeToTimer;
            EventBus.OnCollectedItem -= OnCollectedItem;
            EventBus.OnPlayerHealthChange -= OnPlayerHealthChange;
            EventBus.OnPlayerBoostCooldown -= OnPlayerBoostCooldown;
        }
    }
}