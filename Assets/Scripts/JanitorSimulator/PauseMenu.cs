using BasicGameStuff;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    public Slider sensitivitySlider;
    public Toggle invertMouseY;
    public Toggle invertMouseX;

    private double sensitivity = 0.2;
    private bool invertX = false;
    private bool invertY = false;

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

    private void UpdateSensitivity()
    {
        var sensitivityX = invertX ? -sensitivity : sensitivity;
        var sensitivityY = invertY ? -sensitivity : sensitivity;

        PlayerController.Instance.xSensitivity = (float)sensitivityX;
        PlayerController.Instance.ySensitivity = (float)sensitivityY;
    }
}
