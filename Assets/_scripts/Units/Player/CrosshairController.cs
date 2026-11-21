using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
public class CrosshairController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform _crosshairRectTransform;
    [SerializeField] private Image _crosshairImage;

    [Tooltip("Kéo PlayerGunBehavior (hoặc bất kỳ script implement IAimSource) vào đây")]
    [SerializeField] private MonoBehaviour _aimSourceBehaviour;

    [Header("Target Settings")]
    [SerializeField] private Vector3 _targetScale;
    [SerializeField] private Color _targetColor;
    [SerializeField] private Vector3 _targetRotation;

    [Header("Original Settings")]
    private Vector3 _originalScale;
    private Color _originalColor;
    private Vector3 _originalRotation;

    [Header("AimSource")]
    private IAimSource _aimSource;

    private void Awake()
    {
        _crosshairRectTransform ??= GetComponent<RectTransform>();
        _crosshairImage ??= GetComponent<Image>();
        _aimSource = _aimSourceBehaviour as IAimSource;
        if (_aimSource == null)
        {
            Debug.LogError("CrosshairController: chưa gán IAimSource hợp lệ");
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _originalScale = _crosshairRectTransform.localScale;
        _originalColor = _crosshairImage.color;
        _originalRotation = _crosshairRectTransform.eulerAngles;
        _targetColor.a = 1f; // Set the alpha value to 1 for full opacity
    }
    private void LateUpdate()
    {
        if (_aimSource == null) return;
        float p = _aimSource.AimProgress;
        _crosshairRectTransform.localScale = Vector3.Lerp(_originalScale, _targetScale, p);
        _crosshairImage.color = Color.Lerp(_originalColor, _targetColor, p);

        float z = Mathf.LerpAngle(_originalRotation.z, _targetRotation.z, p);
        _crosshairRectTransform.localRotation = Quaternion.Euler(0f, 0f, z);
    }
}
