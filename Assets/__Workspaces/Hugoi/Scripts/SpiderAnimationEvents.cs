using UnityEngine;

namespace __Workspaces.Hugoi.Scripts
{
    public class SpiderAnimationEvents : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnemyData _enemyData;
        
        public void Attack()
        {
            _enemyData.Attack();
        }
    }
}