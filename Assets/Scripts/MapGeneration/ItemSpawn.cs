using System.Collections.Generic;
using __Workspaces.Alex.Scripts;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using Utils.Game;
using Random = UnityEngine.Random;

namespace MapGeneration
{
    public class ItemSpawn : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private List<Item> _items = new();
        [SerializeField] private Sprite _iconIem;
        [SerializeField] private Sprite _iconOverheating;
        [SerializeField] private Sprite _iconRepair;
        
        [Header("Rotation")]
        [SerializeField] private float _rotationSpeed = 15f;
        [SerializeField] private float _positionOffset = 1f;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private AnimationCurve _animationCurve;
        
        [Header("References")]
        [SerializeField] private GameObject _itemVisual;
        [SerializeField] private EventReference _collectSFX;
        [SerializeField] private Image _image;
        
        private Item _selectedItem;

        private void Awake()
        {
            _selectedItem = _items[Random.Range(0, _items.Count)];

            if (_selectedItem.ItemType == ItemType.BigBlast)
            {
                _image.sprite = _iconIem;
            }
            else if (_selectedItem.ItemType == ItemType.Overheat)
            {
                _image.sprite = _iconOverheating;
            }
            else if (_selectedItem.ItemType == ItemType.Repair)
            {
                _image.sprite = _iconRepair;
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
