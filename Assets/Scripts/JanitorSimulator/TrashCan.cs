using FishNet.Object;
using UnityEngine;

public class TrashCan : NetworkBehaviour
{
    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TrashItem>())
        {
            Despawn(other.gameObject, DespawnType.Destroy);
            HallwayGenerator.UpdateHallway();
        }
    }
}
