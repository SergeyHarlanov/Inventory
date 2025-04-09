using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private Text _countText;
    [SerializeField] private Text _stateText;

    private int _previousCount = 0;
    private InventoryItem _item;
    private CanvasGroup _canvasGroup;
    private GameObject _dragInstance;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void ResetData()
    {
        _previousCount = 0;
        _item = null;
    }

    public void Setup(InventoryItem item)
    {
        _item = item;
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

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_item == null) return;

        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;

        _dragInstance = Instantiate(gameObject, transform.parent.parent);
        _dragInstance.GetComponent<CanvasGroup>().blocksRaycasts = false;
        _dragInstance.transform.position = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragInstance != null)
        {
            _dragInstance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragInstance != null)
        {
            Destroy(_dragInstance);
        }

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedSlot = eventData.pointerDrag.GetComponent<InventorySlot>();
        if (draggedSlot != null && draggedSlot != this && draggedSlot._item != null)
        {
            InventoryManager manager = FindObjectOfType<InventoryManager>();
            manager.SwapItems(draggedSlot._item, _item, draggedSlot);
        }
    }
}