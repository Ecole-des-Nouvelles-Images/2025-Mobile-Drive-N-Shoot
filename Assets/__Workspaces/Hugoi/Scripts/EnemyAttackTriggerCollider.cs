using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace __Workspaces.Hugoi.Scripts
{
    public class EnemyAttackTriggerCollider : MonoBehaviour
    {
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private List<Collider> _colliders = new List<Collider>();

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
