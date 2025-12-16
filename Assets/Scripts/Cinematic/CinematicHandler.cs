using System.Collections;
using Car;
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
        [SerializeField] private float _speed;
        
        [Header("Canvas")]
        [SerializeField] private Image _canvasBackgroundBlack;
        [SerializeField] private Transform _textGo;
        [SerializeField] private Transform _textReady;
        [SerializeField] private GameObject _canvasOverlay;
        [SerializeField] private GameObject _panelTuto;
        
        [Header("Animation GO")]
        [SerializeField] private float _endScaleGo;
        [SerializeField] private float _durationGo;
        [SerializeField] private AnimationCurve _animationCurveGo;
        
        [Header("Animation READY")]
        [SerializeField] private float _endScaleReady;
        [SerializeField] private AnimationCurve _animationCurveReady;
        
        [Header("Cinemachine Camera")]
        [SerializeField] private CinemachineBrain _cinemachineBrain;
        [SerializeField] private CinemachineCamera _activeCinemachineCamera;
        [SerializeField] private CinemachineCamera _nextCinemachineCamera;
        
        private WheelControl[] _wheelsRb;
        private CarControler _carControler;
        private bool _playerDetected;

        private void Start()
        {
            _wheelsRb = _player.GetComponentsInChildren<WheelControl>();
            _carControler = _player.GetComponentInChildren<CarControler>();
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
                _textGo.DOScale(_endScaleGo, _durationGo).SetEase(_animationCurveGo).SetLoops(2, LoopType.Yoyo);
                Invoke(nameof(FadeOut), _durationGo * 1.2f);
            }
        }

        private void SwitchCinemachineCamera(CinemachineCamera activeCam, CinemachineCamera nextCam)
        {
            activeCam.Priority = 0;
            nextCam.Priority = 1;
        }
        
        private IEnumerator WaitForBlendEnd(float time)
        {
            _carControler.enabled = false;
            _textReady.DOScale(_endScaleReady, time / 2f).SetEase(_animationCurveReady).SetLoops(2, LoopType.Yoyo);
            while (!_playerDetected)
            {
                foreach (var wheel in _wheelsRb)
                {
                    if (wheel.motorized)
                    {
                        wheel.WheelCollider.motorTorque = _speed;
                    }
                    wheel.WheelCollider.brakeTorque = 0f;
                }

                yield return null;
            }
            yield return new WaitForSeconds(time);
            _carControler.enabled = true;
            _canvasOverlay.SetActive(true);
            _panelTuto.SetActive(true);
            _activeCinemachineCamera.gameObject.SetActive(false);
            
            EventBus.OnCinematicEnd?.Invoke();
            StartCoroutine(DisableGo(_panelTuto, 5f));
        }

        private void FadeOut()
        {
            _textGo.GetComponent<TextMeshProUGUI>().DOFade(0.0f, _durationGo * 0.4f);
        }

        private IEnumerator DisableGo(GameObject go, float duration)
        {
            yield return new WaitForSeconds(duration);
            go.SetActive(false);
        }
    }
}