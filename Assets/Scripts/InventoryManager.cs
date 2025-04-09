using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int _maxSlots = 20;
    [SerializeField] private InventoryUI _inventoryUI;
    
     private List<InventoryItem> _items = new List<InventoryItem>();

    public void AddItem(Item item, int count = 1, AnimalState state = AnimalState.Healthy)
    {
        InventoryItem existingItem = FindMatchingItem(item, state);
        
        if (existingItem != null && existingItem.count < item.stackLimit)
        {
            int remainingSpace = item.stackLimit - existingItem.count;
            int toAdd = Mathf.Min(count, remainingSpace);
            existingItem.count += toAdd;
            count -= toAdd;
        }

        while (count > 0 && _items.Count < _maxSlots)
        {
            int amountToAdd = Mathf.Min(count, item.stackLimit);
            _items.Add(new InventoryItem(item, amountToAdd, state));
            count -= amountToAdd;
        }

        _inventoryUI.UpdateUI(_items);
    }

    public void RemoveItem(Item item, AnimalState state)
    {
        InventoryItem existingItem = FindMatchingItem(item, state);
        
        if (existingItem != null)
        {
            existingItem.count--;
            if (existingItem.count <= 0)
                _items.Remove(existingItem);
            _inventoryUI.UpdateUI(_items);
        }
    }

    public void ToggleAnimalState(Item item, AnimalState currentState)
    {
        InventoryItem existingItem = _items.Find(i => i.itemData == item && i.state == currentState);
        
        if (existingItem != null && existingItem.itemData.type == ItemType.Animal)
        {
            // Вместо удаления и добавления просто меняем состояние
            existingItem.state = currentState == AnimalState.Healthy ? AnimalState.Wounded : AnimalState.Healthy;
            _inventoryUI.UpdateUI(_items);
        }
    }

    public void AddRandomItem()
    {
        if (_items.Count > 0)
        {
            int index = Random.Range(0, _items.Count);
            AddItem(_items[index].itemData, 1, _items[index].state);
        }
    }
    
    public void RemoveRandomItem()
    {
        if (_items.Count > 0)
        {
            int index = Random.Range(0, _items.Count);
            RemoveItem(_items[index].itemData, _items[index].state);
        }
    }
    
    public void ChangeStateRandomItem()
    {
        List<InventoryItem> itemsAnimals = _items.FindAll(x => x.itemData.type == ItemType.Animal); 
        if (itemsAnimals.Count > 0)
        {
            int index = Random.Range(0, itemsAnimals.Count);
            ToggleAnimalState(itemsAnimals[index].itemData, itemsAnimals[index].state);
        }
    }

    private InventoryItem FindMatchingItem(Item item, AnimalState state)
    {
        return _items.Find(i => i.itemData == item && 
                            (i.itemData.type != ItemType.Animal || i.state == state) && 
                            i.count < i.itemData.stackLimit);
    }
}