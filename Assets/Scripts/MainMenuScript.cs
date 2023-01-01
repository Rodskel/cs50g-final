using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Button[] _menuButtons = new Button[3];
    [Space(10), Header("Play")]
    [SerializeField] private GameObject _playMenu;
    [SerializeField] private bool _playMenuExtended = false;
    [SerializeField] private Button[] _playMenuButtons = new Button[6];
    [Space(5)]
    [SerializeField] private GameObject _playCustomMenu;
    [SerializeField] private bool _playCustomMenuExtended = false;
    [Space(5)]
    [SerializeField] private int _playCustomMenuMapHeight;
    [SerializeField] private Slider _playCustomMenuMapHeightSlider;
    [SerializeField] private TextMeshProUGUI _playCustomMenuMapHeightText;
    [Space(5)]
    [SerializeField] private int _playCustomMenuMapWidth;
    [SerializeField] private Slider _playCustomMenuMapWidthSlider;
    [SerializeField] private TextMeshProUGUI _playCustomMenuMapWidthText;
    [Space(5)]
    [SerializeField] private float _playCustomMenuDifficulty;
    [SerializeField] private Slider _playCustomMenuDifficultySlider;
    [SerializeField] private TextMeshProUGUI _playCustomMenuDifficultyText;
    [Space(10), Header("Options")]
    [SerializeField] private GameObject _optionsMenu;
    [SerializeField] private bool _optionsMenuExtended = false;
    [SerializeField] private Button[] _optionsMenuButtons = new Button[3];
    [SerializeField] private Slider _optionsAudioLevelSlider;
    [Space(10), Header("Quit")]
    [SerializeField] private string _quitText;

    void Awake()
    {
        _playMenu.SetActive(false);
        _playCustomMenu.SetActive(false);
        _optionsMenu.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void ExtendPlayMenu()
    {
        if (!_playMenuExtended)
        {
            _optionsMenuExtended = false;
            _optionsMenu.SetActive(false);
            _playMenuExtended = true;
            _playMenu.SetActive(true);
            _playMenuButtons[0].Select();
        }
        else
        {
            _playMenuExtended = false;
            _playMenu.SetActive(false);
            _playCustomMenuExtended = false;
            _playCustomMenu.SetActive(false);
            _menuButtons[0].Select();
        }
    }

    public void ExtendPlayCustomMenu()
    {
        if (!_playCustomMenuExtended)
        {
            _playCustomMenuExtended = true;
            _playCustomMenu.SetActive(true);
            _playCustomMenuMapHeightSlider.Select();
        }
        else
        {
            _playCustomMenuExtended = false;
            _playCustomMenu.SetActive(false);
            _playMenuButtons[5].Select();
        }
    }

    public void CustomMapHeight()
    {
        _playCustomMenuMapHeight = (int)_playCustomMenuMapHeightSlider.value;
        _playCustomMenuMapHeightText.text = new string($"{_playCustomMenuMapHeight} tiles");
    }

    public void CustomMapWidth()
    {
        _playCustomMenuMapWidth = (int)_playCustomMenuMapWidthSlider.value;
        _playCustomMenuMapWidthText.text = new string($"{_playCustomMenuMapWidth} tiles");
    }

    public void CustomDifficulty()
    {
        _playCustomMenuDifficulty = _playCustomMenuDifficultySlider.value;
        _playCustomMenuDifficultyText.text = new string($"{Mathf.RoundToInt((_playCustomMenuMapHeight * _playCustomMenuMapWidth) * _playCustomMenuDifficulty)} mines");
    }

    public void StartGame(string definition)
    {
        int height = 0;
        int width = 0;
        float difficulty = 0f;
        switch (definition)
        {
            case "Beginner":
                height = 9;
                width = 9;
                difficulty = 0.1234567901234568f;
                break;
            case "Easy":
                height = 16;
                width = 9;
                difficulty = 0.1388888888888889f;
                break;
            case "Medium":
                height = 16;
                width = 16;
                difficulty = 0.1562500000000000f;
                break;
            case "Hard":
                height = 30;
                width = 16;
                difficulty = 0.2062500000000000f;
                break;
            case "Expert":
                height = 30;
                width = 30;
                difficulty = 0.2500000000000000f;
                break;
        }
        GameManager.Instance.StartPlaying(height, width, difficulty);
    }

    public void StartCustomGame()
    {
        GameManager.Instance.StartPlaying(_playCustomMenuMapHeight, _playCustomMenuMapWidth, _playCustomMenuDifficulty);
    }

    public void ExtendOptionsMenu()
    {
        if (!_optionsMenuExtended)
        {
            _playMenuExtended = false;
            _playMenu.SetActive(false);
            _playCustomMenuExtended = false;
            _playCustomMenu.SetActive(false);
            _optionsMenuExtended = true;
            _optionsMenu.SetActive(true);
            _optionsMenuButtons[QualitySettings.GetQualityLevel()].Select();
        }
        else
        {
            _optionsMenuExtended = false;
            _optionsMenu.SetActive(false);
            _menuButtons[1].Select();
        }
    }

    public void OptionsGraphics(int graphicSetting)
    {
        QualitySettings.SetQualityLevel(graphicSetting, true);
    }

    public void OptionsAudio()
    {
        GameManager.Instance.AudioEffectVolume = _optionsAudioLevelSlider.value;
    }

    public void QuitGame()
    {
        GameManager.Instance.ShowInfoWindow(_quitText, () => {
            Debug.Log("Quitting game.");
            Application.Quit();
        }, () => {
            _menuButtons[2].Select();
        });
    }

    public void MenuBack()
    {
        if (_playMenuExtended)
        {
            if (_playCustomMenuExtended)
            {
                _playCustomMenuExtended = false;
                _playCustomMenu.SetActive(false);
                _playMenuButtons[5].Select();
            }
            else
            {
                _playMenuExtended = false;
                _playMenu.SetActive(false);
                _menuButtons[0].Select();
            }
        }
        else if (_optionsMenuExtended)
        {
            _optionsMenuExtended = false;
            _optionsMenu.SetActive(false);
            _menuButtons[1].Select();
        }
        else
        {
            QuitGame();
        }
    }
}
