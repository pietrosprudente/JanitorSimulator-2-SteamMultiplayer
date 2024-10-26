using System.Collections.Generic;
using UnityEngine;

public class HallwayGenerator : MonoBehaviour
{
    public static HallwayGenerator Instance { get; private set; }

    public GameObject hallwayNode;
    public Vector3 hallwayOffset;
    public List<GameObject> trashPrefabs;
    public Vector3 trashBounds = new Vector3(10, 0, 10);

    private int trashAmount = 2;
    private int _trashAmount = 2;
    private int trashIncrement = 1;
    private List<GameObject> trashList = new List<GameObject>();

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
        GenerateStartHallway();
    }

    public void GenerateNewHallway()
    {
        print(trashAmount);

        GameObject door = hallwayNode.GetComponent<Hallway>().door;
        if (door != null)
        {
            Destroy(door);
        }

        hallwayNode = Instantiate(hallwayNode);
        hallwayNode.transform.Translate(hallwayOffset);

        _trashAmount += trashIncrement;
        trashAmount += _trashAmount;
        GenerateTrash(trashAmount);
    }

    public void GenerateStartHallway()
    {
        print(trashAmount +"kjsdkdf");
        trashAmount = 50;
        GenerateTrash(trashAmount);
    }

    private void GenerateTrash(int amount)
    {
        trashList.Clear();

        for (int i = 0; i < amount; i++)
        {
            var prefab = trashPrefabs[Random.Range(0, trashPrefabs.Count)];
            var trashInstance = Instantiate(prefab);

            trashInstance.transform.position = hallwayNode.transform.position + new Vector3(
                (Random.value - 0.5f) * 2 * trashBounds.x,
                0,
                (Random.value - 0.5f) * 2 * trashBounds.z);
            trashList.Add(trashInstance);
        }
    }
}
