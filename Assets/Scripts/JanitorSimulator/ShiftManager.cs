using System.Collections;
using FishNet.Object;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShiftManager : MonoBehaviour
{
    public static ShiftManager Instance { get; private set; }

    public static float shiftLength = 2 * 60;
    public static bool isFreePlay = false;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        foreach(var gadget in MainMenuManager.unlockedGadgets)
        {
            GameManagerA.Singleton.SpawnGadget(gadget.GetComponent<NetworkObject>(), GameManagerA.Singleton.LocalConnection);
        }

        if (isFreePlay)
        {
            yield break;
        }

        yield return new WaitForSeconds(shiftLength);

        SaveLoadSystem.GameSave.shifts++;
        SaveLoadSystem.Save();
        SceneManager.LoadScene(0);
    }
}
