using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<TrashItem>())
        {
            Destroy(other.gameObject);
            HallwayGenerator.Instance.TrashAmount--;
        }
    }
}
