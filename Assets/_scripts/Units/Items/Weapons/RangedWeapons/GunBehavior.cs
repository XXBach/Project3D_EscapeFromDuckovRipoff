using UnityEngine;
using UnityEngine.Events;
public class GunBehavior : MonoBehaviour, ILethality
{
    [Header("Prefabs")]
    [SerializeField] private Ammo _usingBullet;
    [SerializeField] private RangedWeapon _usingGun;
    //[SerializeField] private Transform _muzzleFlashPrefab;

    [Tooltip("Bullet Spawn Point")]
    [SerializeField] private Transform _bulletSpawnPoint;

    [Header("AudioClips")]
    [SerializeField] private AudioClip _reloadSound;
    [SerializeField] private AudioClip _shootingSound;

    private int _currentAmmoAmount;
    private float _nearestShootingTime;
    private float _cooldownTime;
    private void Awake()
    {
        _usingBullet ??= gameObject.GetComponent<Ammo>();
        _usingGun ??= gameObject.GetComponent<RangedWeapon>();
        _usingGun.WeaponBasicSetup();
        _cooldownTime = CalculateCooldownTime(_usingGun.EffectiveStats.CurrentFireRate);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentAmmoAmount = _usingGun.EffectiveStats.CurrentMagazineCapacity;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        // Freeze khả năng bắn
        // Play âm thanh reload
        // Hiện thanh tiến độ
        // Sau đó reset lại số đạn trong băng đạn
        // Unfreeze khả năng bắn
    }
    public void HandleShoot(ShootTriggered shootEvent)
    {
        if(Time.time < _nearestShootingTime)
        {
            return;
        }
        _nearestShootingTime = Time.time + _cooldownTime;
        BulletManager.Instance.Spawn(_bulletSpawnPoint.position, _bulletSpawnPoint.forward, _usingBullet.BaseStats.Speed, _usingGun.EffectiveStats.CurrentRange, CalculateShootingDamage(), _usingBullet.BaseStats, _usingGun.OwnerDomain);
        _currentAmmoAmount--;
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
        // Khi Aim thì sẽ giảm độ giật của Gun
        // Khi Aim thì sẽ tăng độ chính xác của Gun
        // Khi Aim thì sẽ giảm tốc độ di chuyển của Character
        // Play hiệu ứng Aim
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
}
