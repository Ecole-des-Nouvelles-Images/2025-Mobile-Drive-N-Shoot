using System;
using System.Collections.Generic;
using __Workspaces.Alex.Scripts;
using DG.Tweening;
using FMODUnity;
using InGameHandlers;
using UnityEngine;
using Utils.Game;
using Random = UnityEngine.Random;

namespace MapGeneration
{
    public class ItemSpawn : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private List<Item> _items = new();
        [SerializeField] private GameObject _goIem;
        [SerializeField] private GameObject _goOverheating;
        [SerializeField] private GameObject _goRepair;
        
        [Header("Rotation")]
        [SerializeField] private float _rotationSpeed = 15f;
        [SerializeField] private float _positionOffset = 1f;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private AnimationCurve _animationCurve;
        
        [Header("References")]
        [SerializeField] private GameObject _itemVisual;
        [SerializeField] private EventReference _collectSFX;
        
        private Item _selectedItem;
        private InventoryHandler _inventoryHandler;

        private void Awake()
        {
            List<Item> currentItems = _inventoryHandler.GetInventory();
            
            List<Item> selectedItems = new List<Item>();
            foreach (Item item in _items)
            {
                if (currentItems.Contains(item)) continue;
                selectedItems.Add(item);
            }
            
            _selectedItem = selectedItems[Random.Range(0, _items.Count)];

            if (_selectedItem.ItemType == ItemType.BigBlast)
            {
                _goIem.SetActive(true);
            }
            else if (_selectedItem.ItemType == ItemType.Overheat)
            {
                _goOverheating.SetActive(true);
            }
            else if (_selectedItem.ItemType == ItemType.Repair)
            {
                _goRepair.SetActive(true);
            }
            
            DOTween.To(
                () => transform.position.y,
                value =>
                {
                    var vector3 = transform.position;
                    vector3.y = value;
                    transform.position = vector3;
                },
                transform.position.y + _positionOffset,
                _duration
            ).SetEase(_animationCurve).SetLoops(-1, LoopType.Yoyo);
        }

        private void Start()
        {
            _inventoryHandler = FindObjectOfType<InventoryHandler>();
        }

        private void Update()
        {
            transform.Rotate(0, _rotationSpeed * TimeManager.Instance.DeltaTime, 0);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EventBus.OnCollectedItem?.Invoke(_selectedItem);
                _itemVisual.SetActive(false);
                // Play SFX
                AudioManager.Instance.PlayAtPosition(_collectSFX, transform.position);
                Destroy(gameObject, 2f);
            }
        }
    }
}
