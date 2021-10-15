using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Movement), typeof(NavMeshAgent))]
public class EnemyGeneric : MonoBehaviour
{
    [Tooltip("The module this enemy will use")]
    [SerializeField]
    private EnemyAI aiModule;

    [Tooltip("Type of target this enemy will seek")]
    [SerializeField]
    private string targetType;

    private GameObject target;
    private NavMeshAgent agent;
    private Movement movement;

    public State currentState;

    public float cooldown;

    public int slotSize;

    public List<float> ranges;

    // Current valid state transitions
    // [HideInInspector]
    public List<StateTransition> releventTransitions;

    private float startingSpeed;

    private StatusEffects statusEffects;

    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        movement = gameObject.GetComponent<Movement>();
        Debug.Log(agent.ToString());
        startingSpeed = agent.speed;
        TryGetComponent(out statusEffects);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //target = Helpers.FindClosest(gameObject.transform, targetType);
        //aiModule.Tick(gameObject, target, agent, movement);
    }

    private void Update()
    {
        agent.speed = startingSpeed * statusEffects.GetMoveSpeedMod();
        target = Helpers.FindClosest(gameObject.transform, targetType);
        aiModule.Tick(gameObject, target, agent, movement);
    }
}
