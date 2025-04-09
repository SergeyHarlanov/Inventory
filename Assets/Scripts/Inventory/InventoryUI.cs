using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _slotsContainer;

    private List<InventorySlot> _slots = new List<InventorySlot>();
    private List<InventorySlot> _inactiveSlots = new List<InventorySlot>();

    public void UpdateUI(List<InventoryItem> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            InventorySlot slot;
            if (i < _slots.Count)
            {
                slot = _slots[i];
                if (!slot.gameObject.activeSelf)
                    slot.gameObject.SetActive(true);
            }
            else
            {
                slot = GetOrCreateSlot();
                _slots.Add(slot);
            }
            
            slot.Setup(items[i]);
        }

        for (int i = items.Count; i < _slots.Count; i++)
        {
            _slots[i].gameObject.SetActive(false);
            _inactiveSlots.Add(_slots[i]);
        }

        if (items.Count < _slots.Count)
        {
            _slots.RemoveRange(items.Count, _slots.Count - items.Count);
        }
    }

    private InventorySlot GetOrCreateSlot()
    {
        if (_inactiveSlots.Count > 0)
        {
            InventorySlot slot = _inactiveSlots[_inactiveSlots.Count - 1];
            slot.ResetData();
            
            _inactiveSlots.RemoveAt(_inactiveSlots.Count - 1);
            slot.gameObject.SetActive(true);
            return slot;
        }

        GameObject slotObj = Instantiate(_slotPrefab, _slotsContainer);
        return slotObj.GetComponent<InventorySlot>();
    }
}