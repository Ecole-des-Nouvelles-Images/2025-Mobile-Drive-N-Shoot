using FMODUnity;
using UnityEngine;

namespace __Workspaces.Alex.Scripts
{
    [CreateAssetMenu(fileName = "Item_Repair", menuName = "Items/Item Repair")]
    public class ItemRepair : Item
    {
        
        [Header("SFX")]
        [SerializeField] private EventReference _useSFX;
        public override void Execute(GameObject target)
        {
            var health = target.GetComponent<CarHealth>();
            health.Heal();
            // Play SFX
            AudioManager.Instance.PlayAtPosition(_useSFX, target.transform.position);
        }
    }
}
