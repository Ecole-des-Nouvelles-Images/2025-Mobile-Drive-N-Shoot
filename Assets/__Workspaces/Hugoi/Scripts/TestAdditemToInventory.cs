using UnityEngine;
using Utils.Game;

namespace __Workspaces.Hugoi.Scripts
{
    public class TestAdditemToInventory : MonoBehaviour
    {
        [SerializeField] private Item _item;

        [ContextMenu("CollectedItem")]
        public void CollectedItem()
        {
            EventBus.OnCollectedItem?.Invoke(_item);
        }
        
        [ContextMenu("UsingItem")]
        public void UsingItem()
        {
            EventBus.OnUsingItem?.Invoke(_item.ItemType);
        }
    }
}
