using System.Collections.Generic;
using System.Linq;
using __Workspaces.Alex.Scripts;
using Core;
using UnityEngine;
using Utils.Game;

namespace InGameHandlers
{
    public class InventoryHandler : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private List<Item> _items;

        [ContextMenu("Use IEM")]
        public void UseIEM()
        {
            EventBus.OnUsingItem?.Invoke(ItemType.BigBlast);
        }
        
        [ContextMenu("Use Repair")]
        public void UseRepair()
        {
            EventBus.OnUsingItem?.Invoke(ItemType.Repair);
        }
        
        [ContextMenu("Use Overheat")]
        public void UseOverheat()
        {
            EventBus.OnUsingItem?.Invoke(ItemType.Overheat);
        }
        
        private void OnEnable()
        {
            EventBus.OnCollectedItem += OnCollectedItem;
            EventBus.OnUsingItem += OnUsingItem;
        }
        
        private void OnCollectedItem(Item item)
        {
            if (!_items.Contains(item)) _items.Add(item);
        }
        
        private void OnUsingItem(ItemType itemType)
        {
            Item item = _items.FirstOrDefault(e => e.ItemType == itemType);
            if (!item) return;
            
            item.Execute(GameManager.Instance.Player);
            _items.Remove(item);
        }
        
        private void OnDisable()
        {
            EventBus.OnCollectedItem -= OnCollectedItem;
            EventBus.OnUsingItem -= OnUsingItem;
        }

        public List<Item> GetInventory()
        {
            return _items;
        }
    }
}