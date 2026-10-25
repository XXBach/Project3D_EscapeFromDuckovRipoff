using UnityEngine;

[CreateAssetMenu(fileName = "CharacterBaseStats", menuName = "Scriptable Objects/CharacterBaseStats")]
public class CharacterBaseStats : ScriptableObject
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _baseSpeed;
    [SerializeField] private int _baseWeight;
    [SerializeField] private int _baseEyeSight;
    [SerializeField] private int _hungryness;
    [SerializeField] private int _thirstyness;

    public int MaxHealth {
        get { return _maxHealth; }     
    }
    public int BaseSpeed
    {
        get { return _baseSpeed; }
    }
    public int Hungryness
    {
        get { return Hungryness; }
    }
    public int Thirstyness
    {
        get { return _thirstyness; }
    }
    public int BaseWeight
    {
        get { return _baseWeight; }
    }
    public int BaseEyeSight
    {
        get { return _baseEyeSight; }
    }
}
