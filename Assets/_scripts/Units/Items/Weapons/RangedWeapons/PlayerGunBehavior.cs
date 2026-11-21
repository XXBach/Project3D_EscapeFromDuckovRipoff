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

    [Header("Recoil")]
    [SerializeField] private float _recoilBloom;
    [SerializeField] private float _recoilYawDrift;
    [SerializeField] private float _recoilPitchDrift;
    [SerializeField] private float _bloomPerShot;


    public float _aimProgress;
    private int _currentAmmoAmount;
    private float _nearestAvailableShootingTime;
    private float _lastShotTime;
    private float _cooldownTime;
    private bool _isReloading;
    private bool _isAiming;
    private float _effectiveSpread;
    private float _baseSpread;
    private float _verticalFactor;
    private void Awake()
    {
        _usingGun.WeaponBasicSetup();
        _cooldownTime = CalculateCooldownTime(_usingGun.EffectiveStats.CurrentFireRate);
        _gunAudioSource ??= GetComponent<AudioSource>();
        _isReloading = false;
        _aimProgress = 0f;
        _verticalFactor = 0f;
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
        while (elapsedTime < _usingGun.EffectiveStats.CurrentReloadTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / _usingGun.EffectiveStats.CurrentReloadTime);
            yield return null;
        }
        _currentAmmoAmount = _usingGun.EffectiveStats.CurrentMagazineCapacity;
        _isReloading = false;
    }

    public void HandleShoot(ShootTriggered shootEvent)
    {
        if(Time.time < _nearestAvailableShootingTime || _isReloading)
        {
            return;
        }
        _nearestAvailableShootingTime = Time.time + _cooldownTime;
        _lastShotTime = Time.time;
        if(_currentAmmoAmount <= 0)
        {
            _isReloading = true;
            HandleReload(new Reload());
            Bus<Reload>.Raise();
            return;
        }
        Vector3 gunDirection = GetShotDirection();
        BulletManager.Instance.Spawn(_bulletSpawnPoint.position, gunDirection, _usingBullet.BaseStats.Speed, _usingGun.EffectiveStats.CurrentRange, CalculateShootingDamage(), _usingBullet.BaseStats, _usingGun.OwnerDomain);
        _currentAmmoAmount--;
        _gunAudioSource.PlayOneShot(_shootingSound);
        Debug.Log("Gun fired. Ammo remaining: " + _currentAmmoAmount);
        ApplyRecoilKick();

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
        if(Time.time - _nearestAvailableShootingTime >= _usingGun.BaseStats.RecoilRecoveryDelay)
        {
            _recoilBloom = Mathf.MoveTowards(_recoilBloom, 0, _usingGun.BaseStats.RecoilRecoverySpeed * Time.deltaTime);
            _recoilYawDrift = Mathf.MoveTowards(_recoilYawDrift, 0, _usingGun.BaseStats.RecoilRecoverySpeed * Time.deltaTime);
            _recoilPitchDrift = Mathf.MoveTowards(_recoilPitchDrift, 0, _usingGun.BaseStats.RecoilRecoverySpeed * Time.deltaTime);
        }
        _effectiveSpread = Mathf.Clamp((_baseSpread + _recoilBloom), 0, _usingGun.BaseStats.MaxSpread);
    }
    private void ApplyRecoilKick()
    {
        CurrentStats s = _usingGun.EffectiveStats;
        // Aim càng đầy thì recoil càng nhỏ
        float aimMult = Mathf.Lerp(1f, _usingGun.BaseStats.RecoilADSCoefficient, _aimProgress);
        float kick = _usingGun.BaseStats.RecoilToDegreesCoefficient * aimMult;

        _recoilBloom += _bloomPerShot * aimMult;
        _recoilPitchDrift = Mathf.Clamp(_recoilPitchDrift + s.CurrentVerticalRecoil * kick, -_usingGun.BaseStats.MaxSpread, _usingGun.BaseStats.MaxSpread);
        _recoilYawDrift = Mathf.Clamp(_recoilYawDrift + Random.Range(-1f, 1f) * s.CurrentHorizontalRecoil * kick, -_usingGun.BaseStats.MaxSpread, _usingGun.BaseStats.MaxSpread);

        // Cập nhật luôn để phát kế tiếp (và crosshair) thấy ngay, không đợi Update
        _effectiveSpread = Mathf.Clamp(_baseSpread + _recoilBloom, 0f, _usingGun.BaseStats.MaxSpread);
    }

    // Hướng đạn = hướng nòng + lệch ngẫu nhiên trong nón spread + drift của recoil
    private Vector3 GetShotDirection()
    {
        float half = _effectiveSpread * 0.5f;   // _effectiveSpread là góc nón đầy đủ (độ)

        float yaw = Random.Range(-half, half) + _recoilYawDrift;
        float pitch = (Random.Range(-half, half) + _recoilPitchDrift) * _verticalFactor;

        // pitch dương = hất nòng lên
        return _bulletSpawnPoint.rotation * Quaternion.Euler(-pitch, yaw, 0f) * Vector3.forward;
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
