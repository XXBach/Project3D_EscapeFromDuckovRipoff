using UnityEngine;
public enum Domain
{
    None = 0,
    Player = 1,
    Enemy = 2
}

public enum PlayerState
{
    Idle = 0,
    Move = 1,
    Sprint = 2,
    Dodge = 3,
    Reload = 4,
    ThirstyAndHungry = 5,
    Dead = 6
}

public enum EnemyState
{
    Idle = 0,
    Patrol = 1,
    Chase = 2,
    Attack = 3,
    Dead = 4
}
