using UnityEngine;
using UnityEngine.Events;
public class HandgunBehavior : MonoBehaviour, ILethality
{
    [Header("Prefabs")]
    [SerializeField] private Ammo _usingBullet;
    [SerializeField] private Handgun _usingHandgun;

    [Header("AudioClips")]
    [SerializeField] private AudioClip _reloadSound;
    [SerializeField] private AudioClip _shootingSound;
    
    [Header("Events")]    
    public UnityEvent OnShoot;
    public UnityEvent OnReload;

    private void Awake()
    {
        _usingBullet ??= gameObject.GetComponent<Ammo>();
        _usingHandgun ??= gameObject.GetComponent<Handgun>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
