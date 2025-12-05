using System.Collections.Generic;
using UnityEngine;

namespace __Workspaces.Alex.Scripts
{
    public class TurretAimDetector : MonoBehaviour
    {
        public List<Transform> EnemiesInSight = new List<Transform>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                // other.transform.TransformPoint(other.transform.position);
                if (!EnemiesInSight.Contains(other.transform))
                    EnemiesInSight.Add(other.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
                EnemiesInSight.Remove(other.transform);
            EnemiesInSight.RemoveAll(transform => !transform);
        }

        public Transform GetClosestEnemy(Vector3 from)
        {
            EnemiesInSight.RemoveAll(transform => !transform);
        
            Transform best = null;
            float bestDist = Mathf.Infinity;
            foreach (var enemy in EnemiesInSight)
            {
                float dist = Vector3.Distance(from, enemy.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = enemy;
                }
            }

            return best;
        }
    }
}