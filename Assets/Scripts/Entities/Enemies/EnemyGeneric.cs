using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Movement), typeof(NavMeshAgent))]
public class EnemyGeneric : MonoBehaviour
{
    public Rarities rarity;

    [Tooltip("Identifier used for linking to database, DO NOT MODIFY")]
    public string uniqueId;

    [Tooltip("The module this enemy will use")]
    [SerializeField]
    public EnemyAI aiModule;

    [Tooltip("Type of target this enemy will seek")]
    [SerializeField]
    public string targetType;

    [Tooltip("Type of target to be considered an ally")]
    public string allyTargetType;

    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public GameObject allyTarget;
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public Movement movement;
    [HideInInspector]
    public float timeSinceLastStateChange = 0;

    [Tooltip("Where homing projectiles will attempt to aim at, should be an empty child object of enemy")]
    public GameObject homingPoint;

    [Tooltip("The collider which will be used for critical points")]
    public Collider critBox;

    public State currentState;

    public float cooldown;

    public int slotSize;

    public float visionRange;

    public GameObject spawnSoundEffect;

    public List<float> ranges;

    // Current valid state transitions
    // [HideInInspector]
    public List<StateTransition> releventTransitions;

    [HideInInspector]
    public float startingSpeed;

    [HideInInspector]
    public StatusEffects statusEffects;

    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        movement = gameObject.GetComponent<Movement>();
        Debug.Log(agent.ToString());
        startingSpeed = agent.speed;
        TryGetComponent(out statusEffects);

        if(spawnSoundEffect != null)
        {
            var m_Sound = Instantiate(spawnSoundEffect, transform.position, Quaternion.identity);
            var m_Source = m_Sound.GetComponent<AudioSource>();
            float life = m_Source.clip.length / m_Source.pitch;
            Destroy(m_Sound, life);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //target = Helpers.FindClosest(gameObject.transform, targetType);
        //aiModule.Tick(gameObject, target, agent, movement);
    }

    private void Update()
    {
        if(statusEffects.GetIsStunned())
        {
            agent.ResetPath();
            return;
        }

        agent.speed = startingSpeed * statusEffects.GetMoveSpeedMod();
        target = Helpers.FindClosestVisible(gameObject.transform, targetType);
        allyTarget = Helpers.FindClosestVisible(gameObject.transform, allyTargetType);
        aiModule.Tick(gameObject, target, allyTarget, agent, movement);
    }
}
