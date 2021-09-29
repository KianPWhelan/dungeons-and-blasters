using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderTrigger : MonoBehaviour
{
    [Tooltip("If true, collider can only be triggered once")]
    [SerializeField]
    private bool triggerOnce;

    private bool hasBeenTriggered;

    [Tooltip("Game event to fire when triggered, leave empty for anything")]
    [SerializeField]
    private GameEvent gameEvent;

    [Tooltip("Tags that can trigger the game event")]
    [SerializeField]
    private List<string> tags = new List<string>();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
        if(gameEvent == null)
        {
            Debug.LogWarning("No event set");
        }

        if(tags.Count == 0 && (!triggerOnce || !hasBeenTriggered))
        {
            gameEvent.Raise();
            hasBeenTriggered = true;
        }
        
        else if(tags.Contains(other.tag) && (!triggerOnce || !hasBeenTriggered))
        {
            gameEvent.Raise();
            hasBeenTriggered = true;
        }
    }
}
