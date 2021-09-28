#region Description
// -----------------------------------------------------------------------------------------
// Game Event class which can be added through the assets menu and attatched to an object to be triggered
// -----------------------------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Events/Game Event")]
public class GameEvent : ScriptableObject
{
    /// <summary>
    /// List of the listeners which will be notified when this game event is triggered
    /// </summary>
    private readonly List<IGameEventListener> eventListeners = new List<IGameEventListener>();

    /// <summary>
    /// Sends message to all subscribed game event listeners that this event has been triggered
    /// </summary>
    public void Raise()
    {
        foreach(IGameEventListener i in eventListeners)
        {
            i.OnEventRaised();
        }
    }

    /// <summary>
    /// Subscribes the provided game event listener to this game event if it is not already subscribed
    /// </summary>
    public void RegisterListener(IGameEventListener listener)
    {
        if(!eventListeners.Contains(listener))
        {
            eventListeners.Add(listener);
        }
    }

    /// <summary>
    /// Unsubscribes the provided game event listener from this game event if it is currently subscribed
    /// </summary>
    /// <param name="listener"></param>
    public void UnregisterListener(IGameEventListener listener)
    {
        if(eventListeners.Contains(listener))
        {
            eventListeners.Remove(listener);
        }
    }
}
