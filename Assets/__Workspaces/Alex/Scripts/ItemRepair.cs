using UnityEngine;

namespace __Workspaces.Alex.Scripts
{
    [CreateAssetMenu(fileName = "Item_Repair", menuName = "Items/Item Repair")]
    public class ItemRepair : Item
    {
        public override void Execute(GameObject target)
        {
            var health = target.GetComponent<CarHealth>();
            health.Heal();
        }
    }
}
