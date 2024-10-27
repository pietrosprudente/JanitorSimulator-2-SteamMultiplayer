using BasicGameStuff;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class TrashItem : PickUpItem
{
    [Range(0f, 100f)]
    public float moveSpeedPercentage = 100;
    public int moneyWorth = 3;

    private Rigidbody rb;

    private void OnDestroy()
    {
        if(!ShiftManager.isFreePlay)
        {
            EconomySystem.MoneyBux += moneyWorth;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }
}