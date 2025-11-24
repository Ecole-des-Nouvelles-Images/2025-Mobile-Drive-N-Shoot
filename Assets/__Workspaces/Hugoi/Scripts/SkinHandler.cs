using System.Collections.Generic;
using __Workspaces.Hugoi.Scripts.GameLoop;
using UnityEngine;
using UnityEngine.UI;

namespace __Workspaces.Hugoi.Scripts
{
    public class SkinHandler : MonoBehaviour
    {
        [Header("Skin")]
        [SerializeField] private List<Skin> _pickupSkins =  new();
        [SerializeField] private List<Skin> _turretSkins =  new();
        
        [Header("Visual")]
        [SerializeField] private MeshRenderer _pickupMeshRenderer;
        [SerializeField] private List<MeshRenderer> _turretMeshRenderers = new();

        [Header("Buttons")]
        [SerializeField] private Button _leftArrow;
        [SerializeField] private Button _rightArrow;

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
            _leftArrow.onClick.AddListener(PreviousSkin);
            _rightArrow.onClick.AddListener(NextSkin);
        }
        
        private void OnDisable()
        {
            _leftArrow.onClick.RemoveListener(PreviousSkin);
            _rightArrow.onClick.RemoveListener(NextSkin);
        }

        public void SetSkinType(string skinType)
        {
            if (skinType == "Pickup")
            {
                _currentSkinType = SkinType.Pickup;
            }
            else
            {
                _currentSkinType = SkinType.Turret;
            }
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
                _pickupMeshRenderer.materials = materials;

                if (GameManager.Instance) GameManager.Instance.CarMaterial = materials;
            }

            if (skinType == SkinType.Turret)
            {
                Material[] materials = {
                    _turretSkins[_currentTurretSkinId].Material
                };
                
                foreach (var meshRenderers in _turretMeshRenderers)
                {
                    meshRenderers.materials = materials;
                }
                
                if (GameManager.Instance) GameManager.Instance.TurretMaterial = materials;
            }
        }
    }
}
