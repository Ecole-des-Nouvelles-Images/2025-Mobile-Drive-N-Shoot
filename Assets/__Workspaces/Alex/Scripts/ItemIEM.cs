using FMODUnity;
using UnityEngine;
using Utils.Interfaces;

namespace __Workspaces.Alex.Scripts
{
    [CreateAssetMenu(fileName = "Item_IEM", menuName = "Items/Item IEM")]
    public class ItemIEM : Item
    {
        public float Radius = 30f;
        public float Damage = 100f;
        
        [Header("SFX")]
        [SerializeField] private EventReference _useSFX;

        public override void Execute(GameObject target)
        {
            Vector3 center = target.transform.position;

            // Visual Debug
            DebugDrawWireSphere(center, Radius, Color.cyan, 3f);

            // Find enemies in radius and apply damage
            var enemies = Physics.OverlapSphere(center, Radius);

            foreach (var enemy in enemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<IDamageable>()?.TakeDamage(Damage);
                    Debug.Log("Item_IEM damaged " + enemy.name);
                }
            }
            // Play SFX
            AudioManager.Instance.PlayAtPosition(_useSFX, center);
        }

        private void DebugDrawWireSphere(Vector3 center, float radius, Color color, float duration = 0f)
        {
            const int segments = 36;
            for (int i = 0; i < segments; i++)
            {
                float a1 = i * Mathf.PI * 2 / segments;
                float a2 = (i + 1) * Mathf.PI * 2 / segments;
                Vector3 p1 = center + new Vector3(Mathf.Cos(a1) * radius, 0, Mathf.Sin(a1) * radius);
                Vector3 p2 = center + new Vector3(Mathf.Cos(a2) * radius, 0, Mathf.Sin(a2) * radius);
                Debug.DrawLine(p1, p2, color, duration);
            }
        }
    }
}