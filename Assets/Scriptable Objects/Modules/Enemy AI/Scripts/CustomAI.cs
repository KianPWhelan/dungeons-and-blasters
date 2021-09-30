using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Modules/AI/Custom AI")]
public class CustomAI : EnemyAI
{
    [Tooltip("All states that this AI can use")]
    [SerializeField]
    private List<State> states = new List<State>();

    [Tooltip("Default state the AI always starts in")]
    [SerializeField]
    private State defaultState;

    [Tooltip("Transition conditions between states (E.G currentState == <current state name> conditions: targetDistance < i => <new state name>)")]
    [TextArea(minLines: 0, maxLines: 5)]
    [SerializeField]
    private List<string> transitions = new List<string>();

    private State currentState;

    [SerializeField]
    private List<StateTransition> stateMachine = new List<StateTransition>();

    [System.Serializable]
    private class StateTransition
    {
        public State fromState;
        public State toState;
        public List<string> transitionComponents = new List<string>();
    }

    //public void OnAfterDeserialize()
    //{
    //    stateMachine = new List<StateTransition>();
    //    BuildStateMachine();
    //}

    //public void OnBeforeSerialize() { }

    public override void Tick(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
        throw new System.NotImplementedException();
    }

    public void BuildStateMachine()
    {
        stateMachine = new List<StateTransition>();
        foreach(string transition in transitions)
        {
            var components = transition.Split(' ');
            StateTransition stateTransition = new StateTransition();
            for(int i = 0; i < components.Length; i++)
            {
                if(i == 2 && components[i - 2] == "currentState")
                {
                    stateTransition.fromState = states.Find(x => x.name == components[i]);
                    if(stateTransition.fromState == null)
                    {
                        Debug.LogError("Could not find fromState " + components[i] + " in list of states for transition " + transition);
                    }
                }

                else if(i > 3 && components[i] != "=>")
                {
                    stateTransition.transitionComponents.Add(components[i]);
                }

                else if(components[i] == "=>")
                {
                    stateTransition.toState = states.Find(x => x.name == components[i + 1]);
                    if(stateTransition.toState == null)
                    {
                        Debug.LogError("Could not find toState " + components[i] + " in list of states for transition " + transition);
                    }
                    break;
                }
            }
            Debug.Log("From State: " + stateTransition.fromState.name + "  To State: " + stateTransition.toState.name + "  Condition: ");
            foreach(string component in stateTransition.transitionComponents)
            {
                Debug.Log(component);
            }
            stateMachine.Add(stateTransition);
        }
    }
}
