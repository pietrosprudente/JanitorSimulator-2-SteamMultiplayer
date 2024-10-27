using BasicGameStuff;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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

    private void Start()
    {
        resolutions = Screen.resolutions;
        Debug.Log(resolutions);
        resolutionDropdown.ClearOptions();

        var options = new List<string>();
        int currentResIndex = 0;

        foreach(var resolution in resolutions)
        {
            if(resolution.width / 16 == resolution.height / 9)
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
    }

    public void Awake()
    {
        Instance = this;
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

    private void Update()
    {
        Debug.Log(res);

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
        res = newResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode, res.refreshRateRatio);
        Debug.Log(res);
    }
}
