using BasicGameStuff;
using FishNet.Component.Transforming;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class TrashItem : PickUpItem
{
    [Range(0f, 100f)]
    public float moveSpeedPercentage = 100;
    public int moneyWorth = 3;

    private void OnDestroy()
    {
        if(!ShiftManager.isFreePlay)
        {
            EconomySystem.MoneyBux += moneyWorth;
        }
    }
}