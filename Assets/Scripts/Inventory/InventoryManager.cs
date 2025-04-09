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

        UpdateAndSave();
    }

    public void RemoveItem(Item item, AnimalState state, bool autoConsolidate = false)
    {
        InventoryItem existingItem = FindItem(item, state);
        
        if (existingItem != null)
        {
            existingItem.count--;
            if (existingItem.count <= 0)
            {
                _items.Remove(existingItem);
            }
            
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
                _items.Add(new InventoryItem(item, 1, newState));
            }
            else
            {
                existingItem.state = newState;
            }
            
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
            RemoveItem(_items[index].itemData, _items[index].state, autoConsolidate: false);
        }
    }
    
    public void ChangeStateRandomItem()
    {
        List<InventoryItem> animalItems = _items.FindAll(x => x.itemData.type == ItemType.Animal); 
        if (animalItems.Count == 0) return;

        int randomIndex = Random.Range(0, animalItems.Count);
        InventoryItem selectedItem = animalItems[randomIndex];
        AnimalState newState = selectedItem.state == AnimalState.Healthy ? AnimalState.Wounded : AnimalState.Healthy;
        
        RemoveItem(selectedItem.itemData, selectedItem.state);
        AddItem(selectedItem.itemData, 1, newState);
        
        UpdateAndSave();
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
        (_items[draggedIndex], _items[targetIndex]) = (_items[targetIndex], _items[draggedIndex]);
    }

    private void UpdateAndSave()
    {
        _inventoryUI.UpdateUI(_items);
        _saver.SaveInventory(_items);
    }
}