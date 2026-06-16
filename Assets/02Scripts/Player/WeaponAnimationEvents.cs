using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : UnityEvent<string>
{

}
public class WeaponAnimationEvents : MonoBehaviour
{
    public AnimationEvent WeaponAnimEvent = new AnimationEvent();
    public void OnAnimationEvent(string p_eventName)
    {
        WeaponAnimEvent.Invoke(p_eventName);
    }
}
