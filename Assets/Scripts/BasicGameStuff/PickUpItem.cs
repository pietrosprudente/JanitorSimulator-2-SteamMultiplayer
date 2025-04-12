using FishNet.Component.Transforming;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks.Data;
using UnityEngine;

namespace BasicGameStuff
{
    public class PickUpItem : NetworkBehaviour
    {
        public bool lookAtPlayer = false;

        [HideInInspector] public Rigidbody rb;

        public override void OnStartClient()
        {
            base.OnStartClient();
            rb = GetComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = false;
        }

        [ServerRpc(RequireOwnership = false)]
        public void GrabItem(NetworkConnection conn)
        {
            NetworkObject.GiveOwnership(conn);
            rb.isKinematic = false;
        }

        [ServerRpc(RequireOwnership = false)]
        public void DropItem()
        {
            rb.AddForce(Vector3.up * 1.2f);
        }
    }
}
