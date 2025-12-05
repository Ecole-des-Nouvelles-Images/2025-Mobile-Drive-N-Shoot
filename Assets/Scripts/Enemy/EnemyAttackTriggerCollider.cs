using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttackTriggerCollider : MonoBehaviour
    {
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private List<Collider> _colliders = new();

        private void OnTriggerEnter(Collider other)
        {
            _colliders.Add(other);

            if (other.CompareTag("Player"))
            {
                enemyData.CanAttack = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                enemyData.CanAttack = false;
            }
            
            _colliders.Remove(other);
            _colliders.RemoveAll(collider => collider == null);
        }
    }
}
