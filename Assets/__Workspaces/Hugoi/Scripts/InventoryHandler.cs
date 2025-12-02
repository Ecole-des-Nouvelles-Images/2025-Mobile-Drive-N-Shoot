using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;
using Utils.Game;

namespace __Workspaces.Hugoi.Scripts
{
    public class InventoryHandler : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private List<Item> _items;
        
        private void OnEnable()
        {
            EventBus.OnCollectedItem += OnCollectedItem;
            EventBus.OnUsingItem += OnUsingItem;
        }

        private void OnDisable()
        {
            EventBus.OnCollectedItem -= OnCollectedItem;
            EventBus.OnUsingItem -= OnUsingItem;
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
    }
}