using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text countText;
    [SerializeField] private Text stateText;

    public void Setup(InventoryItem item)
    {
        icon.sprite = item.ItemData.icon;
        countText.text = item.Count.ToString();

        if (item.ItemData.type == ItemType.Animal)
            stateText.text = item.State.ToString();
        else
            stateText.gameObject.SetActive(false);
    }
}