using Unity.Cinemachine;
using UnityEngine;
using Utils.Game;

namespace Camera
{
    public class CameraShake : MonoBehaviour
    {
        [Header("Shake Settings")]
        [SerializeField] private CinemachineImpulseSource _bigExplosion;
        [SerializeField] private CinemachineImpulseSource _smallExplosion;

        private void OnEnable()
        {
            EventBus.OnBigExplosion += OnBigExplosion;
            EventBus.OnSmallExplosion += OnSmallExplosion;

        }
        
        private void OnDisable()
        {
            EventBus.OnBigExplosion -= OnBigExplosion;
            EventBus.OnSmallExplosion -= OnSmallExplosion;
            
        }

        private void OnBigExplosion()
        {
            _bigExplosion.GenerateImpulse();
        }
        
        private void OnSmallExplosion()
        {
            _smallExplosion.GenerateImpulse();
        }
    }
}
