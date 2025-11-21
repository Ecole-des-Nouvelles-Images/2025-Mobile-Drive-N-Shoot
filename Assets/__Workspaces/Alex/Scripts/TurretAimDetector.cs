using System;
using System.Collections.Generic;
using UnityEngine;

public class TurretAimDetector : MonoBehaviour
{
    public List<Transform> EnemiesInSight = new List<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!EnemiesInSight.Contains(other.transform))
                EnemiesInSight.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
            EnemiesInSight.Remove(other.transform);
    }

    public Transform GetClosestEnemy(Vector3 from)
    {
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