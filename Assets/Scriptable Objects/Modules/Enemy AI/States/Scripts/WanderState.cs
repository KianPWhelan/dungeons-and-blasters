using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "States/WanderState")]
public class WanderState : State
{
    public override string name
    {
        get
        {
            return "WanderState";
        }
    }

    [Tooltip("Frequency with which enemy chooses a wander location")]
    [SerializeField]
    private float wanderFrequency;

    [Tooltip("Range from starting location can wander")]
    [SerializeField]
    private float wanderRange;

    private Dictionary<GameObject, Container> info = new Dictionary<GameObject, Container>();

    private class Container
    {
        public GameObject obj;
        public List<GameObject> validTiles;
        public float lastTime;

        public Container(GameObject obj)
        {
            this.obj = obj;
            lastTime = 0f;
        }
    }

    public override void OnEnter(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
        Debug.Log("Enemy " + self.GetInstanceID() + " has entered " + name);

        if (info.ContainsKey(self))
        {
            info[self].validTiles = Helpers.FindAllInRange(self.transform, wanderRange, "Ground");
        }

        else
        {
            info.Add(self, new Container(self));
            info[self].validTiles = Helpers.FindAllInRange(self.transform, wanderRange, "Ground");
        }

        Debug.Log("There are " + info[self].validTiles.Count + " valid tiles");
    }

    public override void OnExit(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
        agent.ResetPath();
    }

    public override void Tick(GameObject self, GameObject target, NavMeshAgent agent, Movement movement)
    {
        var currentTime = Time.time;

        if(info[self].lastTime + wanderFrequency <= currentTime)
        {
            info[self].lastTime = currentTime;
            var targetTile = info[self].validTiles[Random.Range(0, info[self].validTiles.Count)];
            Debug.Log("Enemy is seeking tile" + targetTile.GetInstanceID());
            agent.SetDestination(targetTile.transform.position);
        }

        SetIsAgentMovingAnimation(self, agent);
    }
}
