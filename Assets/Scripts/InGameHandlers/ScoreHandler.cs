using UI;
using UnityEngine;
using EventBus = Utils.Game.EventBus;

namespace InGameHandlers
{
    public class ScoreHandler : MonoBehaviour
    {
        [Header("References Display")]
        [SerializeField] private ScoreDisplay _panelScore;
        
        [Header("References Data")]
        [SerializeField] private DistanceHandler _distanceHandler;

        private int _checkpointPass;
        private int _spiderKills;
        private int _droneKills;

        private void OnEnable()
        {
            EventBus.OnGameOver += OnGameOver;
            EventBus.OnPlayerPassCheckpoint += OnPlayerPassCheckpoint;
            EventBus.OnSpiderIsKilled += OnSpiderIsKilled;
            EventBus.OnDroneIsKilled += OnDroneIsKilled;
        }
        
        private void OnDisable()
        {
            EventBus.OnGameOver -= OnGameOver;
            EventBus.OnPlayerPassCheckpoint -= OnPlayerPassCheckpoint;
            EventBus.OnSpiderIsKilled -= OnSpiderIsKilled;
            EventBus.OnDroneIsKilled -= OnDroneIsKilled;
        }
        
        private void OnPlayerPassCheckpoint()
        {
            _checkpointPass++;
        }

        private void OnSpiderIsKilled()
        {
            _spiderKills++;
        }
        
        private void OnDroneIsKilled()
        {
            _droneKills++;
        }
        
        private void OnGameOver()
        {
            _panelScore.gameObject.SetActive(true);
            _panelScore.Setup(_distanceHandler.Distance, _checkpointPass, _spiderKills, _droneKills);
        }
    }
}
