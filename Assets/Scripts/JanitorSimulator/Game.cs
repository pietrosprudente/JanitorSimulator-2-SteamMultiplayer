using System;
using System.Linq;
using FishNet.Component.Spawning;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using Steamworks;
using Steamworks.Data;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    public static Lobby CurrentLobby { get; private set; }
    public static ulong CurrentLobbyID;
    public NetworkManager manager;
    [SerializeField] private FishyFacepunch.FishyFacepunch _FishyFacepunch;

    void Start()
    {
        Instance = this;
        manager = GetComponent<NetworkManager>();
        _FishyFacepunch = GetComponent<FishyFacepunch.FishyFacepunch>();
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested += OnJoinRequest;
        SteamMatchmaking.OnLobbyMemberDisconnected += OnLeave;
    }

    private void OnLeave(Lobby arg1, Friend arg2)
    {
        LeaveLobby();
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
        SteamMatchmaking.CreateLobbyAsync(16);
    }

    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        //Debug.Log("Starting lobby creation: " + callback.m_eResult.ToString());
        if (result != Result.OK)
            return;

        CurrentLobby = lobby;
        CurrentLobbyID = lobby.Id;

        lobby.SetPublic();
        lobby.SetData("HostAddress", SteamClient.SteamId.ToString());
        lobby.SetData("name", SteamClient.Name + "'s lobby");
        _FishyFacepunch.SetClientAddress(SteamClient.SteamId.ToString());
        _FishyFacepunch.StartConnection(true);
        LoadLevel();
        Debug.Log("Lobby creation was successful");
    }

    private void OnJoinRequest(Lobby lobby, SteamId id)
    {
        SteamMatchmaking.JoinLobbyAsync(id);
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        CurrentLobbyID = lobby.Id;

        _FishyFacepunch.SetClientAddress(lobby.Id.Value.ToString());
        _FishyFacepunch.StartConnection(false);
    }

    public static void JoinByID(SteamId steamID)
    {
        Debug.Log("Attempting to join lobby with ID: " + steamID.Value);
        try
        {
            SteamMatchmaking.JoinLobbyAsync(steamID);
        }
        catch (Exception e)
        {
            Debug.Log("Failed to join lobby with ID: " + steamID.Value);
        }
    }

    public static void LeaveLobby()
    {
        CurrentLobbyID = 0;
        CurrentLobbyID = 0;

        Instance._FishyFacepunch.StopConnection(false);
        if (Instance.manager.IsServer)
            Instance._FishyFacepunch.StopConnection(true);
    }
}
