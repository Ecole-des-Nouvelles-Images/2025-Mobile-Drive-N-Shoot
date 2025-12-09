using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace SelectionSkins
{
    public class SelectionSkinHandler : MonoBehaviour
    {
        [Header("Skin")]
        [SerializeField] private List<Skin> _pickupSkins =  new();
        [SerializeField] private List<Skin> _turretSkins =  new();
        
        [Header("Visual")]
        [SerializeField] private List<MeshRenderer> _pickupMeshRenderers = new();
        [SerializeField] private List<MeshRenderer> _turretMeshRenderers = new();

        [Header("Buttons")]
        [SerializeField] private Button _pickupButton;
        [SerializeField] private Button _turretButton;
        [SerializeField] private Button _leftArrowButton;
        [SerializeField] private Button _rightArrowButton;

        private SkinType _currentSkinType;
        private int _currentPickupSkinId;
        private int _currentTurretSkinId;

        private void Awake()
        {
            ApplySkin(SkinType.Pickup);
            ApplySkin(SkinType.Turret);
        }

        private void OnEnable()
        {
            _pickupButton.onClick.AddListener(() => SetSkinType(SkinType.Pickup));
            _turretButton.onClick.AddListener(() => SetSkinType(SkinType.Turret));
            _leftArrowButton.onClick.AddListener(PreviousSkin);
            _rightArrowButton.onClick.AddListener(NextSkin);
        }
        
        private void OnDisable()
        {
            _pickupButton.onClick.RemoveListener(() => SetSkinType(SkinType.Pickup));
            _turretButton.onClick.RemoveListener(() => SetSkinType(SkinType.Turret));
            _leftArrowButton.onClick.RemoveListener(PreviousSkin);
            _rightArrowButton.onClick.RemoveListener(NextSkin);
        }

        private void SetSkinType(SkinType skinType)
        {
            _currentSkinType = skinType;
        }

        private void PreviousSkin()
        {
            if (_currentSkinType == SkinType.Pickup)
            {
                if (_currentPickupSkinId - 1 < 0)
                {
                    _currentPickupSkinId = _pickupSkins.Count - 1;
                }
                else
                {
                    _currentPickupSkinId--;
                }
                
                ApplySkin(SkinType.Pickup);
            }
            
            if (_currentSkinType == SkinType.Turret)
            {
                if (_currentTurretSkinId - 1 < 0)
                {
                    _currentTurretSkinId = _turretSkins.Count - 1;
                }
                else
                {
                    _currentTurretSkinId--;
                }
                
                ApplySkin(SkinType.Turret);
            }
        }

        private void NextSkin()
        {
            if (_currentSkinType == SkinType.Pickup)
            {
                if (_currentPickupSkinId + 1 >= _pickupSkins.Count)
                {
                    _currentPickupSkinId = 0;
                }
                else
                {
                    _currentPickupSkinId++;
                }
                
                ApplySkin(SkinType.Pickup);
            }
            
            if (_currentSkinType == SkinType.Turret)
            {
                if (_currentTurretSkinId + 1 >= _turretSkins.Count)
                {
                    _currentTurretSkinId = 0;
                }
                else
                {
                    _currentTurretSkinId++;
                }
                
                ApplySkin(SkinType.Turret);
            }
        }

        private void ApplySkin(SkinType skinType)
        {
            if (skinType == SkinType.Pickup)
            {
                Material[] materials = {
                    _pickupSkins[_currentPickupSkinId].Material
                };

                foreach (var meshRenderer in _pickupMeshRenderers)
                {
                    meshRenderer.materials = materials;
                }

                if (GameManager.Instance) GameManager.Instance.CurrentCarMaterials = materials;
            }

            if (skinType == SkinType.Turret)
            {
                Material[] materials = {
                    _turretSkins[_currentTurretSkinId].Material
                };
                
                foreach (var meshRenderer in _turretMeshRenderers)
                {
                    meshRenderer.materials = materials;
                }
                
                if (GameManager.Instance) GameManager.Instance.CurrentTurretMaterials = materials;
            }
        }
    }
}
