using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int _maxSlots = 20;
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private List<InventoryItem> _items = new List<InventoryItem>();

    private void Awake()
    {
        LoadInventory();
    }

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

        ConsolidateSlots(item, state);
        _inventoryUI.UpdateUI(_items);
        SaveInventory();
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
            ConsolidateSlots(item, state);
        }
        SaveInventory();
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
            
            ConsolidateSlots(item, currentState);
            ConsolidateSlots(item, newState);
            _inventoryUI.UpdateUI(_items);
            SaveInventory();
        }
    }

    public void SwapItems(InventoryItem draggedItem, InventoryItem targetItem, InventorySlot draggedSlot)
    {
        int draggedIndex = _items.IndexOf(draggedItem);
        if (targetItem == null)
        {
            _inventoryUI.UpdateUI(_items);
            return;
        }

        int targetIndex = _items.IndexOf(targetItem);

        if (draggedIndex != -1 && targetIndex != -1)
        {
            if (draggedItem.itemData == targetItem.itemData && 
                (draggedItem.itemData.type != ItemType.Animal || draggedItem.state == targetItem.state))
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
            else
            {
                _items[draggedIndex] = targetItem;
                _items[targetIndex] = draggedItem;
            }
            
            _inventoryUI.UpdateUI(_items);
            SaveInventory();
        }
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
            SaveInventory();
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

    private void SaveInventory()
    {
        string saveData = "";
        foreach (var item in _items)
        {
            saveData += $"{item.itemData.id},{item.count},{(int)item.state};";
        }
        PlayerPrefs.SetString("Inventory", saveData);
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        if (PlayerPrefs.HasKey("Inventory"))
        {
            string saveData = PlayerPrefs.GetString("Inventory");
            if (!string.IsNullOrEmpty(saveData))
            {
                _items.Clear();
                string[] itemData = saveData.Split(';');
                foreach (var data in itemData)
                {
                    if (string.IsNullOrEmpty(data)) continue;
                    string[] parts = data.Split(',');
                    int id = int.Parse(parts[0]);
                    int count = int.Parse(parts[1]);
                    AnimalState state = (AnimalState)int.Parse(parts[2]);

                    Item item = FindItemById(id); // Предполагается, что у вас есть способ найти Item по ID
                    if (item != null)
                    {
                        _items.Add(new InventoryItem(item, count, state));
                    }
                }
                _inventoryUI.UpdateUI(_items);
            }
        }
    }

    private Item FindItemById(int id)
    {
        return Resources.FindObjectsOfTypeAll<Item>().FirstOrDefault(i => i.id == id);
    }
}