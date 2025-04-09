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

        while (count > 0 && _items.Count < _maxSlots)
        {
            int amountToAdd = Mathf.Min(count, item.stackLimit);
            _items.Add(new InventoryItem(item, amountToAdd, state));
            count -= amountToAdd;
        }

        ConsolidateSlots(item, state); // Объединяем слоты после добавления
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
            {
                _items.Remove(existingItem);
            }
            ConsolidateSlots(item, state); // Объединяем слоты после удаления
        }
    }

    public void ToggleAnimalState(Item item, AnimalState currentState)
    {
        InventoryItem existingItem = _items.Find(i => i.itemData == item && i.state == currentState);
        
        if (existingItem != null && existingItem.itemData.type == ItemType.Animal)
        {
            AnimalState newState = currentState == AnimalState.Healthy ? AnimalState.Wounded : AnimalState.Healthy;

            if (existingItem.count > 1)
            {
                existingItem.count--;
                AddItem(item, 1, newState);
            }
            else
            {
                _items.Remove(existingItem);
                AddItem(item, 1, newState);
            }
            
            ConsolidateSlots(item, currentState); // Объединяем слоты для текущего состояния
            ConsolidateSlots(item, newState);     // Объединяем слоты для нового состояния
            _inventoryUI.UpdateUI(_items);
        }
    }

    private void ConsolidateSlots(Item item, AnimalState state)
    {
        // Находим все слоты с таким же item и state
        List<InventoryItem> matchingSlots = _items.FindAll(i => i.itemData == item && 
                                                              (i.itemData.type != ItemType.Animal || i.state == state));
        
        if (matchingSlots.Count > 1)
        {
            // Суммируем все предметы в первый слот
            InventoryItem primarySlot = matchingSlots[0];
            int totalCount = primarySlot.count;
            for (int i = 1; i < matchingSlots.Count; i++)
            {
                totalCount += matchingSlots[i].count;
                _items.Remove(matchingSlots[i]);
            }

            // Распределяем общее количество по слотам с учетом stackLimit
            primarySlot.count = Mathf.Min(totalCount, item.stackLimit);
            totalCount -= primarySlot.count;

            while (totalCount > 0 && _items.Count < _maxSlots)
            {
                int amountToAdd = Mathf.Min(totalCount, item.stackLimit);
                _items.Add(new InventoryItem(item, amountToAdd, state));
                totalCount -= amountToAdd;
            }
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