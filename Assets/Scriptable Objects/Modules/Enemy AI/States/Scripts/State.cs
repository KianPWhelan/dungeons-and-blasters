using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[System.Serializable]
public abstract class State : ScriptableObject
{
    public abstract string name
    {
        get;
    }

    public abstract void Tick(GameObject self, GameObject target, NavMeshAgent agent, Movement movement);

    public abstract void OnEnter(GameObject self, GameObject target, NavMeshAgent agent, Movement movement);

    public abstract void OnExit(GameObject self, GameObject target, NavMeshAgent agent, Movement movement);

    public void SetIsAgentMovingAnimation(GameObject self, NavMeshAgent agent)
    {
        if (agent.remainingDistance > 1)
        {
            var animator = self.GetComponentInChildren<Animator>();

            if (animator != null)
            {
                animator.SetBool("isMoving", true);
            }
        }

        else
        {
            var animator = self.GetComponentInChildren<Animator>();

            if (animator != null)
            {
                animator.SetBool("isMoving", false);
            }
        }
    }

    public void SetAttackAnimationTrigger(GameObject self)
    {
        var animator = self.GetComponentInChildren<Animator>();

        if (animator != null)
        {
            animator.SetTrigger("attack");
        }
    }

    public void SetAttackAnimationTrigger(GameObject self, int index)
    {
        var animator = self.GetComponentInChildren<Animator>();

        if (animator != null)
        {
            animator.SetTrigger("attack" + index);
        }
    }
}
