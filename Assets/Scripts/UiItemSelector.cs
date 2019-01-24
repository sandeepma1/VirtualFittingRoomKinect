using UnityEngine;
using UnityEngine.UI;

public class UiItemSelector : MonoBehaviour
{
    [SerializeField] private float showDistance;
    [SerializeField] private RectTransform previewGo;
    [SerializeField] private Image previewImage;
    [SerializeField] private UiKinectButton leftButton;
    [SerializeField] private UiKinectButton rightButton;
    [SerializeField] private Sprite[] items;
    [SerializeField] private float distanceOffset = 2.5f;
    private int currentItemIndex = 0;
    private int totalItems;

    private void Start()
    {
        leftButton.OnButtonClicked += OnLeftButtonClick;
        rightButton.OnButtonClicked += OnRightButtonClick;
        totalItems = items.Length - 1;
        OnRightButtonClick();
    }

    private void OnRightButtonClick()
    {
        currentItemIndex++;
        if (currentItemIndex > totalItems)
        {
            currentItemIndex = 0;
        }
        previewImage.sprite = items[currentItemIndex];
    }

    private void OnLeftButtonClick()
    {
        currentItemIndex--;
        if (currentItemIndex < 0)
        {
            currentItemIndex = totalItems;
        }
        previewImage.sprite = items[currentItemIndex];
    }

    public void MoveItem(Vector2 position)
    {
        previewGo.anchoredPosition = position;
    }

    public void ScaleItem(float distance)
    {
        showDistance = distance;
        distance *= distanceOffset;
        previewImage.rectTransform.sizeDelta = new Vector2(distance, previewImage.rectTransform.sizeDelta.y);
    }
}
