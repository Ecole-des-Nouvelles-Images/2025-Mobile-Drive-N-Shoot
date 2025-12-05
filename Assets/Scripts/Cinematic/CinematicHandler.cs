using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Utils.Game;

namespace Cinematic
{
    public class CinematicHandler : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private GameObject _player;
        [SerializeField] private Vector3 _force;
        
        [Header("Canvas")]
        [SerializeField] private Image _canvasBackgroundBlack;
        [SerializeField] private GameObject _canvasOverlay;
        
        [Header("Cinemachine Camera")]
        [SerializeField] private CinemachineBrain _cinemachineBrain;
        [SerializeField] private CinemachineCamera _activeCinemachineCamera;
        [SerializeField] private CinemachineCamera _nextCinemachineCamera;
        
        private Rigidbody _playerRigidBody;
        private bool _playerDetected;

        private void Awake()
        {
            _playerRigidBody = _player.GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            EventBus.OnModuleFinishedGeneration += OnModuleFinishedGeneration;
        }
        
        private void OnDisable()
        {
            EventBus.OnModuleFinishedGeneration -= OnModuleFinishedGeneration;
        }

        private void OnModuleFinishedGeneration()
        {
            if (_playerDetected) return;
            
            _canvasBackgroundBlack.DOFade(0.0f, 0.5f);
            StartCoroutine(WaitForBlendEnd(2f));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SwitchCinemachineCamera(_activeCinemachineCamera, _nextCinemachineCamera);
                _playerDetected = true;
            }
        }

        private void SwitchCinemachineCamera(CinemachineCamera activeCam, CinemachineCamera nextCam)
        {
            activeCam.Priority = 0;
            nextCam.Priority = 1;
        }
        
        private IEnumerator WaitForBlendEnd(float time)
        {
            while (!_playerDetected)
            {
                _playerRigidBody.AddForce(_force, ForceMode.Impulse);
                yield return new WaitForSeconds(time / 10f);
            }
            yield return new WaitForSeconds(time);
            _canvasOverlay.SetActive(true);
        }
    }
}
