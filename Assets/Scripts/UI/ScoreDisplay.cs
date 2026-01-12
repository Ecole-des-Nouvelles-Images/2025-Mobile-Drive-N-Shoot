using System.Collections;
using __Workspaces.Hugoi.Scripts;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private Vector3 _scoreTextPos;
        [SerializeField] private Vector3 _distanceTextPos;
        [SerializeField] private Vector3 _checkpointTextPos;
        [SerializeField] private Vector3 _spiderTextPos;
        [SerializeField] private Vector3 _droneTextPos;

        [Header("References")]
        [SerializeField] private Transform _tmpGameOver;
        [SerializeField] private Transform _panelScore;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _bestScoreText;
        [SerializeField] private TextMeshProUGUI _distanceText;
        [SerializeField] private TextMeshProUGUI _checkpointText;
        [SerializeField] private TextMeshProUGUI _spiderKillsText;
        [SerializeField] private TextMeshProUGUI _droneKillsText;
        [SerializeField] private GameObject _addedGameObject;

        private int _currentDistance;
        private int _currentCheckpointPass;
        private int _currentSpiderKills;
        private int _currentDroneKills;
        
        private SaveGameData _saveGameData = new SaveGameData();

        public void Setup(float distance, int checkpointPass, int spiderKills, int droneKills)
        {
            StopAllCoroutines();
            StartCoroutine(GameOver((int)distance, checkpointPass, spiderKills, droneKills));

            _saveGameData = SaveSystem.GetSaveGameData();
            _bestScoreText.text = _saveGameData.BestScore.ToString();
            
            SaveSystem.SaveGameData((int)distance + checkpointPass + spiderKills + droneKills);
        }

        private IEnumerator GameOver(float distance, int checkpointPass, int spiderKills, int droneKills)
        {
            _tmpGameOver.DOScale(1f, 1f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
            
            yield return new WaitForSeconds(2f);
            _panelScore.DOScale(1f, 0.5f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.5f);
            StartCoroutine(AnimateAll((int)distance, checkpointPass, spiderKills, droneKills));
        }

        private IEnumerator AnimateAll(int distance, int checkpoint, int spiders, int drones)
        {
            yield return AnimateValueTimed(
                _currentDistance,
                distance,
                _animationDuration,
                _distanceText,
                v => _currentDistance = v
            );
            _addedGameObject.GetComponent<UIAddedTimer>().Setup(_distanceTextPos, _scoreTextPos, _currentDistance);
            _addedGameObject.SetActive(true);

            yield return AnimateValueTimed(
                _currentCheckpointPass,
                checkpoint,
                _animationDuration,
                _checkpointText,
                v => _currentCheckpointPass = v
            );
            _addedGameObject.GetComponent<UIAddedTimer>().Setup(_checkpointTextPos, _scoreTextPos, _currentCheckpointPass);
            _addedGameObject.SetActive(true);

            yield return AnimateValueTimed(
                _currentSpiderKills,
                spiders,
                _animationDuration,
                _spiderKillsText,
                v => _currentSpiderKills = v
            );
            _addedGameObject.GetComponent<UIAddedTimer>().Setup(_spiderTextPos, _scoreTextPos, _currentSpiderKills);
            _addedGameObject.SetActive(true);

            yield return AnimateValueTimed(
                _currentDroneKills,
                drones,
                _animationDuration,
                _droneKillsText,
                v => _currentDroneKills = v
            );
            _addedGameObject.GetComponent<UIAddedTimer>().Setup(_droneTextPos, _scoreTextPos, _currentDroneKills);
            _addedGameObject.SetActive(true);
            
            yield return new WaitForSeconds(_animationDuration);
            _saveGameData = SaveSystem.GetSaveGameData();
            _bestScoreText.text = _saveGameData.BestScore.ToString();
        }

        private IEnumerator AnimateValueTimed(
            int startValue,
            int targetValue,
            float duration,
            TextMeshProUGUI text,
            System.Action<int> onValueChanged
        )
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                int value = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, t));
                onValueChanged(value);
                text.text = value.ToString();

                yield return null;
            }

            onValueChanged(targetValue);
            text.text = targetValue.ToString();
            
            Invoke(nameof(UpdateScore), 0.6f);
        }
        
        private void UpdateScore()
        {
            int score = _currentDistance + _currentCheckpointPass + _currentSpiderKills + _currentDroneKills;
            _scoreText.text = score.ToString();
        }
    }
}