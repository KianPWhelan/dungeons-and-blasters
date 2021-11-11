using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Com.OfTomorrowInc.DMShooter;

public class EnemyGeneric : MonoBehaviour
{
    public Rarities rarity;

    public SizeClasses size;

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

    [SerializeField]
    public GameObject selectionHighlight;

    [Tooltip("The collider which will be used for critical points")]
    public Collider critBox;

    public State currentState;

    public float cooldown;

    public int slotSize;

    [Tooltip("-1 is a representative value for no charge limit")]
    public int charges = -1;

    public float visionRange;

    public GameObject spawnSoundEffect;

    public List<float> ranges;

    public float aggroRange = 15;

    // Current valid state transitions
    // [HideInInspector]
    public List<StateTransition> releventTransitions;

    [HideInInspector]
    public float startingSpeed;

    [HideInInspector]
    public StatusEffects statusEffects;

    public bool doNotUseAsAbility;

    public bool slottable;

    [HideInInspector]
    public bool moving;

    [HideInInspector]
    public Queue<Vector3> destinationQueue = new Queue<Vector3>();

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
        if(statusEffects.GetIsStunned() && agent.isActiveAndEnabled)
        {
            agent.ResetPath();
            moving = false;
            return;
        }

        if(agent.isActiveAndEnabled && agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                //if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                //{
                    ProcessQueue();
                //}
            }
        }

        agent.speed = startingSpeed * statusEffects.GetMoveSpeedMod();

        // target = GetClosestVisible();
        
        // allyTarget = Helpers.FindClosestVisible(gameObject.transform, allyTargetType);
        // aiModule.Tick(gameObject, target, allyTarget, agent, movement);
    }

    public void MoveTo(Vector3 position)
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            //Debug.Log("Moving");
            agent.SetDestination(position);
            moving = true;
        }
    }

    public void AddToQueue(Vector3 position)
    {
        destinationQueue.Enqueue(position);
    }

    public void ClearQueue()
    {
        destinationQueue.Clear();
    }

    private void ProcessQueue()
    {
        if(destinationQueue.Count > 0)
        {
            //Debug.Log("Processing Queue");
            MoveTo(destinationQueue.Dequeue());
        } 

        else if(moving)
        {
            moving = false;
        }
    }

    private GameObject GetClosestVisible()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aggroRange, LayerMask.GetMask(targetType));

        GameObject closest = null;
        float dist = Mathf.Infinity;

        foreach(Collider hit in hits)
        {
            if(Helpers.CheckLineOfSight(transform, hit.transform, Mathf.Infinity) && hit.gameObject != closest)
            {
                float thisDist = Vector3.Distance(transform.position, hit.transform.position);

                if(thisDist < dist)
                {
                    dist = thisDist;
                    closest = hit.gameObject;
                }
            }
        }

        return closest;
    }
}
