using System.Collections.Generic;
using UnityEngine;

namespace __Workspaces.Alex.Scripts
{
    public class TurretAimDetector : MonoBehaviour
    {
        [Header("Enemies")]
        [SerializeField] private List<Collider> EnemiesInSight = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (!EnemiesInSight.Contains(other)) EnemiesInSight.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy")) EnemiesInSight.Remove(other);
            
            EnemiesInSight.RemoveAll(collider => !collider.enabled);
        }

        public Transform GetClosestEnemy(Vector3 from)
        {
            EnemiesInSight.RemoveAll(collider => !collider.enabled);
        
            Transform best = null;
            float bestDist = Mathf.Infinity;
            foreach (var enemy in EnemiesInSight)
            {
                float dist = Vector3.Distance(from, enemy.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = enemy.transform;
                }
            }

            return best;
        }
    }
}