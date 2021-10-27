using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(menuName = "Modules/Weapons/Weapon")]
public class Weapon : ScriptableObject
{
    [Tooltip("Identifier used for linking to database, DO NOT MODIFY")]
    public string uniqueId;

    [Tooltip("The rarity enum of this item")]
    [SerializeField]
    private Rarities rarity;

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

    public bool hasSpinup;

    public float spinupTime;

    public bool canOverheat;

    public float overheatTime;

    public float overheatRecoveryTime;

    [HideInInspector]
    public bool isOverheated;

    private Dictionary<Identifier, Container> map;

    private List<Identifier> ids = new List<Identifier>();

    private class Identifier
    {
        public Attack attack;
        public int id;

        public Identifier(Attack attack, int id)
        {
            this.attack = attack;
            this.id = id;
        }
    }

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

        var count = 0;

        foreach(Attack attack in attacks)
        {
            var id = ids[count];

            // Debug.Log(id.attack + " " + id.id);

            if(!map[id].lastUseTime.ContainsKey(self))
            {
                map[id].lastUseTime.Add(self, 0f);
            }

            var currentTime = Time.time;

            if(map[id].lastUseTime[self] + map[id].cooldown <= currentTime)
            {
                didUseAttack = true;

                if(self.GetPhotonView().IsMine)
                {
                    attack.PerformAttack(self, map[id].delay, targetTag, true, destination);
                }
                
                map[id].lastUseTime[self] = currentTime;
            }
            
            else
            {
                // Debug.Log("On cooldown");
            }

            count++;
        }

        return didUseAttack;
    }

    // Map attacks to cooldowns on serialization
    public void Map()
    {
        if(attacks.Count == cooldowns.Count && !attacks.Contains(null))
        {
            map = new Dictionary<Identifier, Container>();
            for (int i = 0; i < attacks.Count; i++)
            {
                var id = new Identifier(attacks[i], i);
                map.Add(id, new Container(cooldowns[i], delays[i]));
                ids.Add(id);
            }
        }
    }

    public void OnEnable()
    {
        Debug.Log("Mapping " + name);
        Map();
    }

    private Rarities GetRarity()
    {
        return rarity;
    }
}
