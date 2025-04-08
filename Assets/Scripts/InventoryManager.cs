// InventoryManager.cs
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxSlots = 20;
    [SerializeField] private InventoryUI inventoryUI;
   [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(Item item, int count = 1, AnimalState state = AnimalState.Healthy)
    {
        InventoryItem existingItem = FindMatchingItem(item, state);
        
        if (existingItem != null && existingItem.Count < item.StackLimit)
        {
            int remainingSpace = item.StackLimit - existingItem.Count;
            int toAdd = Mathf.Min(count, remainingSpace);
            existingItem.Count += toAdd;
            count -= toAdd;
        }

        while (count > 0 && items.Count < maxSlots)
        {
            int amountToAdd = Mathf.Min(count, item.StackLimit);
            items.Add(new InventoryItem(item, amountToAdd, state));
            count -= amountToAdd;
        }

        inventoryUI.UpdateUI(items);
    }

    public void RemoveItem(Item item, AnimalState state)
    {
        InventoryItem existingItem = FindMatchingItem(item, state);
        
        if (existingItem != null)
        {
            existingItem.Count--;
            if (existingItem.Count <= 0)
                items.Remove(existingItem);
            inventoryUI.UpdateUI(items);
        }
    }

    public void ToggleAnimalState(Item item, AnimalState currentState)
    {
        InventoryItem existingItem = FindMatchingItem(item, currentState);
        if (existingItem != null && existingItem.ItemData.Type == ItemType.Animal)
        {
            AnimalState newState = currentState == AnimalState.Healthy ? AnimalState.Wounded : AnimalState.Healthy;
            RemoveItem(item, currentState);
            AddItem(item, 1, newState);
        }
    }

    private InventoryItem FindMatchingItem(Item item, AnimalState state)
    {
        return items.Find(i => i.ItemData == item && 
                            (i.ItemData.Type != ItemType.Animal || i.State == state) && 
                            i.Count < i.ItemData.StackLimit);
    }
}