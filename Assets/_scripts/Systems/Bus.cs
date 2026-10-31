using UnityEngine;
using UnityEngine.Events;

public class Bus<T> where T : struct, IEvent
{
    public static event UnityAction<T> OnEvent;
    public static void Raise(T _event) => OnEvent?.Invoke(_event);
    public static void Raise() => OnEvent?.Invoke(default);
    public static void Reset() => OnEvent = null;
//#if UNITY_EDITOR
//    // Reset khi tắt Domain Reload
//    [UnityEngine.RuntimeInitializeOnLoadMethod(
//        UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
//#endif
}

public struct CharacterDied : IEvent
{
    public readonly CharacterDataManager _character;
    public CharacterDied(CharacterDataManager character) => _character = character;
}
public struct CharacterHungryAndThirsty : IEvent
{
    public readonly CharacterDataManager _character;
    public CharacterHungryAndThirsty(CharacterDataManager character) => _character = character;
}
public struct GameOver : IEvent
{

}
