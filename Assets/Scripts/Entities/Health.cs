using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

    /// <summary>
    /// Adjusts health value by amount provided, can be negative
    /// </summary>
    /// <param name="amount"></param>
    public void AdjustHealth(float amount)
    {
        if(infiniteHealth)
        {
            return;
        }

        if(health != null)
        {
            health.runtimeValue += amount;
            if(health.runtimeValue <= 0)
            {
                isDead = true;
            }
        }

        else
        {
            floatHealth += amount;
            if(floatHealth <= 0)
            {
                isDead = true;

                if(photonView.IsMine)
                {
                    if(isPlayer)
                    {
                        PhotonNetwork.LeaveRoom();
                    }

                    else
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }            
                }
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
}
