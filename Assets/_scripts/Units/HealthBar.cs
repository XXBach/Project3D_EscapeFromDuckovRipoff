using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;
    [SerializeField] private CharacterDataManager _owner;
    [SerializeField] private CanvasGroup _canvasGroup;
    private void Awake()
    {
        _healthBar ??= GetComponent<Slider>();
        _canvasGroup ??= GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        _healthBar.value = _owner.GetCurrentHealth() / _owner.GetMaxHealth();
        Invoke(nameof(HideHealthBar), 1f);
    }
    public void ShowHealthBar()
    {
        this._canvasGroup.alpha = 1;
    }
    public void HideHealthBar()
    {
        this._canvasGroup.alpha = 0;
    }
    public void UpdateHealthBar(HealthChanged args)
    {
        if (args._character != _owner) return;
        ShowHealthBar();
        _healthBar.value = _owner.GetCurrentHealth() / _owner.GetMaxHealth();
        Invoke(nameof(HideHealthBar), 1f);
    }
    public void OnEnable()
    {
        Bus<HealthChanged>.OnEvent += UpdateHealthBar;
    }
    public void OnDisable()
    {
        Bus<HealthChanged>.OnEvent -= UpdateHealthBar;
    }
}
