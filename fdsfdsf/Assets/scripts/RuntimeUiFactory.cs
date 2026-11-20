using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Small helpers for UI panels created by existing windows at runtime.</summary>
public static class RuntimeUiFactory
{
    public static RectTransform Panel(Transform parent, string name, Vector2 size, Vector2 position)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rect = (RectTransform)go.transform; rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f); rect.sizeDelta = size; rect.anchoredPosition = position;
        go.GetComponent<Image>().color = new Color(.10f, .075f, .055f, .96f);
        return rect;
    }

    public static TMP_Text Text(Transform parent, string value, Vector2 position, Vector2 size, int fontSize = 26)
    {
        var go = new GameObject("Text - " + value, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var rect = (RectTransform)go.transform; rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.sizeDelta = size; rect.anchoredPosition = position;
        var text = go.GetComponent<TextMeshProUGUI>(); text.text = value; text.fontSize = fontSize;
        text.color = new Color(.96f, .84f, .59f); text.alignment = TextAlignmentOptions.Center;
        if (TMP_Settings.defaultFontAsset != null) text.font = TMP_Settings.defaultFontAsset;
        return text;
    }

    public static Button Button(Transform parent, string label, Vector2 position, Vector2 size, UnityEngine.Events.UnityAction action)
    {
        var go = new GameObject("Button - " + label, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rect = (RectTransform)go.transform; rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.sizeDelta = size; rect.anchoredPosition = position;
        var image = go.GetComponent<Image>(); image.color = new Color(.35f, .20f, .10f);
        var button = go.GetComponent<Button>(); button.targetGraphic = image;
        var colors = button.colors; colors.normalColor = new Color(.48f, .28f, .12f); colors.highlightedColor = new Color(.70f, .46f, .19f);
        colors.pressedColor = new Color(.26f, .15f, .08f); colors.disabledColor = new Color(.25f, .23f, .20f, .55f); button.colors = colors;
        var text = Text(go.transform, label, Vector2.zero, size - new Vector2(12, 8), 22); text.raycastTarget = false;
        if (action != null) button.onClick.AddListener(action);
        return button;
    }

    public static Slider Slider(Transform parent, string label, float y)
    {
        Text(parent, label, new Vector2(-160, y + 28), new Vector2(280, 42), 22);
        var go = new GameObject("Slider - " + label, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Slider));
        go.transform.SetParent(parent, false);
        var rect = (RectTransform)go.transform; rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.sizeDelta = new Vector2(390, 22); rect.anchoredPosition = new Vector2(0, y - 8);
        go.GetComponent<Image>().color = new Color(.22f, .18f, .13f);
        var handleGo = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)); handleGo.transform.SetParent(go.transform, false);
        var handle = handleGo.GetComponent<RectTransform>(); handle.sizeDelta = new Vector2(24, 34);
        handleGo.GetComponent<Image>().color = new Color(.92f, .67f, .30f);
        var slider = go.GetComponent<Slider>(); slider.minValue = 0; slider.maxValue = 1;
        slider.targetGraphic = handleGo.GetComponent<Image>(); slider.handleRect = handle; slider.direction = UnityEngine.UI.Slider.Direction.LeftToRight;
        slider.value = 1f;
        return slider;
    }
}
