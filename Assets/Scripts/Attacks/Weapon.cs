using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Weapon : MonoBehaviour
{
    public Rarities rarity;

    [SerializeField]
    private List<AttackSettings> attacks = new List<AttackSettings>();

    [SerializeField]
    private Weapon alternateAttackWeapon;

    [System.Serializable]
    public class AttackSettings
    {
        public Attack attack;
        public float cooldown;
        public float delay;
        public List<AudioSource> audio = new List<AudioSource>();

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

    public bool useSpinup;
    public SpinupSettings spinup;

    [System.Serializable]
    public class SpinupSettings
    {
        [Tooltip("How many points of spinup is needed before firing can occur")]
        public float limit;

        [Tooltip("How many points past the spinup limit can this weapon be spun")]
        public float overspinLimit;

        [Tooltip("Rate at which spinup increases for each tick")]
        public float rate;

        [Tooltip("Rate at which spinup decays when not firing")]
        public float decayRate;

        public float decayDelay;

        public float decayTickRate;

        [HideInInspector]
        public float value;
    }

    public bool playAudioForEachAttack;

    public bool attackIsDirectlyControlled;

    private AudioSource audio;

    private Coroutine spinCoroutine;
    private bool spinDecaying;

    public NetworkObject owner;

    public void Start()
    {
        if(useAmmo)
        {
            ammo.value = ammo.clipSize;
        }

        TryGetComponent(out audio);

        if(owner == null)
        {
            transform.parent.TryGetComponent(out Weapon w);

            if(w != null)
            {
                owner = w.owner;
            }
        }
    }

    public bool Use(GameObject self, string targetTag, bool useDestination, Vector3? destination = null, bool useRotation = false, Quaternion? rotation = null, WeaponHolder weaponHolder = null)
    {
        if(useSpinup)
        {
            if(!overheat.onCooldown && ! ammo.reloading)
            {
                RunSpinup();
            }

            else if(!spinDecaying)
            {
                spinDecaying = true;
                spinCoroutine = StartCoroutine(SpinupDecay());
            }

            if(spinup.value < spinup.limit)
            {
                return false;
            }
        }

        if (overheat.onCooldown || ammo.reloading)
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
                    attackSetting.attack.PerformAttack(self, attackSetting.delay, destination, targetTag, useOverrideRotation: true, overrideRotation: rotation, weaponHolder: weaponHolder, useSelfForPositioning: true);
                }

                else
                {
                    attackSetting.attack.PerformAttack(self, attackSetting.delay, destination, targetTag, weaponHolder: weaponHolder, useSelfForPositioning: true);
                }

                if(playAudioForEachAttack)
                {
                    foreach(AudioSource a in attackSetting.audio)
                    {
                        a.PlayDelayed(attackSetting.delay);
                    }
                }
            }
        }

        if (didUseAttack && !playAudioForEachAttack && audio != null)
        {
            audio.Play();
        }

        return didUseAttack;
    }

    public bool UseAlternate(GameObject self, string targetTag, bool useDestination, Vector3? destination = null, bool useRotation = false, Quaternion? rotation = null, WeaponHolder weaponHolder = null)
    {
        if (alternateAttackWeapon == null)
        {
            return false;
        }

        return alternateAttackWeapon.Use(self, targetTag, useDestination, destination, useRotation, rotation, weaponHolder);
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

    private void RunSpinup()
    {
        if(spinCoroutine != null)
        {
            StopCoroutine(spinCoroutine);
            spinDecaying = false;
        }

        spinup.value += spinup.rate;

        if(spinup.value > spinup.limit + spinup.overspinLimit)
        {
            spinup.value = spinup.limit + spinup.overspinLimit;
        }

        spinCoroutine = StartCoroutine(SpinupDecay());
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

    private IEnumerator SpinupDecay()
    {
        yield return new WaitForSeconds(spinup.decayDelay);

        while (spinup.value > 0)
        {
            spinup.value -= spinup.decayRate;
            yield return new WaitForSeconds(spinup.decayTickRate);
        }

        if(spinup.value < 0)
        {
            spinup.value = 0;
        }

        spinDecaying = false;
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
            text += "Overheat: " + overheat.value.ToString("0.00") + "\n";

            if (overheat.onCooldown)
            {
                text += "On Cooldown\n";
            }

            if (overheat.isRecharging)
            {
                text += "Recharging\n";
            }
        }

        if(useSpinup)
        {
            text += "Spinup: " + spinup.value.ToString("0.00") + "\n";
            text += "Spinup required: " + spinup.limit.ToString("0.00");
        }

        GUI.TextField(new Rect(30, 300, 130, 100), text);
    }
}
