using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int _maxSlots = 20;
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private List<InventoryItem> _items = new List<InventoryItem>();

    public void AddItem(Item item, int count = 1, AnimalState state = AnimalState.Healthy)
    {
        InventoryItem existingItem = FindMatchingItem(item, state);
        
        if (existingItem != null && existingItem.Count < item.stackLimit)
        {
            int remainingSpace = item.stackLimit - existingItem.Count;
            int toAdd = Mathf.Min(count, remainingSpace);
            existingItem.Count += toAdd;
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
            existingItem.Count--;
            if (existingItem.Count <= 0)
                _items.Remove(existingItem);
            _inventoryUI.UpdateUI(_items);
        }
    }

    public void ToggleAnimalState(Item item, AnimalState currentState)
    {
        InventoryItem existingItem = _items.Find(i => i.ItemData == item && i.State == currentState);
        
        if (existingItem != null && existingItem.ItemData.type == ItemType.Animal)
        {
            // Вместо удаления и добавления просто меняем состояние
            existingItem.State = currentState == AnimalState.Healthy ? AnimalState.Wounded : AnimalState.Healthy;
            _inventoryUI.UpdateUI(_items);
        }
    }

    public void AddRandomItem()
    {
        if (_items.Count > 0)
        {
            int index = Random.Range(0, _items.Count);
            AddItem(_items[index].ItemData, 1, _items[index].State);
        }
    }
    
    public void RemoveRandomItem()
    {
        if (_items.Count > 0)
        {
            int index = Random.Range(0, _items.Count);
            RemoveItem(_items[index].ItemData, _items[index].State);
        }
    }
    
    public void ChangeStateRandomItem()
    {
        List<InventoryItem> itemsAnimals = _items.FindAll(x => x.ItemData.type == ItemType.Animal); 
        if (itemsAnimals.Count > 0)
        {
            int index = Random.Range(0, itemsAnimals.Count);
            ToggleAnimalState(itemsAnimals[index].ItemData, itemsAnimals[index].State);
        }
    }

    private InventoryItem FindMatchingItem(Item item, AnimalState state)
    {
        return _items.Find(i => i.ItemData == item && 
                            (i.ItemData.type != ItemType.Animal || i.State == state) && 
                            i.Count < i.ItemData.stackLimit);
    }
}