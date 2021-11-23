using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Weapon : MonoBehaviour
{
    public Rarities rarity;

    [SerializeField]
    private List<AttackSettings> attacks = new List<AttackSettings>();

    [System.Serializable]
    public class AttackSettings
    {
        public Attack attack;
        public float cooldown;
        public float delay;

        [HideInInspector]
        public float time = -10000;
    }

    public bool Use(GameObject self, string targetTag, bool useDestination, Vector3? destination = null)
    {
        if(!useDestination)
        {
            destination = Vector3.negativeInfinity;
        }

        bool didUseAttack = false;

        foreach(AttackSettings attackSetting in attacks)
        {
            if(Time.time - attackSetting.time >= attackSetting.cooldown)
            {
                didUseAttack = true;
                attackSetting.attack.PerformAttack(self, attackSetting.delay, destination, targetTag);
            }
        }

        return didUseAttack;
    }
}
