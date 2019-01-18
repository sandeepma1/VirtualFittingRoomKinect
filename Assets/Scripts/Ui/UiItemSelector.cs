using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiItemSelector : MonoBehaviour
{
    [SerializeField] private Image previewImage;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Sprite[] items;
    private int currentItemIndex = 0;
    private int totalItems;

    private void Start()
    {
        leftButton.onClick.AddListener(OnLeftButtonClick);
        rightButton.onClick.AddListener(OnRightButtonClick);
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
}
