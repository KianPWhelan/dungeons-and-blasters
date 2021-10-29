using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

[CreateAssetMenu(menuName = "Modules/AI/JormungandrAI")]
public class JormungandrAI : EnemyAI
{
    [SerializeField]
    private float maxRangeBeforeTP = 30;
    [SerializeField]
    private float teleportTimeout = 1;
    [SerializeField]
    private float rageHealthThreshold = 0.3f;
    [SerializeField]
    private GameObject teleportSound;
    [SerializeField]
    private float minTimeForRageTp = 2f;
    [SerializeField]
    private float maxTimeForRageTp = 20f;

    private GameObject targetPlayer;
    private GameObject localSelf;
    private Dictionary<GameObject, Container> data = new Dictionary<GameObject, Container>();
    private Container thisData;

    private class Container
    {
        public bool onTimeout = false;
        public float startTimeoutTime = -1000f;
        public int arsenal = 0;
        public bool inRage = false;
        public float tpDistance = 25;
        public float timeOfLastNothingPersonalKid = -1000f;
        public float nextTpTime = 0f;
    }

    public override void Tick(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement)
    {
        if(!data.ContainsKey(self))
        {
            thisData = new Container();
            data.Add(self, thisData);
        }

        else
        {
            thisData = data[self];
        }

        localSelf = self;

        if (target == null)
        {
            target = Helpers.FindClosest(self.transform, "Player");

            if (target == null)
            {
                return;
            }
        }

        if (!thisData.onTimeout)
        {
            var mine = self.GetPhotonView().IsMine;

            if (Vector3.Distance(self.transform.position, target.transform.position) > maxRangeBeforeTP && mine)
            {
                Debug.Log("Teleporting to player");
                ChooseTargetPlayer();
                TeleportInFrontOfTargetPlayer();
                self.transform.LookAt(target.transform);
                self.transform.rotation = Quaternion.Euler(0, self.transform.rotation.eulerAngles.y, self.transform.rotation.eulerAngles.z);
                thisData.startTimeoutTime = Time.time;
                thisData.onTimeout = true;
                return;
            }

            else if(mine && thisData.inRage && Time.time - thisData.timeOfLastNothingPersonalKid >= thisData.nextTpTime)
            {
                Debug.Log("Teleporting to player");
                ChooseTargetPlayer();
                TeleportInFrontOfTargetPlayer();
                self.transform.LookAt(target.transform);
                self.transform.rotation = Quaternion.Euler(0, self.transform.rotation.eulerAngles.y, self.transform.rotation.eulerAngles.z);
                thisData.nextTpTime = UnityEngine.Random.Range(minTimeForRageTp, maxTimeForRageTp);
                Debug.Log("Next TP time: " + thisData.nextTpTime);
                thisData.startTimeoutTime = Time.time;
                thisData.onTimeout = true;
                thisData.timeOfLastNothingPersonalKid = Time.time;
                return;
            }

            DoAttacks(target);
        }

        else if(Time.time - thisData.startTimeoutTime >= teleportTimeout)
        {
            thisData.onTimeout = false;
        }
        
        self.transform.LookAt(target.transform);
        self.transform.rotation = Quaternion.Euler(0, self.transform.rotation.eulerAngles.y, self.transform.rotation.eulerAngles.z);
    }

    private void ChooseTargetPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        int choice = UnityEngine.Random.Range(0, players.Length);
        targetPlayer = players[choice];
    }

    private void TeleportInFrontOfTargetPlayer()
    {
        var tile = Helpers.FindClosest(targetPlayer.transform.position + (targetPlayer.transform.forward * thisData.tpDistance), "Ground");
        Debug.Log("Teleporting to " + tile.transform.position);
        localSelf.transform.position = tile.transform.position;
        
        var m_Sound = Instantiate(teleportSound, localSelf.transform.position, Quaternion.identity);
        var m_Source = m_Sound.GetComponent<AudioSource>();
        float life = m_Source.clip.length / m_Source.pitch;
        Destroy(m_Sound, life);
    }

    private void DoAttacks(GameObject target)
    {
        var weaponHolder = localSelf.GetComponent<WeaponHolder>();

        // Use shotgun
        weaponHolder.UseWeapon(thisData.arsenal, target.transform.position, true);

        // Spawn aura
        weaponHolder.UseWeapon(thisData.arsenal + 1);

        var hp = localSelf.GetComponent<Health>();

        if(hp.GetHealth() / hp.startingHealth <= rageHealthThreshold)
        {
            thisData.arsenal = 4;
            DoRageSetupStuff();
            var players = GameObject.FindGameObjectsWithTag("Player");
            Array.Sort(players, delegate (GameObject player1, GameObject player2) { return Vector3.Distance(localSelf.transform.position, player1.transform.position).CompareTo(Vector3.Distance(localSelf.transform.position, player2.transform.position)); });
            int lastIndexOfValidPlayer = 0;

            for(int i = 0; i < 2; i++)
            {
                if(players.Length > i + 1 && Vector3.Distance(localSelf.transform.position, players[i + 1].transform.position) <= maxRangeBeforeTP)
                {
                    weaponHolder.UseWeapon(i + 2, players[i + 1].transform.position, true);
                    lastIndexOfValidPlayer = i + 1;
                }

                else
                {
                    var index = UnityEngine.Random.Range(0, lastIndexOfValidPlayer);
                    weaponHolder.UseWeapon(i + 2, players[index].transform.position, true);
                }
            }
        }
    }

    private void DoRageSetupStuff()
    {
        if (thisData.inRage)
        {
            return;
        }

        else
        {
            thisData.inRage = true;
            thisData.tpDistance = -7;
        }

        var attacks = localSelf.GetComponentsInChildren<AttackScript>();
        GameObject aura = null;

        foreach(AttackScript attack in attacks)
        {
            if(attack.gameObject.name.Contains("Jorm Aura"))
            {
                aura = attack.gameObject;
            }
        }

        if(aura != null)
        {
            Destroy(aura);
        }
    }
}
