using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Weapons/Weapon")]
public class Weapon : ScriptableObject
{
    [Tooltip("Attacks are they key, and cooldown is the value")]
    [SerializeField]
    private List<Attack> attacks = new List<Attack>();

    [Tooltip("Cooldowns for attacks (must be in same order as above)")]
    [SerializeField]
    private List<float> cooldowns = new List<float>();

    private static Dictionary<Attack, Container> map;

    private class Container
    {
        public float cooldown;
        public float lastUseTime;

        public Container(float cooldown)
        {
            this.cooldown = cooldown;
            lastUseTime = 0;
        }
    }

    /// <summary>
    /// Uses each attack this weapon has if that attack is off cooldown
    /// </summary>
    /// <param name="self"></param>
    /// <param name="targetTag"></param>
    public void Use(GameObject self, string targetTag = "none")
    {
        Debug.Log("Using weapon " + this.name);
        foreach(Attack attack in attacks)
        {
            var currentTime = Time.time;
            if(map[attack].lastUseTime + map[attack].cooldown <= currentTime)
            {
                attack.PerformAttack(self, targetTag);
                map[attack].lastUseTime = currentTime;
            }
            
            else
            {
                Debug.Log("On cooldown");
            }
        }
    }

    // Map attacks to cooldowns on serialization
    public void Map()
    {
        if(attacks.Count == cooldowns.Count && !attacks.Contains(null))
        {
            map = new Dictionary<Attack, Container>();
            for (int i = 0; i < attacks.Count; i++)
            {
                map.Add(attacks[i], new Container(cooldowns[i]));
            }
        }
    }

    public void OnEnable()
    {
        if (map == null)
        {
            Map();
        }
    }
}
