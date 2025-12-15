using __Workspaces.Alex.Scripts;
using UnityEngine;
using Utils.Game;

namespace Car
{
    public class IemActivation : MonoBehaviour
    {
        [Header("Particle System")]
        [SerializeField] private ParticleSystem _psIem;
        
        [Header("Material")]
        [SerializeField] private Material _material;

        private void OnEnable()
        {
            EventBus.OnCollectedItem += OnCollectedItem;
            EventBus.OnUsingItem += OnUsingItem;
        }
        
        private void OnCollectedItem(Item item)
        {
            if (item.ItemType == ItemType.BigBlast)
            {
                _material.SetFloat("_GlowStrength", 10f);
            }
        }
        
        private void OnUsingItem(ItemType itemType)
        {
            if (itemType == ItemType.BigBlast)
            {
                _material.SetFloat("_GlowStrength", 0f);
                _psIem.Play();
            }
        }
        
        private void OnDisable()
        {
            EventBus.OnCollectedItem -= OnCollectedItem;
            EventBus.OnUsingItem -= OnUsingItem;
        }
    }
}
