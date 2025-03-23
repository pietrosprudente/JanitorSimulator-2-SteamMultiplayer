using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IsTriggerEvent : MonoBehaviour
{
    [SerializeField]
    string TriggereTag;
    [SerializeField]
    UnityEvent TriggerEnterEvent;
    [SerializeField]
    UnityEvent TriggerExitEvent;
    Collider otherCollider;
    Collision otherCollision;

    private void OnTriggerEnter(Collider other) {
        otherCollider = other;
        if (otherCollider.tag == TriggereTag) {
            TriggerEnterEvent.Invoke();       
        }
    }

    void OnTriggerExit(Collider other) {
        otherCollider = null;
        if (otherCollider.tag == TriggereTag) {
            TriggerExitEvent.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        otherCollision = collision;
        if(collision.gameObject.tag == TriggereTag) {
            TriggerEnterEvent.Invoke();
        }
    }

    private void OnCollisionExit(Collision collision) {
        otherCollision = null;
        if (collision.gameObject.tag == TriggereTag) {
            TriggerExitEvent.Invoke();
        }
    }

}
