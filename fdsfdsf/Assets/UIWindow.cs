using DG.Tweening;
using UnityEngine;
using NaughtyAttributes;

public class UIWindow : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private bool _hideOnStart;
    void Start()
    {
        Initalize();
    }

    public virtual void Initalize()
    {
        if (_hideOnStart)
        {
            Hide();
        }
    }

    [Button]
    public virtual void Show(bool instant = false)
    {
        if(instant)
        {
            _canvasRectTransform.gameObject.SetActive(true);
        }
        else
        {
            RectTransform rectTransform = _canvasGroup.GetComponent<RectTransform>();
            rectTransform.DOScale(Vector3.one, duration :0.5f).SetEase(Ease.OutBack);
        }
    }

    [Button]
    public virtual void Hide(bool instant = false)
    {
        if (instant)
        {
            _canvasRectTransform.gameObject.SetActive(false);
        }
        else
        {
            RectTransform rectTransform = _canvasGroup.GetComponent<RectTransform>();
            rectTransform.DOScale(Vector3.zero, duration: 0.5f).SetEase(Ease.InBack);
        }
    }
}
