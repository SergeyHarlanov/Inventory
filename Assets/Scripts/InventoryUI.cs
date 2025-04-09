using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _slotsContainer;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private List<InventorySlot> inactiveSlots = new List<InventorySlot>();

    public void UpdateUI(List<InventoryItem> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            InventorySlot slot;
            if (i < slots.Count)
            {
                slot = slots[i];
                if (!slot.gameObject.activeSelf)
                    slot.gameObject.SetActive(true);
            }
            else
            {
                slot = GetOrCreateSlot();
                slots.Add(slot);
            }
            
            slot.Setup(items[i]); // Вызываем Setup с анимацией
        }

        for (int i = items.Count; i < slots.Count; i++)
        {
            slots[i].gameObject.SetActive(false);
            inactiveSlots.Add(slots[i]);
        }

        if (items.Count < slots.Count)
        {
            slots.RemoveRange(items.Count, slots.Count - items.Count);
        }
    }

    private InventorySlot GetOrCreateSlot()
    {
        if (inactiveSlots.Count > 0)
        {
            InventorySlot slot = inactiveSlots[inactiveSlots.Count - 1];
            slot.ResetData();
            
            inactiveSlots.RemoveAt(inactiveSlots.Count - 1);
            slot.gameObject.SetActive(true);
            return slot;
        }

        GameObject slotObj = Instantiate(_slotPrefab, _slotsContainer);
        return slotObj.GetComponent<InventorySlot>();
    }
}