using UnityEngine;

namespace SelectionSkins
{
    [CreateAssetMenu(fileName = "Skin", menuName = "ScriptableObjects/Skin")]
    public class Skin : ScriptableObject
    {
        public SkinType SkinType;
        public Material Material;
    }
    
    public enum SkinType
    {
        Pickup,
        Turret
    }
}