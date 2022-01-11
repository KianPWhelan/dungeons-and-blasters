using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointValues : ScriptableObject
{
    [Tooltip("The number of points rewarded per 1 damage dealt")]
    public float pointsPerDamage;

    [Tooltip("The number of points rewarded per 1 unit moved")]
    public float pointsPerDistanceMoved;
}
