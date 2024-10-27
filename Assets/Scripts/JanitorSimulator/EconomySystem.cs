using UnityEngine;

public class EconomySystem : MonoBehaviour
{
    public static EconomySystem Instance { get; private set; }

    public static int MoneyBux
    {
        get
        {
            return SaveLoadSystem.GameSave.money;
        }
        set
        {
            SaveLoadSystem.GameSave.money = value;
        }
    }

    private void Awake()
    {
        if(SaveLoadSystem.GameSave == null)
        {
            SaveLoadSystem.Load();
        }
    }
}
