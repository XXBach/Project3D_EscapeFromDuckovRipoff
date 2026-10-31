using UnityEngine;
[System.Serializable]
public class Ammo : Item, ILethality
{
    [SerializeField] protected AmmoBaseStats _baseStats;
    public AmmoBaseStats BaseStats
    {
        get { return _baseStats; }
    }
}
