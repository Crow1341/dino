using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class ButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rect;
    private Vector3 baseScale;

    private void Awake() { rect = transform as RectTransform; if (rect != null) baseScale = rect.localScale; }
    public void OnPointerEnter(PointerEventData eventData) { Animate(1.045f); }
    public void OnPointerExit(PointerEventData eventData) { Animate(1f); }
    public void OnPointerDown(PointerEventData eventData)
    {
        Animate(.94f);
        if (Dino.Utility.Audio.AudioManager.Instance != null) Dino.Utility.Audio.AudioManager.Instance.PlaySound("click");
    }
    public void OnPointerUp(PointerEventData eventData) { Animate(1.02f); }
    private void Animate(float scale)
    {
        if (rect == null) return;
        rect.DOKill(); rect.DOScale(baseScale * scale, .12f).SetEase(Ease.OutQuad);
    }
    private void OnDisable() { if (rect != null) { rect.DOKill(); rect.localScale = baseScale; } }
}
