using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


[OrderAfter(typeof(HitboxManager))]
public class Projectile : AttackComponent
{
	[Header("Settings")]
	[SerializeField]
	private ProjectileSettings settings;

	[Networked]
	public TickTimer networkedLifeTimer { get; set; }
	private TickTimer predictedLifeTimer;
	private TickTimer lifeTimer
	{
		get => Object.IsPredictedSpawn ? predictedLifeTimer : networkedLifeTimer;
		set { if (Object.IsPredictedSpawn) predictedLifeTimer = value; else networkedLifeTimer = value; }
	}

	[Networked]
	public Vector3 networkedVelocity { get; set; }
	private Vector3 predictedVelocity;
	public Vector3 velocity
	{
		get => Object.IsPredictedSpawn ? predictedVelocity : networkedVelocity;
		set { if (Object.IsPredictedSpawn) predictedVelocity = value; else networkedVelocity = value; }
	}

	public NetworkBool networkedDestroyed { get; set; }
	private bool predictedDestroyed;
	private bool destroyed
	{
		get => Object.IsPredictedSpawn ? predictedDestroyed : (bool)networkedDestroyed;
		set { if (Object.IsPredictedSpawn) predictedDestroyed = value; else networkedDestroyed = value; }
	}

	private Vector3 lastPosition;

	private List<LagCompensatedHit> areaHits = new List<LagCompensatedHit>();

	private List<GameObject> hitList = new List<GameObject>();

	private int numHits;

	private Vector3 hitNormal;

	private Vector3 hitPoint;

	private NetworkObject owner;

	[System.Serializable]
	public class ProjectileSettings
	{
		public Attack attack;

		public List<Attack> subAttacks = new List<Attack>();
		public bool subAttacksOnEnd;
		public bool subAttacksOnHit;
		public bool subAttacksAtSurfaceNormal;
		public bool subAttacksCanOnlyProcOnce;

		public Vector3 offset;
		public bool ignoreRotationForOffset;
		public Vector3 rotationOffset;
		public bool worldSpaceRotation;
		public bool reevaluateDestinationAfterOffset;
		public float spread;

		public float length;
		public LayerMask hitMask;
		public bool infinitePierce;
		public int numPierces;
		public bool canMultiHitTarget;
		public float multiHitCooldown;
		public bool ignoreObstacles;
		public bool applyEffectsOnEnd;
		public float lifetime;
		public float speed;
		public float gravityStrength;

		public bool homing;
		public float homingStrength;
		public bool reevaluateHomingTargetConstantly;

		[Tooltip("Performs another raycast from the previous position to the current one, used to garuntee that extremely high speed projectiles register hits consistently, at the cost of nearly doubling the performance cost of the script")]
		public bool performSafetyHitRegistration;
	}

	private Transform target;
	private bool subAttacksHaveProced;

	public override void InitNetworkState(string validTag, float damageMod, object destination, NetworkObject owner = null, int weaponIndex = 0, int attackIndex = 0, bool isAlt = false)
    {
		base.InitNetworkState(validTag, damageMod, destination);
		//Object = GetComponent<NetworkObject>();
		//Debug.LogWarning("Object:");
		//Debug.LogWarning(Object);
		//lifeTimer = TickTimer.CreateFromSeconds(Runner, settings.lifetime);
		//velocity = settings.speed * transform.forward;
		this.validTag = validTag;
		this.damageMod = damageMod;

        if (destination != null)
        {
            Debug.Log("Destination recieved: " + destination);
            this.destination = (Vector3)destination;
            useDestination = true;
            Debug.Log(useDestination);
        }

		this.owner = owner;
	}

	public override void Spawned()
	{
		if(!Object.HasStateAuthority)
        {
			return;
        }

		Debug.Log("Spawned " + useDestination);
		// Create lifetimer
		lifeTimer = TickTimer.CreateFromSeconds(Runner, settings.lifetime);

		// Perform initial direction rotation
		if(useDestination)
        {
			Debug.Log("Using destination");
			transform.rotation = Quaternion.LookRotation((destination - transform.position).normalized);
		}

		// Calculate spread
		Vector3 accuracyOffset = Random.insideUnitCircle * settings.spread;

		// Adjust position to offset
		if(settings.ignoreRotationForOffset)
        {
			transform.position += settings.offset;
        }

		else
        {
			transform.position += (transform.forward * settings.offset.z) + (transform.right * settings.offset.x) + (transform.up * settings.offset.y);
		}

		// Adjust rotation offset
		if(settings.worldSpaceRotation)
        {
			transform.rotation = Quaternion.Euler(settings.rotationOffset + accuracyOffset);
        }

		else
        {
			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + settings.rotationOffset + accuracyOffset);
		}

