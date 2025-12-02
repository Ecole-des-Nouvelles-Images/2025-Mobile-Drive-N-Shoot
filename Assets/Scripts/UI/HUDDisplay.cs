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
        [SerializeField] private TextMeshProUGUI _tmpDistance;
        [SerializeField] private Button _buttonItemIEM;
        [SerializeField] private Button _buttonItemNoOverheat;
        [SerializeField] private Button _buttonItemRepair;
        
        [Header("References Data")]
        [SerializeField] private TimerHandler _timerHandler;
        [SerializeField] private DistanceHandler _distanceHandler;

        // public void UseItem(string itemType)
        // {
        //     switch (itemType)
        //     {
        //         case ItemType.BigBlast:
        //             _buttonItemIEM.interactable = false;
        //             break;
        //         case ItemType.Overheat:
        //             _buttonItemNoOverheat.interactable = false;
        //             break;
        //         case ItemType.Repair:
        //             _buttonItemRepair.interactable = false;
        //             break;
        //     }
        //     EventBus.OnUsingItem?.Invoke(itemType);
        // }
        
        private void Update()
        {
            if (_timerHandler) _tmpTimer.text = _timerHandler.Timer.ToString("F1");
            if (_distanceHandler) _tmpDistance.text = _distanceHandler.Distance.ToString("F1");
        }

        private void OnEnable()
        {
            EventBus.OnCollectedItem += OnCollectedItem;
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
            EventBus.OnCollectedItem -= OnCollectedItem;
        }
    }
}