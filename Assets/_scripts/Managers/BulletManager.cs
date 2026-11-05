using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
[System.Serializable]
public struct AmmoVisualData
{
    public Vector3 CurrentPosition;
    public Vector3 FiredDirection;
    public float Speed;
    public float RemainingRange;
    public float Damage;
    public Domain OwnerDomain;
    public AmmoBaseStats AmmoStats;
    public Transform Visual;
    public Transform VisualPrefab;
}
public class BulletManager : MonoBehaviour
{
    [Tooltip("Bullet Manager Instance")]
    public static BulletManager Instance { get; private set; }
    [Tooltip("Hitable Mask")]
    [SerializeField] private LayerMask _hitableMask;
    [Tooltip("Current Bullets On Scene")]
    private readonly List<AmmoVisualData> _currentBullets = new List<AmmoVisualData>();
    [Tooltip("Bullet Visual Pool")]
    private readonly Dictionary<Transform, ObjectPool<Transform>> _bulletVisualPool = new Dictionary<Transform, ObjectPool<Transform>>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        float dt = Time.deltaTime;

        for (int i = _currentBullets.Count - 1; i >= 0; i--)      // duyệt NGƯỢC
        {
            AmmoVisualData b = _currentBullets[i];                    // lấy BẢN SAO ra

            float stepLength = Mathf.Min(b.Speed * dt, b.RemainingRange);

            // Raycast đúng đoạn đường sắp đi
            if (Physics.Raycast(b.CurrentPosition, b.FiredDirection, out RaycastHit hit,
                                stepLength, _hitableMask, QueryTriggerInteraction.Ignore))
            {
                OnBulletHit(b, hit);
                RemoveBullet(i);
                continue;                                  // sang viên tiếp theo
            }

            // Không trúng gì: đạn đi tiếp
            b.CurrentPosition += b.FiredDirection * stepLength;
            b.RemainingRange -= stepLength;
            b.Visual.position = b.CurrentPosition;                // kéo hiển thị theo

            if (b.RemainingRange <= 0f)                    // hết tầm
            {
                RemoveBullet(i);
                continue;
            }

            _currentBullets[i] = b;                               // GHI bản sao ngược lại danh sách
        }
    }
    private ObjectPool<Transform> GetBulletVisualPool(Transform prefab)
    {
        // Đã có pool cho prefab này rồi thì dùng luôn
        if (_bulletVisualPool.TryGetValue(prefab, out ObjectPool<Transform> pool))
            return pool;

        // Chưa có thì tạo pool mới
        pool = new ObjectPool<Transform>(
            createFunc: () => Instantiate(prefab, transform),
            actionOnGet: t => t.gameObject.SetActive(true),
            actionOnRelease: t => t.gameObject.SetActive(false),
            actionOnDestroy: t => Destroy(t.gameObject),
            defaultCapacity: 128);

        _bulletVisualPool.Add(prefab, pool);
        return pool;
    }
    public void Spawn(Vector3 pos, Vector3 dir, float speed, float range,
                  float gunDamage, AmmoBaseStats ammo, Domain owner)
    {
        Transform prefab = ammo.AmmoVisualPrefab;
        Transform visual = GetBulletVisualPool(prefab).Get();          // lấy "đĩa" từ chồng

        visual.SetPositionAndRotation(pos, Quaternion.LookRotation(dir));
        if (visual.TryGetComponent(out TrailRenderer trail)) trail.Clear();   // xóa vệt cũ

        _currentBullets.Add(new AmmoVisualData
        {
            CurrentPosition = pos,
            FiredDirection = dir,
            Speed = speed,
            RemainingRange = range,
            Damage = gunDamage * ammo.DamageCoefficient,   // sát thương súng × hệ số đạn
            OwnerDomain = owner,
            AmmoStats = ammo,
            Visual = visual,
            VisualPrefab = prefab
        });
    }
    private void RemoveBullet(int index)
    {
        AmmoVisualData b = _currentBullets[index];
        _bulletVisualPool[b.VisualPrefab].Release(b.Visual);          // cất "đĩa" về đúng chồng

        // Swap-remove: chép phần tử cuối đè lên chỗ cần xóa rồi bỏ phần tử cuối
        int last = _currentBullets.Count - 1;
        _currentBullets[index] = _currentBullets[last];
        _currentBullets.RemoveAt(last);
    }
    private void OnBulletHit(AmmoVisualData b, RaycastHit hit)
    {
        var target = hit.collider.GetComponentInParent<CharacterDataManager>();

        if (target != null && target.GetDomain() != b.OwnerDomain)
        {
            target.HealthChange(b.Damage, true);
            if(target.GetDomain() == Domain.Player)
            {
                Bus<BulletHitPlayer>.Raise(new BulletHitPlayer(target));
            }
            else
            {
                Bus<BulletHitEnemy>.Raise(new BulletHitEnemy(target));
            }
            // Sau này: xuyên giáp (b.Ammo.ArmorPenetrationLevel),
            // chảy máu (b.Ammo.ChanceToBleed), hiệu ứng va chạm tại hit.point...
        }
    }
}   // hết class BulletManager
