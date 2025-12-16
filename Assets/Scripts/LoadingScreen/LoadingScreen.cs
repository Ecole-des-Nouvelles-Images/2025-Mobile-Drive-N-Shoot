using System.Collections.Generic;
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
        
        [Header("References")]
        [SerializeField] private Image _imageBackground;
        [SerializeField] private TextMeshProUGUI _tmpSentence;

        private void OnEnable()
        {
            _imageBackground.sprite = _sprites[Random.Range(0, _sprites.Count)];
            _tmpSentence.text = _sentences[Random.Range(0, _sentences.Count)];
        }
    }
}