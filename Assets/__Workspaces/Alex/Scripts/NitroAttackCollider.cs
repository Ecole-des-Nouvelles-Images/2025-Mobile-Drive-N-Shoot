using System;
using UnityEngine;
using Utils.Interfaces;

public class NitroAttackCollider : MonoBehaviour
{
    [SerializeField] private float damage = 500f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<IDamageable>().TakeDamage(damage);
        }
    }
}
