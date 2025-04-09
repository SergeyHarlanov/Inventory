[System.Serializable]
public class InventoryItem
{
    public Item itemData;
    public int count;
    public AnimalState state; // Только для животных

    public InventoryItem(Item itemData, int count, AnimalState state = AnimalState.Healthy)
    {
        this.itemData = itemData;
        this.count = count;
        this.state = state;
    }
}