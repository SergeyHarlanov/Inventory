// Item.cs
using UnityEngine;

public enum ItemType { Resource, Animal, Consumable }
public enum AnimalState { Healthy, Wounded }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int ID;
    public string Name;
    public ItemType Type;
    public Sprite Icon;
    public int StackLimit;
}

// InventoryItem.cs