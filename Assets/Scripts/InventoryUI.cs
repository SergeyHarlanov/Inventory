using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private ScrollRect scrollRect;

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
            
            slot.Setup(items[i]);
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
            inactiveSlots.RemoveAt(inactiveSlots.Count - 1);
            slot.gameObject.SetActive(true);
            return slot;
        }

        GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
        return slotObj.GetComponent<InventorySlot>();
    }
}