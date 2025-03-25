using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RaceLineData", menuName = "RaceLine/RaceLineData", order = 1)]
public class RaceLineData : ScriptableObject
{
    public List<Vector3> locations = new List<Vector3>();
}