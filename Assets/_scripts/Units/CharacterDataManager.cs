using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class CharacterCurrentStats
{
    public float MaxHealth;
    public float CurrentHealth;
    public float CurrentSpeed;
    public float MaxWeight;
    public float CurrentWeight;
    public float CurrentEyeSight;
    public float CurrentHungryness;
    public float CurrentThirstyness;
}
public enum Domain
{
    None = 0,
    Player = 1,
    Enemy = 2
}

public enum PlayerState
{
    Idle = 0,
    Move = 1,
    Sprint = 2,
    Dodge = 3,
    Reload = 4,
    ThirstyAndHungry = 5,
    Dead = 6
}

public enum EnemyState
{
    Idle = 0,
    Patrol = 1,
    Chase = 2,
    Attack = 3,
    Effect = 4,
    Dead = 5
}
public abstract class CharacterDataManager : MonoBehaviour
{
    [SerializeField] private CharacterBaseStats _charBaseStats;
    [SerializeField] private Domain _charDomain = Domain.None;
    private CharacterCurrentStats _charCurrentStats;
    private void Awake()
    {
        _charCurrentStats.MaxHealth = _charBaseStats.MaxHealth;
        _charCurrentStats.CurrentHealth = _charBaseStats.MaxHealth;
        _charCurrentStats.CurrentSpeed = _charBaseStats.BaseSpeed;
        _charCurrentStats.MaxWeight = _charBaseStats.MaxWeight;
        _charCurrentStats.CurrentWeight = 12.5f;
        _charCurrentStats.CurrentEyeSight = _charBaseStats.BaseEyeSight;
        _charCurrentStats.CurrentHungryness = _charBaseStats.Hungryness;
        _charCurrentStats.CurrentThirstyness = _charBaseStats.Thirstyness;
    }
    public void HealthChange(float changeAmount, bool isTakeDamage)
    {
        if(isTakeDamage)
        {
            if(_charCurrentStats.CurrentHealth <= 0)
            {
                if (_charDomain == Domain.Enemy)
                {
                    Destroy(this.gameObject);
                    Bus<CharacterDied>.Raise(new CharacterDied(this));
                }
                else
                {
                    Bus<GameOver>.Raise();
                    Bus<CharacterDied>.Raise(new CharacterDied(this));
                }
            }
            if (isTakeDamage)
            {
                _charCurrentStats.CurrentHealth -= changeAmount;
                if (_charDomain == Domain.Enemy)
                {
                    Destroy(this.gameObject);
                    Bus<CharacterDied>.Raise(new CharacterDied(this));
                }
                else
                {
                    Bus<GameOver>.Raise();
                    Bus<CharacterDied>.Raise(new CharacterDied(this));
                }
            }
            else
            {
                _charCurrentStats.CurrentHealth += changeAmount;
            }
        }
    }
    public void SpeedChange(float changeAmount, bool isSlowdown)
    {
        if(isSlowdown) _charCurrentStats.CurrentSpeed = Mathf.Min(_charCurrentStats.CurrentSpeed, (_charCurrentStats.CurrentSpeed - changeAmount));
        else _charCurrentStats.CurrentSpeed = Mathf.Max(_charCurrentStats.CurrentSpeed, (_charCurrentStats.CurrentSpeed + changeAmount));
        _charCurrentStats.CurrentSpeed = Mathf.Clamp(_charCurrentStats.CurrentSpeed, 0f, 30f);
    }
    public void WeightChange(float changeAmount, bool isHeavier)
    {
        if (!isHeavier) _charCurrentStats.CurrentWeight = Mathf.Min(_charCurrentStats.CurrentWeight, (_charCurrentStats.CurrentWeight - changeAmount));
        else
        {
            _charCurrentStats.CurrentWeight = Mathf.Max(_charCurrentStats.CurrentWeight, (_charCurrentStats.CurrentWeight + changeAmount));
            if (_charCurrentStats.CurrentWeight >= _charCurrentStats.MaxWeight) SpeedChange(_charCurrentStats.CurrentSpeed, true);
        }
        _charCurrentStats.CurrentWeight = Mathf.Clamp(_charCurrentStats.CurrentWeight, 12.5f, _charCurrentStats.MaxWeight);
    }
    public void EyeSightChange(float changeAmount, bool isNearerSight)
    {
        if (isNearerSight) _charCurrentStats.CurrentEyeSight = Mathf.Min(_charCurrentStats.CurrentEyeSight, (_charCurrentStats.CurrentEyeSight - changeAmount));
        else _charCurrentStats.CurrentEyeSight = Mathf.Max(_charCurrentStats.CurrentEyeSight, (_charCurrentStats.CurrentEyeSight + changeAmount));
        _charCurrentStats.CurrentEyeSight = Mathf.Clamp(_charCurrentStats.CurrentEyeSight, 1f, 100f);
    }
    public void HungrinessChange(float changeAmount, bool isMoreHungry)
    {
        if (!isMoreHungry) _charCurrentStats.CurrentHungryness = Mathf.Min(_charCurrentStats.CurrentHungryness, (_charCurrentStats.CurrentHungryness - changeAmount));
        else _charCurrentStats.CurrentHungryness = Mathf.Max(_charCurrentStats.CurrentHungryness, (_charCurrentStats.CurrentHungryness + changeAmount));
        _charCurrentStats.CurrentHungryness = Mathf.Clamp(_charCurrentStats.CurrentHungryness, 0f, 100f);
        if (_charCurrentStats.CurrentHungryness <= 0f) Bus<CharacterHungryAndThirsty>.Raise(new CharacterHungryAndThirsty(this));
    }
    public void ThirstynessChange(float changeAmount, bool isMoreThirsty)
    {
        if (!isMoreThirsty) _charCurrentStats.CurrentThirstyness = Mathf.Min(_charCurrentStats.CurrentThirstyness, (_charCurrentStats.CurrentThirstyness - changeAmount));
        else _charCurrentStats.CurrentThirstyness = Mathf.Max(_charCurrentStats.CurrentThirstyness, (_charCurrentStats.CurrentThirstyness + changeAmount));
        _charCurrentStats.CurrentThirstyness = Mathf.Clamp(_charCurrentStats.CurrentThirstyness, 0f, 100f);
        if (_charCurrentStats.CurrentThirstyness <= 0f) Bus<CharacterHungryAndThirsty>.Raise(new CharacterHungryAndThirsty(this));
    }
    public float GetMaxHealth()
    {
        return _charCurrentStats.MaxHealth;
    }
    public float GetCurrentHealth()
    {
        return _charCurrentStats.CurrentHealth;
    }
    public float GetCurrentSpeed()
    {
        return _charCurrentStats.CurrentSpeed;
    }
    public float GetMaxWeight()
    {
        return _charCurrentStats.MaxWeight;
    }
    public float GetCurrentWeight()
    {
        return _charCurrentStats.CurrentWeight;
    }
    public float GetCurrentEyeSight()
    {
        return _charCurrentStats.CurrentEyeSight;
    }
    public float GetCurrentHungryness()
    {
        return _charCurrentStats.CurrentHungryness;
    }
    public float GetCurrentThirstyness()
    {
        return _charCurrentStats.CurrentThirstyness;
    }
    public Domain GetDomain()
    {
        return this._charDomain;
    }
}
