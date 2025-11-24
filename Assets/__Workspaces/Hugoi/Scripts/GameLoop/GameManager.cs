using UnityEngine;
using Utils.Singletons;

namespace __Workspaces.Hugoi.Scripts.GameLoop
{
    public class GameManager : MonoBehaviourSingletonDontDestroyOnLoad<GameManager>
    {
        [Header("Player")]
        public Material[] CarMaterial;
        public Material[] TurretMaterial;
    }
}
