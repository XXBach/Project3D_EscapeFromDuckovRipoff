using UnityEngine;

[CreateAssetMenu(fileName = "AmmoBaseStats", menuName = "Scriptable Objects/AmmoBaseStats")]
public class AmmoBaseStats : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private float _damageCoefficient;
    [SerializeField] private float _moneyValue;
    [SerializeField] private int _armorPenetrationLevel;
    [SerializeField] private float _armorBreak;
    [SerializeField] private float _weaponWearOut;
    [SerializeField] private float _debuffChanceCoefficient;
    [SerializeField] private float _chanceToBleed;

    public string Name { get { return _name; } }
    public float DamageCoefficient { get { return _damageCoefficient; } }
    public float MoneyValue { get { return _moneyValue; } }
    public int ArmorPenetrationLevel { get { return _armorPenetrationLevel; } }
    public float ArmorBreak { get { return _armorBreak; } }
    public float WeaponWearOut { get { return _weaponWearOut; } }
    public float DebuffChanceCoefficient { get { return _debuffChanceCoefficient; } }
    public float ChanceToBleed { get { return _chanceToBleed; } }
}
