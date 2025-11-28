using System;
using UnityEngine;

public class TestSpawnEnemy : MonoBehaviour
{
    public GameObject Enemy;
    private void Start()
    {
        Instantiate(Enemy, transform.position, Quaternion.identity);
    }
}
