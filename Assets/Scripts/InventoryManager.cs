using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int _maxSlots = 20;
    [SerializeField] private InventoryUI _inventoryUI;
    
    [SerializeField] private List<InventoryItem> _items = new List<InventoryItem>();

    public void AddItem(Item item, int count = 1, AnimalState state = AnimalState.Healthy)
    {
        // Ищем существующий слот с таким же item и state
        InventoryItem existingItem = _items.Find(i => i.itemData == item && 
                                                    (i.itemData.type != ItemType.Animal || i.state == state) && 
                                                    i.count < i.itemData.stackLimit);
        
        if (existingItem != null)
        {
            int remainingSpace = item.stackLimit - existingItem.count;
            int toAdd = Mathf.Min(count, remainingSpace);
            existingItem.count += toAdd;
            count -= toAdd;
        }

        // Если осталось что добавить и есть свободные слоты
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
        InventoryItem existingItem = _items.Find(i => i.itemData == item && 
                                                    (i.itemData.type != ItemType.Animal || i.state == state));
        
        if (existingItem != null)
        {
            existingItem.count--;
            if (existingItem.count <= 0)
                _items.Remove(existingItem);
        }
    }

    public void ToggleAnimalState(Item item, AnimalState currentState)
    {
        InventoryItem existingItem = _items.Find(i => i.itemData == item && i.state == currentState);
        
        if (existingItem != null && existingItem.itemData.type == ItemType.Animal)
        {
            AnimalState newState = currentState == AnimalState.Healthy ? AnimalState.Wounded : AnimalState.Healthy;
            InventoryItem targetSlot = _items.Find(i => i.itemData == item && i.state == newState && i.count < i.itemData.stackLimit);

            if (existingItem.count > 1)
            {
                existingItem.count--;
                if (targetSlot != null)
                {
                    targetSlot.count++;
                }
                else
                {
                    _items.Add(new InventoryItem(item, 1, newState));
                }
            }
            else
            {
                if (targetSlot != null)
                {
                    targetSlot.count++;
                    _items.Remove(existingItem);
                }
                else
                {
                    existingItem.state = newState;
                }
            }
            
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
            _inventoryUI.UpdateUI(_items);
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