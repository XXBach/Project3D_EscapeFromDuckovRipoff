using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;
    [SerializeField] private CharacterDataManager _owner;
    private void Awake()
    {
        this.gameObject.SetActive(false);
        _healthBar ??= GetComponent<Slider>();

    }
    private void Start()
    {
        _healthBar.value = _owner.GetCurrentHealth() / _owner.GetMaxHealth();
    }
    public void ShowHealthBar()
    {
        _healthBar.gameObject.SetActive(true);
    }
    public void HideHealthBar()
    {
        _healthBar.gameObject.SetActive(false);
    }
    public void UpdateHealthBar(HealthChanged args)
    {
        Debug.Log("Event Received");
        if (args._character != _owner) return;
        ShowHealthBar();
        _healthBar.value = _owner.GetCurrentHealth() / _owner.GetMaxHealth();
        Invoke(nameof(HideHealthBar), 2f); // Hide the health bar after 2 seconds
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
