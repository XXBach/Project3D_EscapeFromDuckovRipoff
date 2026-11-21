using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class PlayerGunBehavior : MonoBehaviour, ILethality, IAimSource, ISpeedModifierSource
{
    [Header("Prefabs")]
    [SerializeField] private Ammo _usingBullet;
    [SerializeField] private RangedWeapon _usingGun;
    //[SerializeField] private Transform _muzzleFlashPrefab;

    [Tooltip("Bullet Spawn Point")]
    [SerializeField] private Transform _bulletSpawnPoint;

    [Header("Audio")]
    [SerializeField] private AudioClip _reloadSound;
    [SerializeField] private AudioClip _shootingSound;
    [SerializeField] private AudioSource _gunAudioSource;

    [Header("Reload")]
    [SerializeField] private float _reloadDuration = 2f; // Duration of the reload in seconds

    [Header("Recoil")]
    [SerializeField] private float _recoilBloom;
    [SerializeField] private float _recoilYawDrift;
    [SerializeField] private float _recoilPitchDrift;

    public float _aimProgress;
    private int _currentAmmoAmount;
    private float _nearestShootingTime;
    private float _cooldownTime;
    private bool _isReloading;
    private bool _isAiming;
    private float _effectiveSpread;
    private float _baseSpread;
    private void Awake()
    {
        _usingGun.WeaponBasicSetup();
        _cooldownTime = CalculateCooldownTime(_usingGun.EffectiveStats.CurrentFireRate);
        _gunAudioSource ??= GetComponent<AudioSource>();
        _isReloading = false;
        _aimProgress = 0f;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentAmmoAmount = _usingGun.EffectiveStats.CurrentMagazineCapacity;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAimProgress();
        UpdateSpread();
    }
    private void OnEnable()
    {
        Bus<Reload>.OnEvent += HandleReload;
        Bus<ShootTriggered>.OnEvent += HandleShoot;
        Bus<Aim>.OnEvent += HandleAim;
    }
    private void OnDisable()
    {
        Bus<Reload>.OnEvent -= HandleReload;
        Bus<ShootTriggered>.OnEvent -= HandleShoot;
        Bus<Aim>.OnEvent -= HandleAim;
    }
    public void HandleReload(Reload reloadEvent)
    {
        if(!_isReloading)
        {
            return;
        }
        StartCoroutine(ReloadCoroutine());
    }
    public IEnumerator ReloadCoroutine()
    {
        // Freeze khả năng bắn
        // Hiện thanh tiến độ
        // Sau đó reset lại số đạn trong băng đạn
        // Unfreeze khả năng bắn
        // Play âm thanh reload
        _gunAudioSource.PlayOneShot(_reloadSound);
        _isReloading = true;
        float elapsedTime = 0f;
        while (elapsedTime < _reloadDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / _reloadDuration);
            yield return null;
        }
        _currentAmmoAmount = _usingGun.EffectiveStats.CurrentMagazineCapacity;
        _isReloading = false;
    }

    public void HandleShoot(ShootTriggered shootEvent)
    {
        if(Time.time < _nearestShootingTime || _isReloading)
        {
            return;
        }
        _nearestShootingTime = Time.time + _cooldownTime;
        if(_currentAmmoAmount <= 0)
        {
            _isReloading = true;
            HandleReload(new Reload());
            Bus<Reload>.Raise();
            return;
        }
        BulletManager.Instance.Spawn(_bulletSpawnPoint.position, _bulletSpawnPoint.forward, _usingBullet.BaseStats.Speed, _usingGun.EffectiveStats.CurrentRange, CalculateShootingDamage(), _usingBullet.BaseStats, _usingGun.OwnerDomain);
        _currentAmmoAmount--;
        _gunAudioSource.PlayOneShot(_shootingSound);
        Debug.Log("Gun fired. Ammo remaining: " + _currentAmmoAmount);
        // Cast ray từ vị trí bắn ra hướng bắn theo kỹ thuật raycast từng bước
        // Giữa mỗi viên sẽ có sự lệch vị trí sử dụng stat recoil của Gun
        // So sánh Domain của Gun với Domain của Target để xác định có trúng hay không
        // Nếu trúng thì lấy thông tin Target và Damage để xử lý - tính damage, trừ máu, trừ đạn, trừ độ bền của Gun, trừ độ bền của Target
        // Play âm thanh bắn
        // Play hiệu ứng bắn: muzzle flash, bullet trail, impact effect
        // Nếu không trúng thì trừ đạn, trừ độ bền của Gun
    }
    public void HandleAim(Aim aimEvent)
    {
        if(_isReloading)
        {
            return;
        }
        _isAiming = aimEvent._isAiming;
        // Khi Aim thì sẽ giảm độ giật của Gun
        // Khi Aim thì sẽ giảm tốc độ của Player
        // Play hiệu ứng Aim trên CrossHair
    }
    private void UpdateAimProgress()
    {
        // Đang reload thì không được aim, và aim đang có sẽ tự thu về
        bool wantsAim = _isAiming && !_isReloading;

        float adsTime = Mathf.Max(0.01f, _usingGun.EffectiveStats.CurrentADSTime); // tránh chia 0
        float target = wantsAim ? 1f : 0f;
        float speed = wantsAim ? 1f / adsTime
                               : 2.5f / adsTime;

        _aimProgress = Mathf.MoveTowards(_aimProgress, target, speed * Time.deltaTime);
        _baseSpread = Mathf.Lerp(_usingGun.EffectiveStats.CurrentHipFireSpread, _usingGun.EffectiveStats.CurrentADSSpread, _aimProgress);
    }
    private void UpdateSpread()
    {
        if(Time.time - _nearestShootingTime >= _usingGun.BaseStats.RecoilRecoveryDelay)
        {
            _recoilBloom = Mathf.MoveTowards(_recoilBloom, 0, _usingGun.BaseStats.RecoilRecoverySpeed * Time.deltaTime);
            _recoilYawDrift = Mathf.MoveTowards(_recoilYawDrift, 0, _usingGun.BaseStats.RecoilRecoverySpeed * Time.deltaTime);
            _recoilPitchDrift = Mathf.MoveTowards(_recoilPitchDrift, 0, _usingGun.BaseStats.RecoilRecoverySpeed * Time.deltaTime);
        }
        _effectiveSpread = Mathf.Clamp((_baseSpread + _recoilBloom), 0, _usingGun.BaseStats.MaxSpread);
    }
    public float CalculateCooldownTime(float fireRate, float modifier = 0)
    {
        _cooldownTime = 1f / (fireRate + modifier);
        return _cooldownTime;
    }
    public float CalculateShootingDamage(float modifier = 0)
    {
        return _usingGun.EffectiveStats.CurrentDamage * _usingBullet.BaseStats.DamageCoefficient + modifier;
    }
    public float AimProgress => _aimProgress;
    public float SpeedMultiplier
    {
        get
        {
            float hip = _usingGun.BaseStats.MovementSpeedCoefficient;
            float ads = _usingGun.BaseStats.ADSMovementCoefficient;
            return hip * Mathf.Lerp(hip, ads, _aimProgress);
        }
    }

}
