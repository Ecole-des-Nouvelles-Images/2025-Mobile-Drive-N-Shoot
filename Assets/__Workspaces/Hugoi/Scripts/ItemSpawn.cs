using System.Collections.Generic;
using UnityEngine;
using Utils.Game;
using Random = UnityEngine.Random;

namespace __Workspaces.Hugoi.Scripts
{
    public class ItemSpawn : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private List<Item> _items = new();
        
        [Header("References")]
        [SerializeField] private GameObject _itemVisual;
        
        private Item _selectedItem;

        private void Awake()
        {
            _selectedItem = _items[Random.Range(0, _items.Count)];
            Debug.Log(_selectedItem.ItemType);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EventBus.OnCollectedItem?.Invoke(_selectedItem);
                
                _itemVisual.SetActive(false);
                Destroy(gameObject, 2f);
            }
        }
    }
}
