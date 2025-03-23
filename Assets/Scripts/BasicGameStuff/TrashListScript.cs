using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashListScript", menuName = "Scriptable Objects/TrashListScript")]
public class TrashListScript : ScriptableObject
{
    public List<NetworkObject> trashPrefabs;
}
