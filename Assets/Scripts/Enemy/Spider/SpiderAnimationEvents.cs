using UnityEngine;

namespace Enemy.Spider
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