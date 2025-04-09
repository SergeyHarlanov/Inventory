using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Text _countText;
    [SerializeField] private Text _stateText;

    private int _previousCount = 0;

    public void ResetData()
    {
        _previousCount = 0;
    }

    public void Setup(InventoryItem item)
    {
        int newCount = item.count;

        _icon.sprite = item.itemData.icon;
        _countText.text = newCount.ToString();

        bool isAnimal = item.itemData.type == ItemType.Animal;
        _stateText.gameObject.SetActive(isAnimal);
        if (isAnimal)
        {
            _stateText.text = item.state.ToString();
            _stateText.color = item.state == AnimalState.Healthy ? Color.green : Color.red;
        }

        if (_previousCount > 0)
        {
            if (newCount < _previousCount)
            {
                //transform.DOPunchScale(Vector3.one * -0.2f, 0.2f).SetDelay(0.05f);
            }
            else if (newCount > _previousCount)
            {
                //transform.DOPunchScale(Vector3.one * 0.2f, 0.2f).SetDelay(0.05f);
            }
        }

        if (_previousCount == 0)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.2f).SetDelay(0.05f);
        }

        _previousCount = newCount;
    }
}