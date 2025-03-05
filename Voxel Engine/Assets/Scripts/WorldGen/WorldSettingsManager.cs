using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSettingsManager : MonoBehaviour
{
    public static WorldSettingsManager instance;

    [SerializeField] private Rigidbody _playerRB;
    [SerializeField] private Canvas _worldGenMenu;
    [SerializeField] private Canvas _playerHUD;

    public bool _isMenuOpen = true;
    private bool _worldGenerated = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isMenuOpen && _worldGenerated)
            {
                CloseMenu();
            }
            else if (!_isMenuOpen)
            {
                OpenMenu();
            }
        }
    }

    public void OpenMenu()
    {
        _playerRB.useGravity = false;

        _worldGenMenu.enabled = true;
        _playerHUD.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _isMenuOpen = true;
    }

    public void CloseMenu()
    {
        _playerRB.useGravity = true;

        _worldGenMenu.enabled = false;
        _playerHUD.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _isMenuOpen = false;
    }

    public void StartSim()
    {
        World.instance.NewWorld();
        _worldGenerated = true;

        CloseMenu();
    }

    public TextMeshProUGUI seedField;
    public Slider sizeXSlider;
    public Slider sizeYSlider;
    public Slider sizeZSlider;
    public Slider chunkSizeSlider;
    public Slider mountainWidthSlider;
    public Slider mountainHeightSlider;

    public void UpdateSeed()
    {
        string seedStr = seedField.text;
        Debug.Log(seedStr);
        int.TryParse(seedStr, out int seedInt);
        Debug.Log(seedInt);
        World.instance.SetSeed(seedInt);
    }
    public void UpdateWorldSizeX()
    {
        World.instance.SetWorldSizeX((int)sizeXSlider.value);
    }
    public void UpdateWorldSizeY()
    {
        World.instance.SetWorldSizeY((int)sizeYSlider.value);
    }
    public void UpdateWorldSizeZ()
    {
        World.instance.SetWorldSizeZ((int)sizeZSlider.value);
    }
    public void UpdateChunkSize()
    {
        World.instance.SetChunkSize((int)chunkSizeSlider.value);
    }
    public void UpdateMountainWidth()
    {
        World.instance.SetMountainWidth((int)mountainWidthSlider.value);
    }
    public void UpdateMountainHeight()
    {
        World.instance.SetMountainHeight((int)mountainHeightSlider.value);
    }
}
