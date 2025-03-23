using FishNet.Object;
using UnityEngine;

public class TrashCan : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TrashItem>() && IsServer)
        {
            Despawn(other.gameObject, DespawnType.Destroy);
            HallwayGenerator.UpdateHallway();
        }
    }
}
