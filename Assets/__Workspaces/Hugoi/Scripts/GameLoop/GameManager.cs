using UnityEngine;
using Utils.Singletons;

namespace __Workspaces.Hugoi.Scripts.GameLoop
{
    public class GameManager : MonoBehaviourSingletonDontDestroyOnLoad<GameManager>
    {
        [Header("Player Data")]
        public Material[] CurrentCarMaterials;
        public Material[] CurrentTurretMaterials;
    }
}