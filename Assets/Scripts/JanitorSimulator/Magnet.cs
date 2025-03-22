using System.Collections;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

public class Magnet : NetworkBehaviour
{
    [Header("Magnet Settings")]
    public float attractionRadius = 5f;
    public float attractionForce = 10f;
    public float updateInterval = 0.1f;

    [Server]
    public override void OnStartServer()
    {
        StartCoroutine(ApplyAttraction());
    }

    [Server]
    private IEnumerator ApplyAttraction()
    {
        while (true)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, attractionRadius);

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent<Rigidbody>(out var rb) && collider.TryGetComponent(out TrashItem item))
                {
                    item.GrabItem(LocalConnection);
                    Vector3 direction = (transform.position - rb.position).normalized;

                    rb.AddForce(attractionForce * Time.deltaTime * direction, ForceMode.Acceleration);
                }
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    }
}
