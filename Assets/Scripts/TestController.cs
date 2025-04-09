// TestController.cs

using System;
using UnityEngine;

public class TestController : MonoBehaviour
{
    [SerializeField] private InventoryManager _inventory;
    [SerializeField] private Item[] _testItems;

    private void Start()
    {
        for (int i = 0; i < _testItems.Length; i++)
        {
              AddTestItem(i);
        }
    }

    public void AddTestItem(int itemIndex)
    {
        _inventory.AddItem(_testItems[itemIndex], 1);
    }
    
    public void AddRandomTestItem()
    {
        _inventory.AddRandomItem();
    }

    public void RemoveRandomTestItem()
    {
        _inventory.RemoveRandomItem();
    }
    
    public void ChangeStateRandomItem()
    {
        _inventory.ChangeStateRandomItem();
    }

    public void ToggleAnimalState(int itemIndex)
    {
        if (_testItems[itemIndex].type == ItemType.Animal)
            _inventory.ToggleAnimalState(_testItems[itemIndex], AnimalState.Healthy);
    }
}