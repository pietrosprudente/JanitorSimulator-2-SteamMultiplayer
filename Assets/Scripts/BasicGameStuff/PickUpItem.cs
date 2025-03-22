using FishNet.Component.Transforming;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace BasicGameStuff
{
    public class PickUpItem : NetworkBehaviour
    {
        public bool lookAtPlayer = false;
        public readonly SyncVar<bool> isGrabbed = new(false);

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
            if (isGrabbed.Value && !IsOwner) return;
            NetworkObject.GiveOwnership(conn);
            isGrabbed.Value = true;
        }

        [ServerRpc(RequireOwnership = false)]
        public void DropItem()
        {
            if (!isGrabbed.Value) return;
            isGrabbed.Value = false;
        }
    }
}
