using UnityEngine;

[CreateAssetMenu(fileName = "FireArmsBaseStats", menuName = "Scriptable Objects/FireArmsBaseStats")]
public class FireArmsBaseStats : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private float _weight;
    [SerializeField] private float _baseDamage;
    [SerializeField] private float _overRangeMultiplier;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _magazineCapacity;
    [SerializeField] private float _reloadTime;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _range;
    [SerializeField] private float _critDamageMultiplier;
    [SerializeField] private float _soundRange;
    [SerializeField] private float _adsTime;
    [SerializeField] private float _movementSpeedCoefficient;
    [SerializeField] private float _adsMovementCoefficient;
    [SerializeField] private float _explosionDamageCoefficient;
    [SerializeField] private float _hipFireSpread;
    [SerializeField] private float _adsSpread;
    [SerializeField] private float _verticalRecoil;
    [SerializeField] private float _horizontalRecoil;
    [SerializeField] private float _durability;
    [SerializeField] private float _chanceToBleed;

    
    public string Name { get { return _name; } }
    public float Weight { get { return _weight; } }
    public float BaseDamage { get { return _baseDamage; } }
    public float OverRangeMultiplier { get { return _overRangeMultiplier; } }
    public float FireRate { get { return _fireRate; } }
    public float MagazineCapacity { get { return _magazineCapacity; } }
    public float ReloadTime { get { return _reloadTime; } }
    public float BulletSpeed { get { return _bulletSpeed; } }
    public float Range { get { return _range; } }
    public float CritDamageMultiplier { get { return _critDamageMultiplier; } }
    public float SoundRange { get { return _soundRange; } }
    public float ADSTime { get { return _adsTime; } }
    public float MovementSpeedCoefficient { get { return _movementSpeedCoefficient; } }
    public float ADSMovementCoefficient { get { return _adsMovementCoefficient; } }
    public float ExplosionDamageCoefficient { get { return _explosionDamageCoefficient; } }
    public float HipFireSpread { get { return _hipFireSpread; } }
    public float ADSSpread { get { return _adsSpread; } }
    public float VerticalRecoil { get { return _verticalRecoil; } }
    public float HorizontalRecoil { get { return _horizontalRecoil; } }
    public float Durability { get { return _durability; } }
    public float ChanceToBleed { get { return _chanceToBleed; } }

}
