using System.Linq;
using FishNet.Component.Spawning;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using Steamworks;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    protected Callback<LobbyCreated_t> LbCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinReq;
    protected Callback<LobbyEnter_t> LbJoined;

    public static Game Instance { get; private set; }
    public static ulong CurrentLobbyID;
    public NetworkManager manager;
    [SerializeField] private FishySteamworks.FishySteamworks _fishySteamworks;

    void Start()
    {
        Instance = this;
        manager = GetComponent<NetworkManager>();
        _fishySteamworks = GetComponent<FishySteamworks.FishySteamworks>();
        LbCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinReq = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LbJoined = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void LoadLevel()
    {
        SceneLoadData sld = new("SampleScene");
        sld.ReplaceScenes = ReplaceOption.All;
        NetworkConnection[] conns = manager.ServerManager.Clients.Values.ToArray();
        manager.SceneManager.LoadConnectionScenes(conns, sld);
        manager.SceneManager.LoadGlobalScenes(sld);
    }

    public static void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        //Debug.Log("Starting lobby creation: " + callback.m_eResult.ToString());
        if (callback.m_eResult != EResult.k_EResultOK)
            return;

        CurrentLobbyID = callback.m_ulSteamIDLobby;
        SteamMatchmaking.SetLobbyData(new CSteamID(CurrentLobbyID), "HostAddress", SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(CurrentLobbyID), "name", SteamFriends.GetPersonaName().ToString() + "'s lobby");
        _fishySteamworks.SetClientAddress(SteamUser.GetSteamID().ToString());
        _fishySteamworks.StartConnection(true);
        LoadLevel();
        Debug.Log("Lobby creation was successful");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = callback.m_ulSteamIDLobby;

        _fishySteamworks.SetClientAddress(SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "HostAddress"));
        _fishySteamworks.StartConnection(false);
    }

    public static void JoinByID(CSteamID steamID)
    {
        Debug.Log("Attempting to join lobby with ID: " + steamID.m_SteamID);
        if (SteamMatchmaking.RequestLobbyData(steamID))
            SteamMatchmaking.JoinLobby(steamID);
        else
            Debug.Log("Failed to join lobby with ID: " + steamID.m_SteamID);
    }

    public static void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(new CSteamID(CurrentLobbyID));
        CurrentLobbyID = 0;

        Instance._fishySteamworks.StopConnection(false);
        if (Instance.manager.IsServer)
            Instance._fishySteamworks.StopConnection(true);
    }
}
