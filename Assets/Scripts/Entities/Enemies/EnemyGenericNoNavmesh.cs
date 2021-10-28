using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGenericNoNavmesh : EnemyGeneric
{
    public bool findTarget;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out statusEffects);
        if (spawnSoundEffect != null)
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
        if(findTarget)
        {
            target = Helpers.FindClosest(gameObject.transform, targetType);
        }    

        else
        {
            target = null;
        }
        
        aiModule.Tick(gameObject, target, null, null, null);
    }
}
