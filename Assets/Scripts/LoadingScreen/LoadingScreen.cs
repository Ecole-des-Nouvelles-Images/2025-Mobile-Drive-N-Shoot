using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace LoadingScreen
{
    public class LoadingScreen : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private List<Sprite> _sprites = new();
        [SerializeField] private List<string> _sentences = new();
        [SerializeField] private float _imagePickupDroneStartingPos;
        [SerializeField] private float _imagePickupDroneEndingPos;
        
        [Header("References")]
        [SerializeField] private Image _imageBackground;
        [SerializeField] private TextMeshProUGUI _tmpSentence;
        [SerializeField] private Image _imageSliderFill;
        [SerializeField] private RectTransform _imagePickupDrone;

        private bool _pickupDrone;

        private void OnEnable()
        {
            _imageBackground.sprite = _sprites[Random.Range(0, _sprites.Count)];
            _tmpSentence.text = _sentences[Random.Range(0, _sentences.Count)];

            var vector2 = _imagePickupDrone.anchoredPosition;
            vector2.x = _imagePickupDroneStartingPos;
            _imagePickupDrone.anchoredPosition = vector2;
        }

        private void Update()
        {
            if (_imageSliderFill.fillAmount < 1f)
            {
                var vector2 = _imagePickupDrone.anchoredPosition;
                vector2.x = Mathf.Lerp(_imagePickupDroneStartingPos, _imagePickupDroneEndingPos, _imageSliderFill.fillAmount);
                _imagePickupDrone.anchoredPosition = vector2;
            }
            else if (!_pickupDrone)
            {
                _imagePickupDrone.DOAnchorPos(new Vector2(2000f, 0f), 0.5f);
                _pickupDrone = true;
            }
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}