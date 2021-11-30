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

    public bool useAmmo;

    public AmmoSettings ammo;

    [System.Serializable]
    public class AmmoSettings
    {
        public int clipSize;

        [Tooltip("In seconds")]
        public float reloadSpeed;

        [HideInInspector]
        public int value;

        [HideInInspector]
        public bool reloading;
    }

    public bool useOverheat;
    public OverheatSettings overheat;

    [System.Serializable]
    public class OverheatSettings
    {
        [Tooltip("How many points overheat before it goes into cooldown")]
        public float limit;

        [Tooltip("How many points of overheat are gained every time the weapon is fired")]
        public float rate;

        [Tooltip("How long in seconds before an overheated weapon can be fired")]
        public float cooldown;

        [Tooltip("How many points of overheat is reduced per Recharge Tick Rate (assuming weapon isn't already overheated)")]
        public float rechargeRate;

        [Tooltip("How many seconds between each recharge")]
        public float rechargeTickRate;

        [Tooltip("How many seconds after a shot is fired before overheat recharge can begin")]
        public float rechargeDelay;

        [HideInInspector]
        public float value;

        [HideInInspector]
        public bool onCooldown;

        [HideInInspector]
        public bool isRecharging;
    }

    public bool playAudioForEachAttack;

    private AudioSource audio;

    public void Start()
    {
        if(useAmmo)
        {
            ammo.value = ammo.clipSize;
        }

        TryGetComponent(out audio);
    }

    public bool Use(GameObject self, string targetTag, bool useDestination, Vector3? destination = null, bool useRotation = false, Quaternion? rotation = null)
    {
        if(overheat.onCooldown || ammo.reloading)
        {
            return false;
        }

        if(!useDestination)
        {
            destination = Vector3.negativeInfinity;
        }

        bool didUseAttack = false;

        foreach(AttackSettings attackSetting in attacks)
        {
            if(Time.time - attackSetting.time >= attackSetting.cooldown)
            {
                if(didUseAttack == false)
                {
                    if(useOverheat)
                    {
                        RunOverheat();
                    }

                    if(useAmmo)
                    {
                        RunAmmo();
                    }
                }

                didUseAttack = true;
                attackSetting.time = Time.time;

                if(useRotation)
                {
                    attackSetting.attack.PerformAttack(self, attackSetting.delay, destination, targetTag, useOverrideRotation: true, overrideRotation: rotation);
                }

                else
                {
                    attackSetting.attack.PerformAttack(self, attackSetting.delay, destination, targetTag);
                }

                if(playAudioForEachAttack && audio != null)
                {
                    audio.PlayDelayed(attackSetting.delay);
                }
            }
        }

        if (didUseAttack && !playAudioForEachAttack && audio != null)
        {
            audio.Play();
        }

        return didUseAttack;
    }

    private void RunOverheat()
    {
        overheat.value += overheat.rate;
        StopAllCoroutines();
        overheat.isRecharging = false;

        if (overheat.value >= overheat.limit)
        {
            overheat.onCooldown = true;
            overheat.value = overheat.limit;
            StartCoroutine(Cooldown());
        }

        else
        {
            StartCoroutine(Recharge());
        }
    }

    private void RunAmmo()
    {
        ammo.value -= 1;

        if(ammo.value <= 0)
        {
            ammo.value = 0;
            ammo.reloading = true;
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Recharge()
    {
        yield return new WaitForSeconds(overheat.rechargeDelay);
        overheat.isRecharging = true;

        while(true)
        {
            overheat.value -= overheat.rechargeRate;

            if(overheat.value <= 0)
            {
                break;
            }

            yield return new WaitForSeconds(overheat.rechargeTickRate);
        }

        overheat.isRecharging = false;
        overheat.value = 0;
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(overheat.cooldown);
        overheat.value = 0;
        overheat.onCooldown = false;
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(ammo.reloadSpeed);
        ammo.reloading = false;
        ammo.value = ammo.clipSize;
    }

    private void OnGUI()
    {
        string text = "";

        if(useAmmo)
        {
            text += "Ammo: " + ammo.value + "\n";

            if(ammo.reloading)
            {
                text += "Reloading\n";
            }
        }

        if(useOverheat)
        {
            text += "Overheat: " + overheat.value.ToString("0.00");

            if (overheat.onCooldown)
            {
                text += "\nOn Cooldown";
            }

            if (overheat.isRecharging)
            {
                text += "\nRecharging";
            }
        }

        GUI.TextField(new Rect(30, 300, 130, 100), text);
    }
}
