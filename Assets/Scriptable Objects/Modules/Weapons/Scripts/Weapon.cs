using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(menuName = "Modules/Weapons/Weapon")]
public class Weapon : ScriptableObject
{
    [Tooltip("Attacks are they key, and cooldown is the value")]
    [SerializeField]
    private List<Attack> attacks = new List<Attack>();

    [Tooltip("Cooldowns for attacks (must be in same order as above)")]
    [SerializeField]
    private List<float> cooldowns = new List<float>();

    [Tooltip("Delays for attacks (must be in same order as above)")]
    [SerializeField]
    private List<float> delays = new List<float>();

    [Tooltip("Animation to play when weapon is used")]
    [SerializeField]
    private Animation animation;


    private Dictionary<Attack, Container> map;

    private class Container
    {
        public float cooldown;
        public float delay;
        public Dictionary<GameObject, float> lastUseTime = new Dictionary<GameObject, float>();

        public Container(float cooldown, float delay)
        {
            this.cooldown = cooldown;
            this.delay = delay;
        }
    }

    /// <summary>
    /// Uses each attack this weapon has if that attack is off cooldown
    /// </summary>
    /// <param name="self"></param>
    /// <param name="targetTag"></param>
    public bool Use(GameObject self, string targetTag = "none", Vector3? destination = null)
    {
        // Debug.Log("Using weapon " + this.name);
        bool didUseAttack = false;

        if(animation != null)
        {
            animation.Play();
        }
        
        foreach(Attack attack in attacks)
        {
            if(!map[attack].lastUseTime.ContainsKey(self))
            {
                map[attack].lastUseTime.Add(self, 0f);
            }

            var currentTime = Time.time;

            if(map[attack].lastUseTime[self] + map[attack].cooldown <= currentTime)
            {
                didUseAttack = true;

                if(self.GetPhotonView().IsMine)
                {
                    attack.PerformAttack(self, map[attack].delay, targetTag, true, destination);
                }
                
                map[attack].lastUseTime[self] = currentTime;
            }
            
            else
            {
                // Debug.Log("On cooldown");
            }
        }

        return didUseAttack;
    }

    // Map attacks to cooldowns on serialization
    public void Map()
    {
        if(attacks.Count == cooldowns.Count && !attacks.Contains(null))
        {
            map = new Dictionary<Attack, Container>();
            for (int i = 0; i < attacks.Count; i++)
            {
                map.Add(attacks[i], new Container(cooldowns[i], delays[i]));
            }
        }
    }

    public void OnEnable()
    {
        Debug.Log("Mapping " + name);
        Map();
    }
}
