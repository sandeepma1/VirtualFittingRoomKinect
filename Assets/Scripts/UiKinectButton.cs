using System;
using UnityEngine;

public class UiKinectButton : MonoBehaviour
{
    public Action OnButtonClicked;

    public void OnButtonClick()
    {
        OnButtonClicked?.Invoke();
    }
}