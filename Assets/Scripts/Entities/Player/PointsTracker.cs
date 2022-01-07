using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PointsTracker : NetworkBehaviour
{
    [Networked]
    public float points { get; set; }

    [Networked]
    public float damageDealt { get; set; }

    [Networked]
    public float distanceMoved { get; set; }

    [SerializeField]
    private PointValues pointValues;

    public void GiveDamagePoints(float damage)
    {
        damageDealt += damage;
        points += damage * pointValues.pointsPerDamage;
    }

    public void GiveMovementPoints(float distance)
    {
        distanceMoved += distance;
        points += distance * pointValues.pointsPerDistanceMoved;
    }
}
