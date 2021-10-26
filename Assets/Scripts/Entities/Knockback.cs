using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Knockback : MonoBehaviour
{
    [Tooltip("Value < 1 for less knockback, value > 1 for more knockback")]
    public float knockbackResistance = 1;

    private Rigidbody rb;
    private NavMeshAgent agent;

    public void Start()
    {
        if(!TryGetComponent(out rb))
        {
            Debug.LogWarning(gameObject.name + " has knockback component but no rigidbody attached");
        }

        TryGetComponent(out agent);
    }

    public void ApplyKnockback(Transform fromTransform, float strength)
    {
        if(rb == null)
        {
            return;
        }

        if(agent != null)
        {
            agent.enabled = false;
            rb.isKinematic = false;
        }

        Debug.Log("Applying knockback force");

        rb.velocity = Vector3.zero;

        rb.AddForce((fromTransform.forward + new Vector3(0f, 0f, 0f)) * strength * knockbackResistance, ForceMode.Impulse);

        if(agent != null)
        {
            StartCoroutine(ResetAgent());
        }
    }

    private IEnumerator ResetAgent()
    {
        yield return new WaitForSeconds(0.05f);

        yield return new WaitUntil(() => IsStopped());

        Debug.Log("Agent is stopped");

        agent.enabled = true;
        rb.isKinematic = true;
    }

    private bool IsStopped()
    {
        return rb.velocity.magnitude < 0.1;
    }
}
