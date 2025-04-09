using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int _maxSlots = DefaultMaxSlots;
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private List<InventoryItem> _items = new List<InventoryItem>();
    [SerializeField] private Item[] _availableItems;

    private const int DefaultMaxSlots = 20;

    private InventorySaver _saver;

    private void Awake()
    {
        _saver = new InventorySaver();
        _saver.LoadInventory(_items, _availableItems);
        _inventoryUI.UpdateUI(_items);
    }

    public void AddItem(Item item, int count = 1, AnimalState state = AnimalState.Healthy)
    {
        InventoryItem existingItem = FindMatchingItem(item, state);
        
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

        ConsolidateSlots(item, state);
        UpdateAndSave();
    }

    public void RemoveItem(Item item, AnimalState state)
    {
        InventoryItem existingItem = FindItem(item, state);
        
        if (existingItem != null)
        {
            existingItem.count--;
            if (existingItem.count <= 0)
            {
                _items.Remove(existingItem);
            }
            ConsolidateSlots(item, state);
            UpdateAndSave();
        }
    }

    public void ToggleAnimalState(Item item, AnimalState currentState)
    {
        InventoryItem existingItem = FindItem(item, currentState);
        
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
            
            ConsolidateSlots(item, currentState);
            ConsolidateSlots(item, newState);
            UpdateAndSave();
        }
    }

    public void SwapItems(InventoryItem draggedItem, InventoryItem targetItem, InventorySlot draggedSlot)
    {
        int draggedIndex = _items.IndexOf(draggedItem);
        if (targetItem == null)
        {
            UpdateAndSave();
            return;
        }

        int targetIndex = _items.IndexOf(targetItem);
        if (draggedIndex == -1 || targetIndex == -1) return;

        if (CanStackItems(draggedItem, targetItem))
        {
            StackItems(draggedItem, targetItem, draggedIndex, targetIndex);
        }
        else
        {
            SwapItemsDirectly(draggedIndex, targetIndex);
        }
        
        UpdateAndSave();
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
            UpdateAndSave();
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

    private InventoryItem FindItem(Item item, AnimalState state)
    {
        return _items.Find(i => i.itemData == item && 
                                (i.itemData.type != ItemType.Animal || i.state == state));
    }

    private void ConsolidateSlots(Item item, AnimalState state)
    {
        List<InventoryItem> matchingSlots = _items.FindAll(i => i.itemData == item && 
                                                              (i.itemData.type != ItemType.Animal || i.state == state));
        
        if (matchingSlots.Count > 1)
        {
            InventoryItem primarySlot = matchingSlots[0];
            int totalCount = primarySlot.count;
            for (int i = 1; i < matchingSlots.Count; i++)
            {
                totalCount += matchingSlots[i].count;
                _items.Remove(matchingSlots[i]);
            }

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

    private bool CanStackItems(InventoryItem draggedItem, InventoryItem targetItem)
    {
        return draggedItem.itemData == targetItem.itemData && 
               (draggedItem.itemData.type != ItemType.Animal || draggedItem.state == targetItem.state);
    }

    private void StackItems(InventoryItem draggedItem, InventoryItem targetItem, int draggedIndex, int targetIndex)
    {
        int totalCount = draggedItem.count + targetItem.count;
        if (totalCount <= draggedItem.itemData.stackLimit)
        {
            targetItem.count = totalCount;
            _items.Remove(draggedItem);
        }
        else
        {
            targetItem.count = draggedItem.itemData.stackLimit;
            draggedItem.count = totalCount - draggedItem.itemData.stackLimit;
            _items[draggedIndex] = draggedItem;
            _items[targetIndex] = targetItem;
        }
    }

    private void SwapItemsDirectly(int draggedIndex, int targetIndex)
    {
        InventoryItem temp = _items[draggedIndex];
        _items[draggedIndex] = _items[targetIndex];
        _items[targetIndex] = temp;
    }

    private void UpdateAndSave()
    {
        _inventoryUI.UpdateUI(_items);
        _saver.SaveInventory(_items);
    }
}