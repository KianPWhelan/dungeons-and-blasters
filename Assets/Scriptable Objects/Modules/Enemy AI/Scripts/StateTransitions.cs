using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateTransition
{
    public State fromState;
    public State toState;
    public List<string> transitionComponents = new List<string>();
}
