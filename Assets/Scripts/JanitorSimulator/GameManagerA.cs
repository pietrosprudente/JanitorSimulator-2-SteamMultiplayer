using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using BasicGameStuff;
using FishNet.Object.Synchronizing;

public class GameManagerA : NetworkBehaviour
{
    public static GameManagerA Singleton;

    void Start()
    {
        Singleton ??= this;
        InvokeRepeating(nameof(Save), 15, 15);
    }

    public void Save()
    {
        SaveLoadSystem.Save();
    }

    [ServerRpc]
    public void SpawnGadget(NetworkObject gadget, NetworkConnection player)
    {
        Spawn(Instantiate(gadget, Vector3.zero, Quaternion.identity), player);
        Debug.Log($"Spawned {gadget.name} for Client {player.ClientId}");
    }

    #region ChatSystem
    private static string[] nonoWords = {
        "Fuck",
        "Bitch",
        "Cunt",
        "Prick",
        "Hoe",
        "Whore",
        "Bastard",
        "Cock",
        "Shit",
        "Dick",
        "Nigger",
        "Address",
        "Home",
        "State",
        "County",
        "Twat",
        "Number",
        "Ass",
        "Sex",
        "Porn",
    };

    private string message = "";
    private string chatLog = "Welcome to Janitor Simulator, Be Kind and Please Chat :)";

    [ServerRpc(RequireOwnership = false)]
    public void SendChat(string sent, PlayerController player)
    {
        string blockReason = "";
        print($"Server: Received Message {sent} from {player.nickname} ({player.OwnerId})");

        if (BlockDetector(sent, ref blockReason))
        {
            print($"Server: Message Blocked from {player.nickname} ({player.OwnerId}), {blockReason}!");
        }
        else
        {
            string message = $"<{player.nickname}({player.OwnerId})>: {sent}";
            ReceiveChat(message);
        }
    }

    private static bool BlockDetector(string message, ref string blockReason)
    {
        bool messageBlocked = false;
        for (int i = 0; i < nonoWords.Length; i++)
        {
            if (message.Contains(nonoWords[i], System.StringComparison.OrdinalIgnoreCase))
            {
                messageBlocked = true;
                blockReason = "Inappropiate";
            }
        }
        if (string.IsNullOrEmpty(message))
        {
            blockReason = "Empty lmao";
        }
        return messageBlocked;
    }

    [ObserversRpc(ExcludeOwner = false, ExcludeServer = false)]
    public void ReceiveChat(string received)
    {
        print(received);
        chatLog += $"\n {message}";
    }

    Vector2 scrollViewPos = Vector2.down;

    bool crapGraphics = false;
    void OnGUI()
    {
        GUILayout.TextField("Chat!");
        if (PauseMenu.Instance.gameObject.activeSelf)
        {
            message = GUILayout.TextArea(message, maxLength: 50);
            if (GUILayout.Button("send message"))
            {
                string blockReason = "";
                if (BlockDetector(message, ref blockReason)) print("Message Blocked " + blockReason);
                else
                {
                    message = "";
                    SendChat(message, PlayerController.Instance);
                }
            }
        }
        scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);
        GUILayout.TextArea(chatLog);
        GUILayout.EndScrollView();
        if (PauseMenu.Instance.gameObject.activeSelf)
        {
            crapGraphics = GUILayout.Toggle(crapGraphics, "Potato Mode");
            if (crapGraphics)
            {
                QualitySettings.SetQualityLevel(0);
            }
            else
            {
                QualitySettings.SetQualityLevel(1);
            }
        }
    }

    private void ClearChat()
    {
        chatLog = "Welcome to Janitor Simulator, Be Kind and Please Chat :)";
    }

    #endregion
}
