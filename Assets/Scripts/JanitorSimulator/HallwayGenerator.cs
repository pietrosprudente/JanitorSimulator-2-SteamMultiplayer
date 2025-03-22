using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class HallwayGenerator : NetworkBehaviour
{
    public static HallwayGenerator Instance { get; private set; }

    public List<Hallway1> hallways;
    public Vector3 hallwayOffset;
    public GameObject currentHallwayObj;
    public List<GameObject> trashPrefabs;
    public Vector3 trashBounds = new Vector3(10, 0, 10);

    private int trashAmount = 2;
    private int _trashAmount = 2;
    private int trashIncrement = 1;
    private Hallway1 currentHallway;
    private uint hallwaysUntilNext;
    private readonly SyncList<GameObject> trashList = new ();

    private uint HallwaysUntilNext
    {
        get
        {
            return hallwaysUntilNext;
        }
        set
        {
            hallwaysUntilNext = value;

            if(hallwaysUntilNext <= 0)
            {
                var index = hallways.IndexOf(currentHallway) + 1;
                currentHallway = hallways[index >= hallways.Count ? 0 : index];
                hallwaysUntilNext = currentHallway.length;
            }
        }
    }

    public int TrashAmount
    {
        get { return trashAmount; }
        set
        {
            trashAmount = value;
            /*
            if (trashAmount <= 0)
            {
                GenerateNewHallway();
            }*/
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsServer) { this.enabled = false; return; }
        Instance = this;

        currentHallway = hallways[0];
        hallwaysUntilNext = currentHallway.length;

        GenerateStartHallway();
    }

    public static void UpdateHallway()
    {
        Instance.UpdateHallwayRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateHallwayRPC()
    {
        Instance.TrashAmount--;
        if (Instance.trashAmount <= 0)
        {
            Instance.GenerateNewHallway();
        }
    }

    [ServerRpc]
    public void GenerateNewHallway()
    {
        HallwaysUntilNext--;
        print(trashAmount);

        GameObject door = currentHallwayObj.GetComponent<Hallway>().door.gameObject;
        if (door != null)
        {
            Despawn(door);
        }

        var pos = currentHallwayObj.transform.position + hallwayOffset;
        currentHallwayObj = Instantiate(currentHallway.prefab, pos, currentHallway.prefab.transform.rotation);
        Spawn(currentHallwayObj);

        _trashAmount += trashIncrement;
        trashAmount += _trashAmount;
        GenerateTrash(trashAmount);
    }

    [ServerRpc]
    public void GenerateStartHallway()
    {
        HallwaysUntilNext--;
        trashAmount = 2;
        GenerateTrash(trashAmount);
    }

    [ServerRpc]
    private void GenerateTrash(int amount)
    {
        trashList.Clear();

        for (int i = 0; i < amount; i++)
        {
            Vector3 generatedPos = currentHallwayObj.transform.position + new Vector3(
                (Random.value - 0.5f) * 2 * trashBounds.x,
                0,
                (Random.value - 0.5f) * 2 * trashBounds.z);

            GameObject prefab = trashPrefabs[Random.Range(0, trashPrefabs.Count)];
            GameObject trashInstance = Instantiate(prefab, generatedPos, Quaternion.identity);
            Spawn(trashInstance, LocalConnection);

            trashList.Add(trashInstance);
        }
    }
}
