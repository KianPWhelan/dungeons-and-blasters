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

	private List<LagCompensatedHit> areaHits = new List<LagCompensatedHit>();

	private List<GameObject> hitList = new List<GameObject>();

	private int numHits;

	private Vector3 hitNormal;

	private Vector3 hitPoint;

	[System.Serializable]
	public class ProjectileSettings
	{
		public Attack attack;

		public List<Attack> subAttacks = new List<Attack>();
		public bool subAttacksOnEnd;
		public bool subAttacksOnHit;
		public bool subAttacksAtSurfaceNormal;

		public Vector3 offset;
		//TODO
		public Vector3 rotationOffset;
		//TODO
		public bool reevaluateDestinationAfterOffset;

		public float length;
		public LayerMask hitMask;
		public int numPierces;
		//TODO
		public bool canMultiHitTarget;
		//TODO
		public float multiHitCooldown;
		public bool ignoreObstacles;
		public bool applyEffectsOnEnd;
		public float lifetime;
		public float speed;

		//TODO
		public bool homing;
		//TODO
		public float homingStrength;
		//TODO
		public bool reevaluateHomingTargetConstantly;
	}

	public override void InitNetworkState(string validTag, float damageMod, object destination)
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
    }

	public override void Spawned()
	{
		Debug.Log("Spawned " + useDestination);
		// Create lifetimer
		lifeTimer = TickTimer.CreateFromSeconds(Runner, settings.lifetime);

		// Perform initial direction rotation
		if(useDestination)
        {
			Debug.Log("Using destination");
			transform.rotation = Quaternion.LookRotation((destination - transform.position).normalized);
        }

		// Adjust position to offset
		transform.position += (transform.forward * settings.offset.z) + (transform.right * settings.offset.x) + (transform.up * settings.offset.y);

		// Set velocity
		velocity = settings.speed * transform.forward;
		GetComponent<NetworkTransform>().InterpolationDataSource = InterpolationDataSources.Predicted;
	}

	public override void FixedUpdateNetwork()
	{
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
		Vector3 vel = velocity;
		Vector3 dir = vel.normalized;

		List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
		Runner.LagCompensation.RaycastAll(transform.position - 0.5f * dir, dir, settings.length, Object.InputAuthority, hits, settings.hitMask.value, options: HitOptions.IncludePhysX);

		foreach (LagCompensatedHit hit in hits)
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

				if (numHits > settings.numPierces)
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

		try
        {
			transform.position += velocity * Runner.DeltaTime;
		}
		
		catch
		{ }
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
		foreach(Attack attack in settings.subAttacks)
        {
			if(settings.subAttacksAtSurfaceNormal)
            {
				Debug.Log("Spawning sub attacks");
				settings.attack.PerformAttack(hitPoint, transform.rotation, damageMod, 0, hitPoint + hitNormal, validTag, 0f);
			}
        }
    }
}
