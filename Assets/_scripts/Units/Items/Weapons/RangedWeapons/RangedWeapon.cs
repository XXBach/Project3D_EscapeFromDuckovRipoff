using UnityEngine;
[System.Serializable]
public class RangedWeapon : Item, IRepairable
{
    [SerializeField] protected FireArmsBaseStats _baseStats;
    protected CurrentStats _effectiveStats;

    public CurrentStats EffectiveStats
    {
        get { return this._effectiveStats; }
    }

    public RangedWeapon (FireArmsBaseStats baseStats, CurrentStats effectiveStats)
    {
        _baseStats = baseStats;
        _effectiveStats = effectiveStats;
    }

    public void WeaponBasicSetup()
    {
        _effectiveStats.CurrentDamage = _baseStats.BaseDamage;
        _effectiveStats.CurrentOverRangeMultiplier = _baseStats.OverRangeMultiplier;
        _effectiveStats.CurrentFireRate = _baseStats.FireRate;
        _effectiveStats.CurrentMagazineCapacity = _baseStats.MagazineCapacity;
        _effectiveStats.CurrentReloadTime = _baseStats.ReloadTime;
        _effectiveStats.CurrentRange = _baseStats.Range;
        _effectiveStats.CurrentCritDamageMultiplier = _baseStats.CritDamageMultiplier;
        _effectiveStats.CurrentADSTime = _baseStats.ADSTime;
        _effectiveStats.CurrentExplosionDamageCoefficient = _baseStats.ExplosionDamageCoefficient;
        _effectiveStats.CurrentVerticalRecoil = _baseStats.VerticalRecoil;
        _effectiveStats.CurrentHorizontalRecoil = _baseStats.HorizontalRecoil;
        _effectiveStats.CurrentDurability = _baseStats.Durability;
        _effectiveStats.CurrentChanceToBleed = _baseStats.ChanceToBleed;
    }
    public virtual void TakeDurabilityDamage(float damage)
    {
        _effectiveStats.CurrentDurability -= damage;
    }
    public virtual void Repair(float repairAmount)
    {
        _effectiveStats.CurrentDurability += repairAmount;
    }
}

public class CurrentStats
{
    public float CurrentDamage { get; set; }
    public float CurrentOverRangeMultiplier { get; set; }
    public float CurrentFireRate { get; set; }
    public float CurrentMagazineCapacity { get; set; }
    public float CurrentReloadTime { get; set; }
    public float CurrentRange { get; set; }
    public float CurrentCritDamageMultiplier { get; set; }
    public float CurrentADSTime { get; set; }
    public float CurrentExplosionDamageCoefficient { get; set; }
    public float CurrentVerticalRecoil { get; set; }
    public float CurrentHorizontalRecoil { get; set; }
    public float CurrentDurability { get; set; }
    public float CurrentChanceToBleed { get; set; }
}
