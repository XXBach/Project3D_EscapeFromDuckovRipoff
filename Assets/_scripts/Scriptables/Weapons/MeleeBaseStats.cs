using UnityEngine;

[CreateAssetMenu(fileName = "MeleeBaseStats", menuName = "Scriptable Objects/MeleeBaseStats")]
public class MeleeBaseStats : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private float _weight;
    [SerializeField] private float _baseDamage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _staminaCost;
    [SerializeField] private int _armorPenetrationLevel;
    [SerializeField] private float _critChance;
    [SerializeField] private float _critDamageMultiplier;
    [SerializeField] private float _chanceToBleed;

    public string Name { get { return _name; } }
    public float Weight { get { return _weight; } }
    public float BaseDamage { get { return _baseDamage; } }
    public float AttackSpeed { get { return _attackSpeed; } }
    public float StaminaCost { get { return _staminaCost; } }
    public int ArmorPenetrationLevel { get { return _armorPenetrationLevel; } }
    public float CritChance { get { return _critChance; } }
    public float CritDamageMultiplier { get { return _critDamageMultiplier; } }
    public float ChanceToBleed { get { return _chanceToBleed; } }
}
