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

    public bool isDead = false;

    private ENPCHealthBar healthBar;
    private float startingHealth;

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
    }

    /// <summary>
    /// Adjusts health value by amount provided, can be negative
    /// </summary>
    /// <param name="amount"></param>
    public void AdjustHealth(float amount)
    {
        float mod = 1;

        if(infiniteHealth)
        {
            return;
        }

        if(amount > 0)
        {
            var statusEffects = GetComponent<StatusEffects>();

            if(statusEffects != null)
            {
                mod = statusEffects.GetDamageRecievedMod();
            }
        }

        if(health != null)
        {
            health.runtimeValue += amount * mod;
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
            floatHealth += amount * mod;
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
}
