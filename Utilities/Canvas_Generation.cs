using TMPro;
using UnityEngine;

namespace YUR.SDK.Unity.UI
{
    public class CreateUI : MonoBehaviour
    {
        public static Canvas CreateCanvas(GameObject object_To_addCanvas)
        {
            Canvas canvas;
            object_To_addCanvas.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            if (object_To_addCanvas.GetComponent<Canvas>() == null)
            {
                canvas = object_To_addCanvas.AddComponent<Canvas>();
            }
            else
            {
                canvas = object_To_addCanvas.GetComponent<Canvas>();
            }
            canvas.renderMode = RenderMode.WorldSpace;
            return canvas;
        }

    
        public static TMP_Text CreateTMP_Text(
        Canvas canvas,
        string text,
        Vector3 anchoredPosition, Vector2 SizeDelta)
        {
            TMP_Text tmp_text;
            var rectTransform = canvas.transform as RectTransform;
            rectTransform.sizeDelta = SizeDelta;

            tmp_text = CreateTextMeshProUGUI(rectTransform, text, anchoredPosition, rectTransform.sizeDelta);
            tmp_text.alignment = TextAlignmentOptions.Center;
            tmp_text.fontSize = 4f;
            tmp_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2f);
            tmp_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 2f);
            tmp_text.enableWordWrapping = false;
            tmp_text.overflowMode = TextOverflowModes.Overflow;
            tmp_text.text = text;
            return tmp_text;
        }

        public static TextMeshProUGUI CreateTextMeshProUGUI(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            GameObject gameObj = new GameObject("CustomUIText");
            gameObj.SetActive(false);

            TextMeshProUGUI textMesh = gameObj.AddComponent<TextMeshProUGUI>();
            textMesh.font = TMP_FontAsset.defaultFontAsset;
            textMesh.rectTransform.SetParent(parent, false);
            textMesh.text = text;
            textMesh.fontSize = 4;
            textMesh.color = Color.white;

            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.sizeDelta = sizeDelta;
            textMesh.rectTransform.anchoredPosition = anchoredPosition;

            gameObj.SetActive(true);
            return textMesh;
        }
    }
}
