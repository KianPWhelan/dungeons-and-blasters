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

    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        movement = gameObject.GetComponent<Movement>();
        Debug.Log(agent.ToString());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        target = Helpers.FindClosest(gameObject.transform, targetType);
        aiModule.Tick(gameObject, target, agent, movement);
    }
}
