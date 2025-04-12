using System;
using Steamworks.Data;
using Steamworks.ServerList;
using UnityEngine;

public class ServerList : MonoBehaviour
{
    public ServerItem serverItemPrefab;
    Base Request;

    void Start()
    {
        ChangeRequestType();
        Request.OnChanges += OnServersUpdated;
    }

    public void ChangeRequestType(bool lan = false)
    {
        Request = lan ? new Steamworks.ServerList.LocalNetwork() : new Steamworks.ServerList.Internet();
        Request.UpdateResponsive();
    }

    void OnServersUpdated()
    {
        // No reponsive servers yet, bail
        if (Request.Responsive.Count == 0)
            return;

        // Process each responsive server
        foreach (var child in FindObjectsByType<ServerItem>(FindObjectsSortMode.None))
        {
            Destroy(child.gameObject);
        }
        foreach (var s in Request.Responsive)
        {
            ServerResponded(s);
        }

        // Clear the responsive server list so we don't
        // reprocess them on the next call.
        Request.Responsive.Clear();
    }

    void ServerResponded(ServerInfo server)
    {
        // Do whatever you want with the server information
        var item = Instantiate(serverItemPrefab, this.transform);
        item.Set(server.Name, $"{server.Players}/{server.MaxPlayers}", server.SteamId);
        print($"{server.Name} Responded!");
    }
}
