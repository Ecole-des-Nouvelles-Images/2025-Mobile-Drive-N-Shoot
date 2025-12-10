using System.Collections;
using DG.Tweening;
using TMPro;
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
        [SerializeField] private Vector3 _speed;
        
        [Header("Canvas")]
        [SerializeField] private Image _canvasBackgroundBlack;
        [SerializeField] private Transform _textGo;
        [SerializeField] private GameObject _canvasOverlay;
        
        [Header("Animation GO")]
        [SerializeField] private float _endScaleValue;
        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _animationCurve;
        
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
                _textGo.DOScale(_endScaleValue, _duration).SetEase(_animationCurve).SetLoops(2, LoopType.Yoyo);
                Invoke(nameof(FadeOut), _duration * 1.2f);
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
                // _playerRigidBody.AddForce(_speed, ForceMode.Impulse);
                _player.transform.position += _speed * TimeManager.Instance.DeltaTime;
                yield return new WaitForSeconds(TimeManager.Instance.DeltaTime);
            }
            yield return new WaitForSeconds(time);
            _canvasOverlay.SetActive(true);
            _activeCinemachineCamera.gameObject.SetActive(false);
            
            EventBus.OnCinematicEnd?.Invoke();
        }

        private void FadeOut()
        {
            _textGo.GetComponent<TextMeshProUGUI>().DOFade(0.0f, _duration * 0.4f);
        }
    }
}
