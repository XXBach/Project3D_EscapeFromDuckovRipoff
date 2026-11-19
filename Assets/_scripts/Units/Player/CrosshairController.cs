using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
public class CrosshairController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform _crosshairRectTransform;
    [SerializeField] private Image _crosshairImage;

    [Header("Target Settings")]
    [SerializeField] private Vector3 _targetScale;
    [SerializeField] private Color _targetColor;
    [SerializeField] private Vector3 _targetRotation;

    [Header("Original Settings")]
    private Vector3 _originalScale;
    private Color _originalColor;
    private Vector3 _originalRotation;

    private void Awake()
    {
        _crosshairRectTransform ??= GetComponent<RectTransform>();
        _crosshairImage ??= GetComponent<Image>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _originalScale = _crosshairRectTransform.localScale;
        _originalColor = _crosshairImage.color;
        _originalRotation = _crosshairRectTransform.eulerAngles;
    }
    private void Update()
    {
    }
    private void HandleAim(Aim aimEvent)
    {
        if (aimEvent._isAiming)
        {
            _crosshairRectTransform.DOScale(_targetScale, 0.2f).SetEase(Ease.OutQuad);
            _crosshairImage.DOColor(_targetColor, 0.2f).SetEase(Ease.OutQuad);
            _crosshairRectTransform.DORotate(_targetRotation, 0.2f).SetEase(Ease.OutQuad);
        }
        else
        {
            _crosshairRectTransform.DOScale(_originalScale, 0.2f).SetEase(Ease.OutQuad);
            _crosshairImage.DOColor(_originalColor, 0.2f).SetEase(Ease.OutQuad);
            _crosshairRectTransform.DORotate(_originalRotation, 0.2f).SetEase(Ease.OutQuad);
        }
    }
    private void OnEnable()
    {
        Bus<Aim>.OnEvent += HandleAim;
    }
    private void OnDisable()
    {
        Bus<Aim>.OnEvent -= HandleAim;
    }

}
