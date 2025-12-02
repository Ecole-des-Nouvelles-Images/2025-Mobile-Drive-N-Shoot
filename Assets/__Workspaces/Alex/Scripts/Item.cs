using System;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public ItemType ItemType;
    public abstract void Execute(GameObject target);
}

[Serializable]
public enum ItemType
{
    Repair,
    Boost,
    Overheat,
    BigBlast
}
