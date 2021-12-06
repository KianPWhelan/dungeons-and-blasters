using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using SnazzlebotTools.ENPCHealthBars;

public class Health : NetworkBehaviour
{
    [SerializeField]
    [Networked]
    private float health { get; set; }

    [Tooltip("Entity has infinite health")]
    [SerializeField]
    private bool infiniteHealth = false;

    [Tooltip("Is this component attached to a player?")]
    [SerializeField]
    private bool isPlayer = false;

    [Tooltip("The sound effect to be played when damage is taken")]
    [SerializeField]
    private GameObject hitSoundEffect;

    [Tooltip("Hit screen effect for player object")]
    [SerializeField]
    private GameObject hitEffect;

    [Tooltip("Damage types that this entity will take less/more damage from (use a multiplier > 1 to make something weaker to a certain damage type)")]
    [SerializeField]
    private List<ResistanceContainer> resistances = new List<ResistanceContainer>();

    public Dictionary<DamageType, float> resistanceStorage = new Dictionary<DamageType, float>();

    [System.Serializable]
    public class ResistanceContainer
    {
        public DamageType damageType;
        public float resistanceMod;

        public ResistanceContainer(DamageType damageType, float resistanceMod)
        {
            this.damageType = damageType;
            this.resistanceMod = resistanceMod;
        }
    }

    public bool isDead = false;

    private ENPCHealthBar healthBar;

    [HideInInspector]
    public float startingHealth;

    public override void Spawned()
    {
        TryGetComponent(out healthBar);

        if(isPlayer && Controller.LocalPlayerInstance != null && transform == Controller.LocalPlayerInstance.transform)
        {
            Destroy(healthBar);
            healthBar = null;
        }
        
        startingHealth = health;

        if(healthBar != null)
        {
            healthBar.MaxValue = (int)startingHealth;
            healthBar.Value = (int)startingHealth;
        }

        if(healthBar != null && Controller.LocalPlayerInstance != null)
        {
            healthBar.FaceCamera = Controller.LocalPlayerInstance.GetComponent<Controller>().playerCam.GetComponent<Camera>();
        }

        else if(healthBar != null && DungeonMasterController.LocalPlayerInstance != null)
        {
            healthBar.FaceCamera = DungeonMasterController.LocalPlayerInstance.GetComponent<DungeonMasterController>().camera;
        }

        foreach(ResistanceContainer resistance in resistances)
        {
            resistanceStorage.Add(resistance.damageType, resistance.resistanceMod);
        }
    }

    /// <summary>
    /// Adjusts health value by amount provided, can be negative
    /// </summary>
    /// <param name="amount"></param>
    public void AdjustHealth(float amount, DamageType damageType = null)
    {
        if(!Object.HasStateAuthority)
        {
            return;
        }

        float statusMod = 1;
        float resistanceMod = 1;

        if(amount < 0)
        {
            PlayHitEffects();
        }

        if(infiniteHealth)
        {
            return;
        }

        var statusEffects = GetComponent<StatusEffects>();

        if(statusEffects != null)
        {
            statusMod = statusEffects.GetDamageRecievedMod();
        }

        if(damageType != null && resistanceStorage.ContainsKey(damageType))
        {
            resistanceMod = resistanceStorage[damageType];
            resistanceMod *= statusEffects.GetResistanceMod(damageType);
        }

        health += amount * statusMod * resistanceMod;
        if(health <= 0)
        {
            isDead = true;

            if(isPlayer)
            {
                //TODO: player death
                //GetComponent<Controller>().GameOver();
            }

            else
            {
                Runner.Despawn(Object);
            }            
        }

        if (healthBar != null && Object != null)
        {
            RPC_UpdateHealthBar(health);
        }
    }

    public float GetHealth()
    {
        if(infiniteHealth)
        {
            return Mathf.Infinity;
        }

        return health;
    }

    public void SetHealth(float value)
    {
        health = value;
    }

    private void PlayHitEffects()
    {
        if(hitSoundEffect == null)
        {
            return;
        }

        if(isPlayer)
        {
            hitEffect.SetActive(true);
            hitEffect.GetComponent<HitFade>().StartFade();
        }

        var m_Sound = Instantiate(hitSoundEffect, gameObject.transform.position, Quaternion.identity);
        var m_Source = m_Sound.GetComponent<AudioSource>();
        float life = m_Source.clip.length / m_Source.pitch;
        Destroy(m_Sound, life);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void RPC_UpdateHealthBar(float health)
    {
        healthBar.Value = (int)health;
    }
}
