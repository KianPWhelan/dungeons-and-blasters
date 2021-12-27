using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Com.OfTomorrowInc.DMShooter;
using Fusion;

public class EnemyGeneric : NetworkBehaviour
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

    public Animator animator;

    [Tooltip("The collider which will be used for critical points")]
    public Hitbox critBox;

    public State currentState;

    public float cooldown;

    public int slotSize;

    [Tooltip("-1 is a representative value for no charge limit")]
    public int charges = -1;

    public float visionRange;

    public GameObject spawnSoundEffect;

    public List<float> ranges;

    public float aggroRange = 15;

    public float desiredRange = 0;

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
    public Queue<Vector3> destinationQueue = new Queue<Vector3>();

    [HideInInspector]
    public WeaponHolder weaponHolder;

    [HideInInspector]
    public Squad squad;

    [HideInInspector]
    [Networked]
    public NetworkBool networkedIsFollowing { get; set; }
    private bool predictedIsFollowing;
    public bool isFollowing
    {
        get => Object.IsPredictedSpawn ? predictedIsFollowing : (bool)networkedIsFollowing;
        set { if (Object.IsPredictedSpawn) predictedIsFollowing = value; else networkedIsFollowing = value; }
    }

    [HideInInspector]
    public GameObject followTarget;

    public bool canAggro = true;

    public bool isAggro = false;

    [Networked]
    public NetworkBool networkedMoving { get; set; }
    private bool predictedMoving;
    private bool moving
    {
        get => Object.IsPredictedSpawn ? predictedMoving : (bool)networkedMoving;
        set { if (Object.IsPredictedSpawn) predictedMoving = value; else networkedMoving = value; }
    }

    public void AssignSquad(Squad squad)
    {
        if (this.squad != null)
        {
            this.squad.RemoveUnit(this);
        }

        this.squad = squad;
        squad.AddUnit(this);
    }

    // Start is called before the first frame update
    public override void Spawned()
    {
        TryGetComponent(out weaponHolder);
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

    public override void FixedUpdateNetwork()
    {
        if(!Object.HasStateAuthority)
        {
            return;
        }

        if(statusEffects.GetIsStunned() && agent.isActiveAndEnabled)
        {
            agent.ResetPath();
            //photonView.RPC("SetIsMoving", RpcTarget.All, false);
            moving = false;
            return;
        }

        if(isFollowing)
        { 
            if(followTarget == null)
            {
                CancelFollow();
                canAggro = true;
            }

            else
            {
                canAggro = false;
                MoveTo(followTarget.transform.position);
            }

            if (agent.isActiveAndEnabled && agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                if (agent.remainingDistance <= agent.stoppingDistance && moving)
                {
                    //photonView.RPC("SetIsMoving", RpcTarget.All, false);
                    moving = false;
                }
            }
        }

        if(agent.isActiveAndEnabled && agent.pathStatus == NavMeshPathStatus.PathComplete && !isFollowing)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                //if (!agent.hasPath)
                //{
                    ProcessQueue();
                //}
            }
        }

        agent.speed = startingSpeed * statusEffects.GetMoveSpeedMod();

        target = null;
        target = GetClosestVisible();
        
        // allyTarget = Helpers.FindClosestVisible(gameObject.transform, allyTargetType);
        aiModule.Tick(gameObject, target, allyTarget, agent, movement, this);
    }

    public void MoveTo(Vector3 position)
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            //Debug.Log("Moving");
            agent.SetDestination(position);
            
            if(!moving)
            {
                //photonView.RPC("SetIsMoving", RpcTarget.All, true);
                moving = true;
            }
        }
    }

    public void AddToQueue(Vector3 position)
    {
        if(!Object.HasStateAuthority)
        {
            RPC_AddToQueue(position);
            return;
        }

        //Debug.Log("Adding to queue");
        //if()
        //{
            destinationQueue.Enqueue(position);
        //}
    }

    public void ClearQueue()
    {
        if(!Object.HasStateAuthority)
        {
            RPC_ClearQueue();
            return;
        }

        //Debug.Log("Clearing queue");
        //if(photonView.IsMine)
        //{
            destinationQueue.Clear();
        //}
    }

    public void ClearPath()
    {
        if(!Object.HasStateAuthority)
        {
            RPC_ClearPath();
            return;
        }

        //Debug.Log("Resetting path");
        agent.ResetPath();
        //photonView.RPC("SetIsMoving", RpcTarget.All, false);
        moving = false;
    }

    public void FollowEntity(NetworkObject target)
    {
        if(!Object.HasStateAuthority)
        {
            RPC_FollowEntity(target);
            return;
        }

        //Debug.Log("Following " + target.gameObject.name);
        followTarget = target.gameObject;
        //photonView.RPC("SetIsFollowing", RpcTarget.All, true);
        isFollowing = true;
        moving = true;
    }

    public void CancelFollow()
    {
        if(!Object.HasStateAuthority)
        {
            RPC_CancelFollow();
            return;
        }

        //Debug.Log("Canceling follow");
        //photonView.RPC("SetIsFollowing", RpcTarget.All, false);
        //photonView.RPC("SetIsMoving", RpcTarget.All, false);
        isFollowing = false;
        moving = false;
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_AddToQueue(Vector3 position)
    {
        AddToQueue(position);
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_ClearQueue()
    {
        ClearQueue();
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_ClearPath()
    {
        ClearPath();
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_FollowEntity(NetworkObject target)
    {
        FollowEntity(target);
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_CancelFollow()
    {
        CancelFollow();
    }

    private void ProcessQueue()
    {
        if(destinationQueue.Count > 0 && !isFollowing)
        {
            //Debug.Log("Processing Queue");
            MoveTo(destinationQueue.Dequeue());
        } 

        else if(moving)
        {
            //Debug.Log("Here");
            //photonView.RPC("SetIsMoving", RpcTarget.All, false);
            moving = false;
            canAggro = true;
        }
    }

    private GameObject GetClosestVisible()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aggroRange, LayerMask.GetMask(targetType), QueryTriggerInteraction.Collide);

        GameObject closest = null;
        float dist = Mathf.Infinity;

        foreach(Collider hit in hits)
        {
            if(Helpers.CheckLineOfSight(transform, hit.transform, Mathf.Infinity) && hit.gameObject != closest)
            {
                float thisDist = Vector3.Distance(transform.position, hit.transform.parent.position);

                if(thisDist < dist)
                {
                    dist = thisDist;
                    closest = hit.transform.parent.gameObject;
                }
            }
        }

        return closest;
    }
}
