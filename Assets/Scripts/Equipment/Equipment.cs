using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Equipment : MonoBehaviour
{
    [Tooltip("Added to the default hp")]
    public float hpMod;

    [Tooltip("Added to default speed")]
    public float speedMod;

    public List<Effect> effects = new List<Effect>();

    private Health health;
    private StatusEffects statusEffects;
    private NetworkCharacterController cc;
    private EnemyGeneric e;

    // Start is called before the first frame update
    void Awake()
    {
        transform.parent.TryGetComponent(out health);
        transform.parent.TryGetComponent(out statusEffects);
        transform.parent.TryGetComponent(out e);
        transform.parent.TryGetComponent(out cc);
    }

    public void Activate()
    {
        ApplyStatChanges();
        ApplyEffects();
    }

    private void ApplyStatChanges()
    {
        if(health != null)
        {
            Debug.Log("Adding " + hpMod + " to health");
            health.SetHealth(health.GetHealth() + hpMod);
        }

        if(cc != null)
        {
            Debug.Log("Adding " + speedMod + " to speed");
            cc.MaxSpeed += speedMod;
            transform.parent.TryGetComponent(out PlayerMovement p);

            if(p != null)
            {
                p.startingSpeed = cc.MaxSpeed;
            }

            Debug.Log(cc.MaxSpeed);
        }

        if(e != null)
        {
            Debug.Log("Adding " + speedMod + " to speed");
            e.agent.speed += speedMod;
            e.startingSpeed = e.agent.speed;
        }
    }

    private void ApplyEffects()
    {
        if(statusEffects != null)
        {
            foreach(Effect effect in effects)
            {
                effect.ApplyEffect(gameObject, health, statusEffects, transform.position, transform.rotation, tag, 1);
            }
        }
    }
}
