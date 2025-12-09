using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace __Workspaces.Alex.Scripts
{
    public enum ImpactType
    {
        Default,
        Enemy
    }
    public class ImpactPool : MonoBehaviour
    {
        public static ImpactPool Instance;
        [Header("Prefabs")]
        public ParticleSystem DefaultImpactPrefab;
        public ParticleSystem EnemyImpactPrefab;
        [Header("Pool Settings")]
        public int PoolSizeDefault = 20;

        public int PoolSizeEnemy = 10;
    
        private Queue<ParticleSystem> _defaultPool;
        private Queue<ParticleSystem> _enemyPool;

        private void Awake()
        {
            Instance = this;
            // Pool Creation
            _defaultPool = new Queue<ParticleSystem>();
            _enemyPool = new Queue<ParticleSystem>();
        
            for (int i = 0; i < PoolSizeDefault; i++)
                _defaultPool.Enqueue(Instantiate(DefaultImpactPrefab, transform));
            for (int i = 0; i < PoolSizeEnemy; i++)
                _enemyPool.Enqueue(Instantiate(EnemyImpactPrefab, transform));
        }

        private ParticleSystem CreateInstance(ParticleSystem prefab)
        {
            var ps = Instantiate(prefab, transform);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            return ps;
        }

        public void PlayImpact(Vector3 position, Vector3 normal, ImpactType type)
        {
            Queue<ParticleSystem> pool = type == ImpactType.Enemy ? _enemyPool : _defaultPool;
        
            //if Pool empty, create an extra
            if (pool.Count == 0)
            {
                var prefab = type == ImpactType.Enemy ? EnemyImpactPrefab : DefaultImpactPrefab;
                pool.Enqueue(CreateInstance(prefab));
            }
            var ps = pool.Dequeue();
        
            //Setup Position + Rotation
            ps.transform.position = position;
            ps.transform.rotation = Quaternion.LookRotation(normal);
            //Play
            ps.Play();
            // Get it back in the pool after its duration
            StartCoroutine(ReturnToPool(ps, type));
        }

        private IEnumerator ReturnToPool(ParticleSystem ps, ImpactType type)
        {
            yield return new WaitForSeconds(ps.main.duration);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (type == ImpactType.Enemy) _enemyPool.Enqueue(ps);
            else _defaultPool.Enqueue(ps);
        }
    }
}