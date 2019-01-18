// OpenPose Unity Plugin v1.0.0alpha-1.5.0
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OpenPose.Example
{
    /*
	 * ImageRenderer renders the pure cvInputData to Texture2D
	 * The texture is used as background image
	 */
    public class ImageRenderer : MonoBehaviour
    {
        [SerializeField] Vector2Int screenSize;
        // Texture to be rendered in image
        private Texture2D texture;
        private RectTransform rectTransform { get { return GetComponent<RectTransform>(); } }
        private RawImage image { get { return GetComponent<RawImage>(); } }
        int tempWidth, tempHeight;

        void Start()
        {
            texture = new Texture2D(screenSize.x, screenSize.y);
            image.texture = texture;
        }

        public void UpdateImage(MultiArray<byte> data)
        {
            // data size: width * height * 3 (BGR)
            if (data == null || data.Empty()) return;

            ///* TRICK */
            //// Unity does not support BGR24 yet, which is the color format in OpenCV.
            //// Here we are using RGB24 as data format, then swap R and B in shader, to maintain the performance.
            int height = data.GetSize(0), width = data.GetSize(1);
            if (tempHeight != height || tempHeight != height)
            {
                print("setting size");
                tempHeight = height;
                tempHeight = height;
                rectTransform.sizeDelta = new Vector2Int(width, height);
                texture.Resize(width, height, TextureFormat.RGB24, false);
            }
            texture.LoadRawTextureData(data.ToArray());
            texture.Apply();
        }
    }
}
