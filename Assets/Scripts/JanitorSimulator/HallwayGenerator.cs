using System.Collections.Generic;
using UnityEngine;

public class HallwayGenerator : MonoBehaviour
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
    private List<GameObject> trashList = new List<GameObject>();

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
            if (trashAmount <= 0)
            {
                GenerateNewHallway();
            }
        }
    }

    public void Start()
    {
        Instance = this;

        currentHallway = hallways[0];
        hallwaysUntilNext = currentHallway.length;

        GenerateStartHallway();
    }

    public void GenerateNewHallway()
    {
        HallwaysUntilNext--;
        print(trashAmount);

        GameObject door = currentHallwayObj.GetComponent<Hallway>().door;
        if (door != null)
        {
            Destroy(door);
        }

        var pos = currentHallwayObj.transform.position + hallwayOffset;
        currentHallwayObj = Instantiate(currentHallway.prefab);
        currentHallwayObj.transform.position = pos;

        _trashAmount += trashIncrement;
        trashAmount += _trashAmount;
        GenerateTrash(trashAmount);
    }

    public void GenerateStartHallway()
    {
        HallwaysUntilNext--;
        trashAmount = 2;
        GenerateTrash(trashAmount);
    }

    private void GenerateTrash(int amount)
    {
        trashList.Clear();

        for (int i = 0; i < amount; i++)
        {
            var prefab = trashPrefabs[Random.Range(0, trashPrefabs.Count)];
            var trashInstance = Instantiate(prefab);

            trashInstance.transform.position = currentHallwayObj.transform.position + new Vector3(
                (Random.value - 0.5f) * 2 * trashBounds.x,
                0,
                (Random.value - 0.5f) * 2 * trashBounds.z);
            trashList.Add(trashInstance);
        }
    }
}
