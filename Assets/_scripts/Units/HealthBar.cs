using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;
    private void Awake()
    {
        this.gameObject.SetActive(false);
        _healthBar ??= GetComponent<Slider>();
        Bus<HealthChanged>.OnEvent += UpdateHealthBar;
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
        Debug.Log($"Health Changed Event Received for Character: {args._character.name}");
        ShowHealthBar();
        _healthBar.value = args._character.GetCurrentHealth() / args._character.GetMaxHealth();
        Invoke(nameof(HideHealthBar), 2f); // Hide the health bar after 2 seconds
    }
    public void OnEnable()
    {
        
    }
    public void OnDisable()
    {
        Bus<HealthChanged>.OnEvent -= UpdateHealthBar;
    }
}
