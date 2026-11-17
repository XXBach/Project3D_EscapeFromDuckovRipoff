using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Enemy Managers")]
    [SerializeField] private CharacterDataManager _enemyDataManager;
    [SerializeField] private NavMeshAgent _enemyNavMeshAgent;

    private EnemyState _currentEnemyState;
    private EnemyState _previousEnemyState;
    private float _enemySpeed;

    private void Awake()
    {
        _enemySpeed = _enemyDataManager.GetCurrentSpeed();
        _currentEnemyState = EnemyState.Idle;
        _previousEnemyState = EnemyState.Idle;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemyNavMeshAgent.speed = _enemySpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(_currentEnemyState == EnemyState.Idle)
        {

        }
    }
}