		// Reevaluate rotation towards directions
		if(settings.reevaluateDestinationAfterOffset)
        {
			Debug.Log("reevaluating direction");
			transform.rotation = Quaternion.LookRotation((destination - transform.position).normalized);
			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + accuracyOffset);
		}

		// Set velocity
		velocity = settings.speed * transform.forward;
		GetComponent<NetworkTransform>().InterpolationDataSource = InterpolationDataSources.Predicted;

		// Set homing target
		if(settings.homing)
        {
			GetHomingTarget();
        }

		lastPosition = transform.position;
	}

	public override void FixedUpdateNetwork()
	{
		if (!Object.HasStateAuthority)
		{
			return;
		}

		if (!lifeTimer.Expired(Runner))
		{
			UpdateProjectile();
		}

		else
		{
			// TODO: Ending effects/subattacks
			hitPoint = transform.position;
			ApplyEffectsOnEnd();
			SubAttacksOnEnd();
			DestroyProjectile();
		}
	}

	private void UpdateProjectile()
	{
		// Check for hits
		// Process hits
		// Apply effects
		// Sub attacks
		// Sound/visual fx
		// Move projectile
		// Check for walls
		// Homing
		if(settings.reevaluateHomingTargetConstantly)
        {
			GetHomingTarget();
        }

		if(settings.homing)
        {
			Quaternion newDir = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((target.position - transform.position).normalized), settings.homingStrength * Runner.DeltaTime);
			transform.rotation = newDir;
			velocity = transform.forward * settings.speed;
        }

		if(settings.gravityStrength > 0)
        {
			velocity = new Vector3(velocity.x, velocity.y - (9.8f * settings.gravityStrength * Runner.DeltaTime), velocity.z);
			transform.LookAt(transform.position + velocity * Runner.DeltaTime);
        }

		Vector3 vel = velocity;
		Vector3 dir = vel.normalized;

		List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
		Runner.LagCompensation.RaycastAll(transform.position - 0.5f * dir, dir, settings.length, Object.InputAuthority, hits, settings.hitMask.value, options: HitOptions.IncludePhysX);
		ProcessHits(hits);

		if(settings.performSafetyHitRegistration && lastPosition != null && transform != null)
        {
			hits = new List<LagCompensatedHit>();
			Runner.LagCompensation.RaycastAll(lastPosition, (transform.position - lastPosition).normalized, Vector3.Distance(lastPosition, transform.position), Object.InputAuthority, hits, settings.hitMask.value, options: HitOptions.IncludePhysX);
			ProcessHits(hits);
		}

		lastPosition = transform.position;

		try
        {
			transform.position += velocity * Runner.DeltaTime;
		}
		
		catch
		{ }
	}

	private void ProcessHits(List<LagCompensatedHit> hits)
    {
		var hitArray = hits.ToArray();
		System.Array.Sort(hitArray, delegate (LagCompensatedHit hit1, LagCompensatedHit hit2) { return hit1.Distance.CompareTo(hit2.Distance); });

		foreach (LagCompensatedHit hit in hitArray)
		{
			//Debug.Log((hit.Hitbox != null) + " " + (hit.GameObject.tag == validTag) + " " + (hit.Hitbox.Root.Object.InputAuthority != Object.InputAuthority) + " " + (!hitList.Contains(hit.GameObject)));
			// TODO: Stop projectile from hitting caster if caster has same target tag
			if (hit.Hitbox != null && hit.Hitbox.Root.tag == validTag/* && hit.Hitbox.Root.Object.InputAuthority != Object.InputAuthority */&& !hitList.Contains(hit.Hitbox.Root.gameObject))
			{
				Debug.Log("Hit valid target");
				settings.attack.ApplyEffects(hit.Hitbox.Root.gameObject, validTag, damageMod: damageMod);
				numHits++;
				hitList.Add(hit.Hitbox.Root.gameObject);
				hitNormal = hit.Normal;
				hitPoint = hit.Point;
				SubAttacksOnHit();

				if (settings.canMultiHitTarget)
				{
					StartCoroutine(RemoveFromHitListDelay(hit.Hitbox.Root.gameObject));
				}

				if (numHits > settings.numPierces && !settings.infinitePierce)
				{
					// TODO: Ending effects/subattacks
					DestroyProjectile();
				}
			}

			else if (hit.Collider != null && !settings.ignoreObstacles)
			{
				// TODO: Ending effects/subattacks
				hitNormal = hit.Normal;
				hitPoint = hit.Point;
				SubAttacksOnEnd();
				ApplyEffectsOnEnd();
				DestroyProjectile();
			}
		}
	}

	private void DestroyProjectile()
    {
		Runner.Despawn(Object);
    }

	private void ApplyEffectsOnEnd()
    {
		if (settings.applyEffectsOnEnd)
		{
			settings.attack.ApplyEffects(null, validTag, hitPoint, transform.rotation, damageMod);
		}
	}

	private void SubAttacksOnHit()
    {
		if(settings.subAttacksOnHit)
        {
			SpawnSubAttacks();
        }
    }

	private void SubAttacksOnEnd()
    {
		if(settings.subAttacksOnEnd)
        {
			SpawnSubAttacks();
        }
    }

	private void SpawnSubAttacks()
    {
		if (subAttacksHaveProced)
        {
			return;
        }

		subAttacksHaveProced = true;
		foreach(Attack attack in settings.subAttacks)
        {
			if(settings.subAttacksAtSurfaceNormal)
            {
				Debug.Log("Spawning sub attacks");
				attack.PerformAttack(hitPoint, transform.rotation, damageMod, 0, hitPoint + hitNormal, validTag, 0f, owner: owner);
			}

			else
            {
				attack.PerformAttack(hitPoint, transform.rotation, damageMod, 0, targetTag: validTag, delay: 0f, destination: Vector3.negativeInfinity, owner: owner);
			}
        }
    }

	private void GetHomingTarget()
    {
		var temp = Helpers.FindClosest(transform, validTag);

		if (temp == null)
		{
			return;
		}

		if (temp.TryGetComponent(out EnemyGeneric e) && e.homingPoint != null)
		{
			target = e.homingPoint.transform;
		}

		else
		{
			target = temp.transform;
		}
	}

	private IEnumerator RemoveFromHitListDelay(GameObject hitObj)
    {
		yield return new WaitForSeconds(settings.multiHitCooldown);
		hitList.Remove(hitObj);
    }
}
