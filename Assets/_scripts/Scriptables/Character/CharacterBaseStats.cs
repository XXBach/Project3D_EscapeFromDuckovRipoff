using UnityEngine;

[CreateAssetMenu(fileName = "CharacterBaseStats", menuName = "Scriptable Objects/CharacterBaseStats")]
public class CharacterBaseStats : ScriptableObject
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _baseSpeed;
    [SerializeField] private float _maxWeight;
    [SerializeField] private float _baseEyeSight;
    [SerializeField] private float _baseHungryness;
    [SerializeField] private float _baseThirstyness;

    public float MaxHealth {
        get { return _maxHealth; }     
    }
    public float BaseSpeed
    {
        get { return _baseSpeed; }
    }
    public float Hungryness
    {
        get { return _baseHungryness; }
    }
    public float Thirstyness
    {
        get { return _baseThirstyness; }
    }
    public float MaxWeight
    {
        get { return _maxWeight; }
    }
    public float BaseEyeSight
    {
        get { return _baseEyeSight; }
    }
}
