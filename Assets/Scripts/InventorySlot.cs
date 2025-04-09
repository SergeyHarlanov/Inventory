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

        bool isAnimal = item.itemData.type == ItemType.Animal;
        _stateText.gameObject.SetActive(isAnimal);
        if (isAnimal)
        {
            _stateText.text = item.state.ToString();
            _stateText.color = item.state == AnimalState.Healthy ? Color.green : Color.red;
        }
    }
}