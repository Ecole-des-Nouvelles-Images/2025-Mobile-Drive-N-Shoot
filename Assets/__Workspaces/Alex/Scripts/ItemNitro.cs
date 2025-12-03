using UnityEngine;

[CreateAssetMenu(fileName = "Item_Nitro", menuName = "Items/Item Nitro")]
public class ItemNitro : Item
{
    public override void Execute(GameObject target)
    {
        target.GetComponent<CarControler>().SetNitroAttackReady();
    }
}
