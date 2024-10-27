using System.Collections;
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
            Instantiate(gadget);
        }

        if(isFreePlay)
        {
            yield break;
        }

        yield return new WaitForSeconds(shiftLength);

        SaveLoadSystem.GameSave.shifts++;
        SaveLoadSystem.Save();
        SceneManager.LoadScene(0);
    }
}
