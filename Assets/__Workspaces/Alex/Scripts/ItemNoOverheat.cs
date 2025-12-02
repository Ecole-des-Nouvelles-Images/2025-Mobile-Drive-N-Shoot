using UnityEngine;

[CreateAssetMenu(fileName = "Item_NoOverheat", menuName = "Items/Item No Overheat")]
public class ItemNoOverheat : Item
{
    public float Duration = 30f;
    
    public override void Execute(GameObject target)
    {
        TurretControler turret = target.GetComponent<TurretControler>();
        turret.ActivateNoOverheat(Duration);
        Debug.Log("Item_NoOverheat");
    }
}
