// Item.cs
using UnityEngine;

public enum ItemType { Resource, Animal, Consumable }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int id;
    public string name;
    public ItemType type;
    public Sprite icon;
    public int stackLimit;
}

