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

	[System.Serializable]
	public class ProjectileSettings
	{
		public Attack attack;
		public float length;
		public LayerMask hitMask;
		public int numPierces;
		public bool ignoreObstacles;
		public float lifetime;
		public float speed;
	}

	public override void InitNetworkState(string validTag, float damageMod)
	{
		//Object = GetComponent<NetworkObject>();
		//Debug.LogWarning("Object:");
		//Debug.LogWarning(Object);
		//lifeTimer = TickTimer.CreateFromSeconds(Runner, settings.lifetime);
		//velocity = settings.speed * transform.forward;
		this.validTag = validTag;
		this.damageMod = damageMod;
	}

	public override void Spawned()
	{
		lifeTimer = TickTimer.CreateFromSeconds(Runner, settings.lifetime);
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
		Runner.LagCompensation.RaycastAll(transform.position - 0.5f * dir, dir, settings.length, Object.InputAuthority, hits, settings.hitMask.value);

		foreach (LagCompensatedHit hit in hits)
		{
			if (hit.Hitbox != null && hit.GameObject.tag == validTag && hit.Hitbox.Root.Object.InputAuthority != Object.InputAuthority && !hitList.Contains(hit.GameObject))
			{
				Debug.LogError("Hit valid target");
				settings.attack.ApplyEffects(hit.GameObject, validTag, damageMod: damageMod);
				numHits++;
				hitList.Add(hit.GameObject);

				if (numHits > settings.numPierces)
				{
					// TODO: Ending effects/subattacks
					DestroyProjectile();
				}
			}

			else if (hit.Collider != null && !settings.ignoreObstacles)
			{
				// TODO: Ending effects/subattacks
				DestroyProjectile();
			}
		}

		transform.position += velocity * Runner.DeltaTime;
	}

	private void DestroyProjectile()
    {
		Runner.Despawn(Object);
    }
}
