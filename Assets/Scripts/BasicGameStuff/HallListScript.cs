using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

[CreateAssetMenu(fileName = "HallListScript", menuName = "Scriptable Objects/HallListScript")]
public class HallListScript : ScriptableObject
{
    public List<Hallway1> hallways;
}
