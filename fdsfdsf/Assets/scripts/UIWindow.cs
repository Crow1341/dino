using DG.Tweening;
using UnityEngine;
using NaughtyAttributes;

public class UIWindow : MonoBehaviour
{
    
    [Header("Data")]
    [SerializeField] private string _id;
    [SerializeField] private RectTransform _canvasRectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private bool _hideOnStart;

    [Header("Animation Settings")]
    [SerializeField] private float showDuration = 0.5f;
    [SerializeField] private float hideDuration = 0.5f;

    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InBack;

    public CanvasGroup CanvasGroup => _canvasGroup;
    public RectTransform CanvasRectTransform => _canvasRectTransform;
    public string Id => _id;
    public void ConfigureIdentity(string id, RectTransform root)
    {
        _id = id;
        _canvasRectTransform = root;
        EnsurePresentationReferences();
    }

    void Start()
    {
        EnsurePresentationReferences();
        Initialize();
    }

    public virtual void Initialize()
    {
        if (_hideOnStart)
        {
            Hide(true);
        }
    }

    [Button]
    public virtual void Show(bool instant = false)
    {
        EnsurePresentationReferences();
        if (_canvasRectTransform == null) { Debug.LogError($"{name} has no window RectTransform.", this); return; }
        _canvasRectTransform.DOKill();
        if (_canvasGroup != null) _canvasGroup.DOKill();
        _canvasRectTransform.gameObject.SetActive(true);
        _canvasRectTransform.SetAsLastSibling();
        if (_canvasGroup != null) { _canvasGroup.interactable = true; _canvasGroup.blocksRaycasts = true; }
        if (instant) { _canvasRectTransform.localScale = Vector3.one; if (_canvasGroup != null) _canvasGroup.alpha = 1f; return; }
        _canvasRectTransform.localScale = Vector3.one * 0.85f;
        if (_canvasGroup != null) _canvasGroup.alpha = 0f;
        _canvasRectTransform.DOScale(Vector3.one, showDuration).SetEase(showEase);
        if (_canvasGroup != null) _canvasGroup.DOFade(1f, showDuration);
    }

    [Button]
    public virtual void Hide(bool instant = false)
    {
        EnsurePresentationReferences();
        if (_canvasRectTransform == null) return;
        _canvasRectTransform.DOKill();
        if (_canvasGroup != null) _canvasGroup.DOKill();
        if (_canvasGroup != null) { _canvasGroup.interactable = false; _canvasGroup.blocksRaycasts = false; }
        if (instant) { _canvasRectTransform.localScale = Vector3.zero; _canvasRectTransform.gameObject.SetActive(false); return; }
        _canvasRectTransform.DOScale(Vector3.one * 0.85f, hideDuration).SetEase(hideEase).OnComplete(() => _canvasRectTransform.gameObject.SetActive(false));
        if (_canvasGroup != null) _canvasGroup.DOFade(0f, hideDuration);
    }

    private void EnsurePresentationReferences()
    {
        if (_canvasRectTransform == null)
            _canvasRectTransform = transform as RectTransform;

        if (_canvasRectTransform == null || _canvasGroup != null)
            return;

        _canvasGroup = _canvasRectTransform.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = _canvasRectTransform.gameObject.AddComponent<CanvasGroup>();
    }
}
