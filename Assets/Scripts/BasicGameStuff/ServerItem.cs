using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerItem : Button
{
    public TMP_Text serverName;
    public TMP_Text serverPlayers;
    public SteamId serverId;

    protected override void Start()
    {
        onClick.AddListener(Join);
    }

    private void Join()
    {
        SteamMatchmaking.JoinLobbyAsync(serverId);
    }

    public void Set(string name, string players, ulong id)
    {
        serverName.text = name;
        serverPlayers.text = players;
        serverId = id;
    }
}
