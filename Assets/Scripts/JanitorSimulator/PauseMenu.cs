using BasicGameStuff;
using FishNet.Managing;
using NUnit.Framework;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    public Slider sensitivitySlider;
    public Toggle invertMouseY;
    public Toggle invertMouseX;
    public TMP_Dropdown resolutionDropdown;

    private double sensitivity = 0.2;
    private bool invertX = false;
    private bool invertY = false;
    private Resolution[] resolutions;
    private List<Resolution> newResolutions = new();
    public TMP_Text lobbyId;

    public void Awake()
    {
        Instance = this;
        Debug.Log(res);
        gameObject.SetActive(false);

        sensitivitySlider.onValueChanged.AddListener((value) =>
        {
            sensitivity = value;
            UpdateSensitivity();
        });

        invertMouseX.onValueChanged.AddListener((b) =>
        {
            invertX = b;
            UpdateSensitivity();
        });

        invertMouseY.onValueChanged.AddListener((b) =>
        {
            invertY = b;
            UpdateSensitivity();
        });
    }

    private void OnEnable()
    {
        lobbyId.text = Game.CurrentLobbyID.ToString();
    }

    public void CopyLobbyID()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = Game.CurrentLobbyID.ToString();
        textEditor.SelectAll();
        textEditor.Copy();
    }

    private void UpdateSensitivity()
    {
        var sensitivityX = invertX ? -sensitivity : sensitivity;
        var sensitivityY = invertY ? -sensitivity : sensitivity;

        PlayerController.Instance.xSensitivity = (float)sensitivityX;
        PlayerController.Instance.ySensitivity = (float)sensitivityY;
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    Resolution res;
    public void SetResolution(int index)
    {
        Debug.Log(res);
        resolutions = Screen.resolutions;
        Debug.Log(resolutions);
        resolutionDropdown.ClearOptions();

        var options = new List<string>();
        int currentResIndex = 0;

        foreach (var resolution in resolutions)
        {
            if (resolution.width / 16 == resolution.height / 9)
            {
                options.Add($"{resolution.width}x{resolution.height}@{resolution.refreshRateRatio}");
                newResolutions.Add(resolution);
            }

            var currentRes = Screen.currentResolution;
            if (resolution.width == currentRes.width &&
                resolution.height == currentRes.height &&
                resolution.refreshRateRatio.value == currentRes.refreshRateRatio.value)
            {
                currentResIndex = resolutions.ToList().IndexOf(resolution);
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();

        Debug.Log(res);
        res = newResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode, res.refreshRateRatio);
        Debug.Log(res);
    }

    public void Exit()
    {
        NetworkManager.Instances[0].ClientManager.StopConnection();
        if (NetworkManager.Instances[0].IsServer) NetworkManager.Instances[0].ServerManager.StopConnection(true);
        SteamMatchmaking.LeaveLobby(new(Game.CurrentLobbyID));
        SceneManager.LoadScene(0);
    }
}
