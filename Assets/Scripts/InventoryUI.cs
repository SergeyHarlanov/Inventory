// InventoryUI.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private ScrollRect scrollRect;

    private List<InventorySlot> slots = new List<InventorySlot>();

    public void UpdateUI(List<InventoryItem> items)
    {
        // Очищаем существующие слоты
        foreach (InventorySlot slot in slots)
            Destroy(slot.gameObject);
        slots.Clear();

        // Создаем новые слоты
        foreach (InventoryItem item in items)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            slot.Setup(item);
            slots.Add(slot);
        }
    }
}