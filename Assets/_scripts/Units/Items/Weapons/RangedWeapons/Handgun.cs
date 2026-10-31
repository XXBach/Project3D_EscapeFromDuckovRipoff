using System;
using UnityEngine;
[System.Serializable]
public class Handgun : RangedWeapon
{
    public FireArmsBaseStats BaseStats
    {
        get { return this._baseStats; }
    }
    public CurrentStats EffectiveStats
    {
        get { return this._effectiveStats; }
    }
    public Handgun(FireArmsBaseStats baseStats, CurrentStats effectiveStats) : base(baseStats, effectiveStats)
    {
        
    }

    public override void TakeDurabilityDamage(float damage)
    {
        base.TakeDurabilityDamage(damage);
    }
}
