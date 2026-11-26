using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDodgeConfiguration", menuName = "Scriptable Objects/PlayerDodgeConfiguration")]
public class PlayerDodgeConfiguration : ScriptableObject
{
    [SerializeField] private float _dodgeTime;
    [SerializeField] private float _dodgeDistance;
    [SerializeField] private float _iFrameCoefficient;
    [SerializeField] private float _dodgeCooldown;

    public float DodgeTime { get { return _dodgeTime; } }
    public float DodgeDistance { get { return _dodgeDistance; } }
    public float IFrameCoefficient { get { return _iFrameCoefficient; } }
    public float DodgeCooldown { get { return _dodgeCooldown; } }
}
