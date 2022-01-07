using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Fusion;

[CreateAssetMenu(menuName = "Modules/AI/Jumping Frog AI")]
public class JumpingFrogAI : EnemyAI
{
    [SerializeField]
    private float minJumpDelay;

    [SerializeField]
    private float maxJumpDelay;

    [SerializeField]
    private float initialAngle;

    [SerializeField]
    private float fallingSpeed = -9.8f;

    private Dictionary<GameObject, Data> data = new Dictionary<GameObject, Data>();

    private class Data
    {
        public float jumpTimer = Time.time;

        public float jumpDelay;

        public Rigidbody rigidbody;

        public WeaponHolder weaponHolder;

        public NavMeshAgent agent;

        public EnemyGeneric enemyGeneric;

        public bool isJumping;

        public Data(GameObject self, float minJumpDelay, float maxJumpDelay)
        {
            jumpDelay = Random.Range(minJumpDelay, maxJumpDelay);
            rigidbody = self.GetComponent<Rigidbody>();
            weaponHolder = self.GetComponent<WeaponHolder>();
            agent = self.GetComponent<NavMeshAgent>();
            enemyGeneric = self.GetComponent<EnemyGeneric>();
        }
    }

    public override void Tick(GameObject self, GameObject target, GameObject allyTarget, NavMeshAgent agent, Movement movement, EnemyGeneric enemyGeneric = null)
    {
        Data d = GetData(self);

        if(Time.time - d.jumpTimer >= d.jumpDelay)
        {
            TryAttack(self, target, d);
        }

        if(target != null)
        {
            self.transform.rotation = Quaternion.LookRotation((target.transform.position - self.transform.position).normalized);
        }
    }

    private Data GetData(GameObject self)
    {
        if(data.ContainsKey(self))
        {
            return data[self];
        }

        else
        {
            Data d = new Data(self, minJumpDelay, maxJumpDelay);
            data.Add(self, d);
            return d;
        }
    }

    private void SetJumpDelay(Data d)
    {
        d.jumpDelay = Random.Range(minJumpDelay, maxJumpDelay);
        d.jumpTimer = Time.time;
    }

    private void TryAttack(GameObject self, GameObject target, Data d)
    {
        if(target != null && !d.isJumping)
        {
            d.enemyGeneric.StartCoroutine(JumpingAttack(self, target, d));
        }
    }

    private IEnumerator JumpingAttack(GameObject self, GameObject target, Data d)
    {
        d.isJumping = true;
        Vector3 velocity = GetJumpForce(self, target);
        d.agent.enabled = false;
        Vector3 lastPosition = self.transform.position;

        while(!IsOnGround(self, lastPosition))
        {
            lastPosition = self.transform.position;
            self.transform.position += velocity * Time.deltaTime;
            velocity -= Time.deltaTime * Vector3.down * fallingSpeed;
            Debug.Log(velocity);
            yield return new WaitForFixedUpdate();
        }

        SetJumpDelay(d);
        d.agent.enabled = true;
        d.isJumping = false;
        d.weaponHolder.UseWeapon(0, self.transform.position, true);
    }

    private Vector3 GetJumpForce(GameObject self, GameObject target)
    {
        Vector3 p = target.transform.position;

        float gravity = -fallingSpeed;
        // Selected angle in radians
        float angle = initialAngle * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(p.x, 0, p.z);
        Vector3 planarPostion = new Vector3(self.transform.position.x, 0, self.transform.position.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = self.transform.position.y - p.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (p.x > self.transform.position.x ? 1 : -1);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;
        //finalVelocity = Quaternion.AngleAxis(finalVelocity.y - (target.transform.position - self.transform.position).y , Vector3Int.up) * finalVelocity;
        Debug.Log("Final Velocity: " + finalVelocity);

        // Fire!
        return finalVelocity;
    }

    private bool IsOnGround(GameObject self, Vector3 lastPosition)
    {
        if ((lastPosition - self.transform.position).y <= 0)
        {
            return false;
        }

        else
        {
            var hits = Physics.RaycastAll(self.transform.position + Vector3.up * 50, Vector3.down, 50.1f, LayerMask.GetMask("Default"), queryTriggerInteraction: QueryTriggerInteraction.Ignore);

            foreach(RaycastHit hit in hits)
            {
                if(hit.collider.tag == "Ground")
                {
                    return true;
                }
            }
        }

        return false;
    }
}
