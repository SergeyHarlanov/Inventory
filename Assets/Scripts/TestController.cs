// TestController.cs
using UnityEngine;

public class TestController : MonoBehaviour
{
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private Item[] testItems;

    public void AddTestItem(int itemIndex)
    {
        inventory.AddItem(testItems[itemIndex], 1);
    }

    public void RemoveTestItem(int itemIndex)
    {
        inventory.RemoveItem(testItems[itemIndex], AnimalState.Healthy);
    }

    public void ToggleAnimalState(int itemIndex)
    {
        if (testItems[itemIndex].Type == ItemType.Animal)
            inventory.ToggleAnimalState(testItems[itemIndex], AnimalState.Healthy);
    }
}