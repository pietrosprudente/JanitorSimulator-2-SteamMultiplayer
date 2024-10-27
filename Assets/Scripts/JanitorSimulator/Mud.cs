using UnityEngine;

public class Mud : MonoBehaviour
{
    public float maxSpeed = 4;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.maxLinearVelocity = maxSpeed;
            rb.maxAngularVelocity = maxSpeed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.maxLinearVelocity = int.MaxValue;
            rb.maxAngularVelocity = int.MaxValue;
        }
    }
}
