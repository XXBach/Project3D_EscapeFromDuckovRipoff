using UnityEngine;
public enum Domain
{
    None = 0,
    Player = 1,
    Enemy = 2
}

public enum PlayerState
{
    Normal = 0,
    Dodge = 1,
    Interacting = 2,
    Looting = 3,
    Dead = 4
}

public enum EnemyState
{
    Idle = 0,
    Patrol = 1,
    Chase = 2,
    Attack = 3,
    Dead = 4
}
