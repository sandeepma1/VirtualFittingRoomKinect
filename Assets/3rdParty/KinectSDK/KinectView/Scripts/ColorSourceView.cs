using UnityEngine;

[RequireComponent(typeof(ColorSourceManager))]
public class ColorSourceView : MonoBehaviour
{
    private ColorSourceManager _ColorManager;
    private Renderer thisRenderer;

    private void Start()
    {
        _ColorManager = GetComponent<ColorSourceManager>();
        if (_ColorManager == null)
        {
            Debug.Log("ColorSourceManager not found on this object, please add");
            return;
        }
        thisRenderer = gameObject.GetComponent<Renderer>();
        thisRenderer.material.SetTextureScale("_MainTex", new Vector2(-1, 1));
    }

    private void Update()
    {
        thisRenderer.material.mainTexture = _ColorManager.GetColorTexture();
    }
}
