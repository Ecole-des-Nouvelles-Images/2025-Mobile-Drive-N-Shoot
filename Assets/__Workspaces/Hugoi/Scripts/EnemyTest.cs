using UnityEngine;
using Utils.Interfaces;

namespace __Workspaces.Hugoi.Scripts
{
    public class EnemyTest : MonoBehaviour, IDamageable, IEnemy
    {
        [SerializeField] private Transform aimTransform;
        public Vector3 GetAimPosition => aimTransform.position;
        
        public void TakeDamage(float damage)
        {
            Debug.Log("Damaged");
        }
    }
}
