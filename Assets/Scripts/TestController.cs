// TestController.cs

using System;
using UnityEngine;

public class TestController : MonoBehaviour
{
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private Item[] testItems;

    private void Start()
    {
        for (int i = 0; i < testItems.Length; i++)
        {
              AddTestItem(i);
        }
    }

    public void AddTestItem(int itemIndex)
    {
        inventory.AddItem(testItems[itemIndex], 1);
    }
    
    public void AddRandomTestItem()
    {
        inventory.AddRandomItem();
    }

    public void RemoveTestItem()
    {
        inventory.RemoveRandomItem();
    }
    
    public void ChangeStateRandomItem()
    {
        inventory.RemoveRandomItem();
    }

    public void ToggleAnimalState(int itemIndex)
    {
        if (testItems[itemIndex].type == ItemType.Animal)
            inventory.ToggleAnimalState(testItems[itemIndex], AnimalState.Healthy);
    }
}