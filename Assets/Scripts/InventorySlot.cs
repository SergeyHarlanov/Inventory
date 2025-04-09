using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Text _countText;
    [SerializeField] private Text _stateText;

    public void Setup(InventoryItem item)
    {
        _icon.sprite = item.itemData.icon;
        _countText.text = item.count.ToString();

        if (item.itemData.type == ItemType.Animal)
            _stateText.text = item.state.ToString();
        else  _stateText.gameObject.SetActive(false);
    }
}