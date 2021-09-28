#region Description
// -----------------------------------------------------------------------------------------
// Game event listener monobehaviour that utilizes Unity's built in event system and can be attatched to a game object
// -----------------------------------------------------------------------------------------
#endregion

using UnityEngine;
using UnityEngine.Events;

public class UnityGameEventListener : MonoBehaviour, IGameEventListener
{
    [Tooltip("Event to register with")]
    [SerializeField]
    private GameEvent @event;

    [Tooltip("Response to invoke when event is raised")]
    [SerializeField]
    private UnityEvent response;

    public void OnEnable()
    {
        if(@event != null)
        {
            @event.RegisterListener(this);
        }
    }

    public void OnDisable()
    {
        @event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        response?.Invoke();
    }
}
