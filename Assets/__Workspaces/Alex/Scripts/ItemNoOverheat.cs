using FMODUnity;
using UnityEngine;

namespace __Workspaces.Alex.Scripts
{
    [CreateAssetMenu(fileName = "Item_NoOverheat", menuName = "Items/Item No Overheat")]
    public class ItemNoOverheat : Item
    {
        public float Duration = 30f;
        [Header("SFX")]
        [SerializeField] private EventReference _useSFX;
    
        public override void Execute(GameObject target)
        {
            TurretControler turret = target.GetComponent<TurretControler>();
            turret.ActivateNoOverheat(Duration);
            // Play SFX 
            AudioManager.Instance.PlayAtPosition(_useSFX, target.transform.position);
            Debug.Log("Item_NoOverheat");
        }
    }
}
