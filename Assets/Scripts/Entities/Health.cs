using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using SnazzlebotTools.ENPCHealthBars;

public class Health : MonoBehaviourPunCallbacks
{
    [Tooltip("FloatVariable reference for health point (Leave blank to use normal float)")]
    private FloatVariable health;

    [Tooltip("Normal float, does not exist outside of behavior")]
    [SerializeField]
    private float floatHealth;

    [Tooltip("Entity has infinite health")]
    [SerializeField]
    private bool infiniteHealth = false;

    [Tooltip("Is this component attached to a player?")]
    [SerializeField]
    private bool isPlayer = false;

    [Tooltip("The sound effect to be played when damage is taken")]
    [SerializeField]
    private GameObject hitSoundEffect;

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

    public void Start()
    {
        TryGetComponent(out healthBar);

        if(isPlayer && Controller.LocalPlayerInstance != null && transform == Controller.LocalPlayerInstance.transform)
        {
            Destroy(healthBar);
            healthBar = null;
        }
        
        if(health != null)
        {
            startingHealth = health.initialValue;
        }

        else
        {
            startingHealth = floatHealth;
        }


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
        float statusMod = 1;
        float resistanceMod = 1;

        if(amount < 0)
        {
            PlayHitSound();
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

        if(health != null)
        {
            health.runtimeValue += amount * statusMod * resistanceMod;
            if(health.runtimeValue <= 0)
            {
                isDead = true;
            }

            if (healthBar != null)
            {
                healthBar.Value = (int)health.runtimeValue;
            }
        }

        else
        {
            floatHealth += amount * statusMod * resistanceMod;
            if(floatHealth <= 0)
            {
                isDead = true;

                if(photonView.IsMine)
                {
                    if(isPlayer)
                    {
                        GetComponent<Controller>().GameOver();
                    }

                    else
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }            
                }
            }

            if (healthBar != null)
            {
                healthBar.Value = (int)floatHealth;
            }
        }
    }

    public float GetHealth()
    {
        if(infiniteHealth)
        {
            return Mathf.Infinity;
        }

        if(health != null)
        {
            return health.runtimeValue;
        }

        return floatHealth;
    }

    public void SetHealth(float value)
    {
        if (health != null)
        {
            health.initialValue = value;
            health.runtimeValue = value;
        }

        else
        {
            floatHealth = value;
        }
    }

    private void PlayHitSound()
    {
        if(hitSoundEffect == null)
        {
            return;
        }

        var m_Sound = Instantiate(hitSoundEffect, gameObject.transform.position, Quaternion.identity);
        var m_Source = m_Sound.GetComponent<AudioSource>();
        float life = m_Source.clip.length / m_Source.pitch;
        Destroy(m_Sound, life);
    }
}
