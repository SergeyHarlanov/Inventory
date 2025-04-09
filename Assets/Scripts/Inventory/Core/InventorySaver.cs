using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySaver
{
    private const string InventoryKey = "Inventory";

    public void SaveInventory(List<InventoryItem> items)
    {
        if (items == null || items.Count == 0)
        {
            PlayerPrefs.SetString(InventoryKey, string.Empty);
            PlayerPrefs.Save();
            return;
        }

        string saveData = string.Join(";", items.Select(item => $"{item.itemData.id},{item.count},{(int)item.state}"));
        PlayerPrefs.SetString(InventoryKey, saveData);
        PlayerPrefs.Save();
    }

    public void LoadInventory(List<InventoryItem> items, Item[] availableItems)
    {
        if (items == null || availableItems == null) return;

        if (!PlayerPrefs.HasKey(InventoryKey)) return;

        string saveData = PlayerPrefs.GetString(InventoryKey);
        if (string.IsNullOrEmpty(saveData)) return;

        items.Clear();
        string[] itemData = saveData.Split(';');
        foreach (string data in itemData)
        {
            if (string.IsNullOrEmpty(data)) continue;

            string[] parts = data.Split(',');
            if (parts.Length != 3) continue;

            int id = int.Parse(parts[0]);
            int count = int.Parse(parts[1]);
            AnimalState state = (AnimalState)int.Parse(parts[2]);

            Item item = FindItemById(id, availableItems);
            if (item != null)
            {
                items.Add(new InventoryItem(item, count, state));
            }
        }
    }

    private Item FindItemById(int id, Item[] availableItems)
    {
        return availableItems.FirstOrDefault(item => item.id == id);
    }
}