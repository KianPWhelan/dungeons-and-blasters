using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Modules/AI/Custom AI")]
public class CustomAI : EnemyAI, ISerializationCallbackReceiver
{
    [Tooltip("All states that this AI can use")]
    [SerializeField]
    private List<State> states = new List<State>();

    [Tooltip("Sub AI's which can be used the same as states")]
    [SerializeField]
    private List<EnemyAI> subAis = new List<EnemyAI>();

    [Tooltip("Default state the AI always starts in")]
    [SerializeField]
    private State defaultState;

    [Tooltip("Transition conditions between states (E.G currentState: <current state name> conditions: targetDistance < i => <new state name>)")]
    [TextArea(minLines: 0, maxLines: 5)]
    [SerializeField]
    private List<string> transitions = new List<string>();

    private State currentState;

    [SerializeField]
    public List<StateTransition> stateMachine = new List<StateTransition>();

    // Valid values that can be called from transitions
    private static string[] vals = { "targetDistance", "targetIsVisible" };
    private List<string> callable = new List<string>(vals);

    // Valid conditions that can be called from transitions
    private static string[] cond = { "==", "<", ">", "<=", ">=", "&&", "||" };
    private List<string> conditionals = new List<string>(cond);

    // Current valid state transitions
    private List<StateTransition> releventTransitions;

    private GameObject localTarget;
    private GameObject localSelf;
    private NavMeshAgent localAgent;
    private Movement localMovement;

    [System.Serializable]
    public class StateTransition
    {
        public State fromState;
        public State toState;
        public List<string> transitionComponents = new List<string>();
    }

    public void OnAfterDeserialize()
    { }

    public void OnBeforeSerialize() 
    {
        callable = new List<string>(vals);
        conditionals = new List<string>(cond);
    }

    public override void Tick(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
        localSelf = self;
        localTarget = target;
        localAgent = agent;
        localMovement = movement;

        if(!CheckStateChanges())
        {
            currentState.Tick(self, target, agent, movement);
        }
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
                if(components[i] == "currentState:")
                {
                    stateTransition.fromState = states.Find(x => x.name == components[i + 1]);
                    if(stateTransition.fromState == null)
                    {
                        Debug.LogError("Could not find fromState " + components[i] + " in list of states for transition " + transition);
                    }
                }

                else if(i > 2 && components[i] != "=>")
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

    public bool ProcessConditionals(List<string> transitionComponents)
    {
        List<string> conditions = new List<string>(transitionComponents);
        int i = 0;
        int infiniteLoopDetector = 0;
        while(conditions.Count > 1)
        {
            if(infiniteLoopDetector > 10000)
            {
                Debug.LogError("Infinite loop detected processing conditionals");
                foreach(string condition in conditions)
                {
                    Debug.Log(condition);
                }
                return false;
            }

            if(i >= conditions.Count)
            {
                i = 0;
            }

            // Debug.Log("Current condition: " + conditions[i]);

            // If current index is a callable value, process that value
            if (callable.Contains(conditions[i]))
            {
                // Debug.Log("Setting value");
                //Debug.Log(conditions[i]);
                conditions[i] = GetValue(conditions[i]);
                //Debug.Log(conditions[i]);
            }
            
            // If current index is a conditional and the previous and next values are processed, perform comparison
            else if(conditionals.Contains(conditions[i]) && !callable.Contains(conditions[i - 1]) && !callable.Contains(conditions[i + 1]) && conditionals[i] != "(" && conditionals[i] != ")")
            {
                // Debug.Log("Processing " + conditions[i]);
                var result = Compare(conditions[i - 1], conditions[i + 1], conditions[i]);
                conditions[i - 1] = result.ToString();
                conditions.RemoveRange(i, 2);
            }

            else if(conditions[i] == "(")
            {
                // Debug.Log("Processing sub condition: ");
                List<string> subCondition = new List<string>();
                int count = i + 1;
                int numSubs = 1;

                while(conditions[count] != ")" || numSubs > 1)
                {
                    if(conditions[count] == "(")
                    {
                        numSubs += 1;
                    }

                    else if(conditions[count] == ")")
                    {
                        numSubs -= 1;
                    }

                    subCondition.Add(conditions[count]);
                    count += 1;
                }

                foreach(string condition in subCondition)
                {
                    Debug.Log(condition);
                }

                conditions[i] = ProcessConditionals(subCondition).ToString();
                // Debug.Log("Up one level");
                conditions.RemoveRange(i + 1, count - i);
            }

            i += 1;
            infiniteLoopDetector += 1;
            var current = string.Join(" ", conditions);
            // Debug.Log("Current full: " + current);
            // Debug.Log("Length: " + conditions.Count);
        }

        // Debug.Log(conditions.Count);
        // Debug.Log(conditions[0]);
        return bool.Parse(conditions[0]);
    }

    private bool CheckStateChanges()
    {
        if(currentState == null)
        {
            currentState = defaultState;
            currentState.OnEnter(localSelf, localTarget, localAgent, localMovement);
            releventTransitions = GetReleventTransitions(currentState);
            return true;
        }

        foreach(StateTransition transition in releventTransitions)
        {
            if(ProcessConditionals(transition.transitionComponents))
            {
                currentState.OnExit(localSelf, localTarget, localAgent, localMovement);
                currentState = transition.toState;
                currentState.OnEnter(localSelf, localTarget, localAgent, localMovement);
                releventTransitions = GetReleventTransitions(currentState);
                return true;
            }
        }

        return false;
    }

    private List<StateTransition> GetReleventTransitions(State state)
    {
        var list = stateMachine.FindAll(x => x.fromState.name == state.name);
        
        if(list.Count == 0)
        {
            Debug.LogError("Did not find relevent state transitions for state " + state.name);
        }

        return list;
    }

    private bool Compare(string val1, string val2, string condition)
    {
        float num1, num2;

        // Check if values are numeric
        if(float.TryParse(val1, out num1) && float.TryParse(val2, out num2))
        {
            Debug.Log(num1);
            Debug.Log(num2);
            if(condition == "==")
            {
                return num1 == num2;
            }

            else if(condition == "<")
            {
                return num1 < num2;
            }

            else if(condition == ">")
            {
                return num1 > num2;
            }

            else if(condition == "<=")
            {
                return num1 <= num2;
            }

            else if(condition == ">=")
            {
                return num1 >= num2;
            }
        }

        // If values are not numeric, the only valid conditionals are ==, &&, and ||
        if(condition == "==")
        {
            return val1 == val2;
        }

        else if(condition == "&&" && val1 == "True" && val2 == "True")
        {
            return true;
        }

        else if(condition == "||" && (val1 == "True" || val2 == "True"))
        {
            return true;
        }

        return false;
    }

    private string GetValue(string variable)
    {
        if(variable == "targetDistance")
        {
            return Vector3.Distance(localSelf.transform.position, localTarget.transform.position).ToString();
        }

        else if(variable == "targetIsVisible")
        {
            if(!localTarget)
            {
                return "False";
            }

            return Helpers.CheckLineOfSight(localSelf.transform, localTarget.transform).ToString();
        }

        return "None";
    }
}
