using BasicGameStuff;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static List<GameObject> unlockedGadgets = new();

    public TextMeshProUGUI statsText;
    public Button startShiftButton;
    public List<GameObject> allGadgets;

    private void Start()
    {
        PlayerController.MouseCaptured = false;

        statsText.text = $"Muns: {EconomySystem.MoneyBux}\nShifts: {SaveLoadSystem.GameSave.shifts}";

        foreach (var value in SaveLoadSystem.GameSave.unlockedGadgets)
        {
            unlockedGadgets.Add(allGadgets[value]);
        }
    }

    private int price = 0;

    public void SetPrice(int price)
    {
        this.price = price;
    }

    public void BuyGadget(GameObject gadget)
    {
        if (EconomySystem.MoneyBux >= price)
        {
            unlockedGadgets.Add(gadget);

            var b = SaveLoadSystem.GameSave.unlockedGadgets == null || SaveLoadSystem.GameSave.unlockedGadgets.Length == 0;
            var list = b ? new List<int>() : SaveLoadSystem.GameSave.unlockedGadgets.ToList();
            list.Add(allGadgets.IndexOf(gadget));
            SaveLoadSystem.GameSave.unlockedGadgets = list.ToArray();

            EconomySystem.MoneyBux -= price;
        }
    }

    public void SetFreeplay(bool b)
    {
        ShiftManager.isFreePlay = b;
    }

    public void StartShit()
    {
        Game.CreateLobby();
    }

    public void JoinById(string id)
    {
        if (ulong.TryParse(id, out ulong result))
            Game.JoinByID(new(result));
        else
        {
            Debug.LogError("Could not join Lobby");
        }
    }
}
