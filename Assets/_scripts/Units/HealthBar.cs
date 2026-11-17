using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    private void Awake()
    {
        this.gameObject.SetActive(false);
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
        _healthBar.fillAmount = args._character.GetCurrentHealth() / args._character.GetMaxHealth();
    }
    public void OnEnable()
    {
        Bus<HealthChanged>.OnEvent += UpdateHealthBar;
    }
}
