using UnityEngine;
using UnityEngine.Events;
public class GunBehavior : MonoBehaviour, ILethality
{
    [Header("Prefabs")]
    [SerializeField] private Ammo _usingBullet;
    [SerializeField] private RangedWeapon _usingGun;

    [Header("AudioClips")]
    [SerializeField] private AudioClip _reloadSound;
    [SerializeField] private AudioClip _shootingSound;

    private int _currentAmmo;
    private float _nearestShootingTime;
    private float _cooldownTime;
    private void Awake()
    {
        _usingBullet ??= gameObject.GetComponent<Ammo>();
        _usingGun ??= gameObject.GetComponent<RangedWeapon>();
        _cooldownTime = CalculateCooldownTime(_usingGun.EffectiveStats.CurrentFireRate);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        Bus<Reload>.OnEvent += HandleReload;
        Bus<Shoot>.OnEvent += HandleShoot;
        Bus<Aim>.OnEvent += HandleAim;
    }
    private void OnDisable()
    {
        Bus<Reload>.OnEvent -= HandleReload;
        Bus<Shoot>.OnEvent -= HandleShoot;
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
    public void HandleShoot(Shoot shootEvent)
    {
        if(Time.time < _nearestShootingTime)
        {
            return;
        }
        _nearestShootingTime = Time.time + _cooldownTime;
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
}
