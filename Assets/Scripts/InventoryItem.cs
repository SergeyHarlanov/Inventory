[System.Serializable]
public class InventoryItem
{
    public Item ItemData;
    public int Count;
    public AnimalState State; // Только для животных

    public InventoryItem(Item itemData, int count, AnimalState state = AnimalState.Healthy)
    {
        ItemData = itemData;
        Count = count;
        State = state;
    }
}