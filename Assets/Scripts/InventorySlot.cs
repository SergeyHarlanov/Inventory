using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text countText;
    [SerializeField] private Text stateText;

    public void Setup(InventoryItem item)
    {
        icon.sprite = item.ItemData.Icon;
        countText.text = item.Count.ToString();

        if (item.ItemData.Type == ItemType.Animal)
            stateText.text = item.State.ToString();
        else
            stateText.gameObject.SetActive(false);
    }
}