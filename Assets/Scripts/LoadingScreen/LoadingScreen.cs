using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LoadingScreen
{
    public class LoadingScreen : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private List<string> _sentences;
        [SerializeField] private TextMeshProUGUI _tmpSentence;

        private void Awake()
        {
            _tmpSentence.text = _sentences[Random.Range(0, _sentences.Count)];
        }
    }
}
