using UnityEngine;

public class HandCursor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color hideColor;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Color clickedColor;
    private bool isClicked = false;
    private bool isClickDown = true;

    public void IsClicked(Vector3 position)
    {
        transform.position = position;
        isClicked = true;
    }

    public void IsNotClicked(Vector3 position)
    {
        transform.position = position;
        isClicked = false;
        isClickDown = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        sprite.color = disabledColor;
        if (isClicked)
        {
            if (isClickDown)
            {
                isClickDown = false;
                if (collision.gameObject.GetComponent<UiKinectButton>())
                {
                    sprite.color = clickedColor;
                    collision.gameObject.GetComponent<UiKinectButton>()?.OnButtonClick();
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        sprite.color = hideColor;
    }
}