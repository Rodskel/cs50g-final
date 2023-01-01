using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    private string[] _states = {"StartState", "PlayState", "EndState"};
    public string CurrentState = "StartState";
    [Space(10), Header("Start")]
    public MainMenuScript MainMenuScript;
    [Space(10), Header("Play")]
    public int MapHeightSetting;
    public int MapWidthSetting;
    public float DifficultySetting;
    public MineControllerScript MineControllerScript;
    public PlayerScript PlayerScript;
    public float _PlayTimer {get; private set;} = 0;
    private bool _countingTime = false;
    private bool _countingOver = false;
    [SerializeField] private AudioClip[] _gameOverSounds;
    public bool IsGameWon;
    [Space(10), Header("End")]
    public GameOverScript GameOverScript;
    [Space(10), Header("Settings")]
    [Range(0f, 1f)] public float AudioEffectVolume = 1f;
    [Space(10), Header("Info Window")]
    [SerializeField] private GameObject _infoWindow;
    [SerializeField] private TextMeshProUGUI _infoWindowText;
    [SerializeField] private Button _infoWindowYesButton;
    [SerializeField] private Button _infoWindowNoButton;
    [Space(10), Header("Loading Screen")]
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private Slider _loadingScreenLoadBar;
    [Space(10), Header("Fade To Black")]
    [SerializeField] private Image _fadeToBlack;
    [SerializeField, Range(0f, 1f)] private float _fadeToBlackAlpha;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void ChangeState(string state)
    {
        foreach (string states in _states)
        {
            if (state == states)
            {
                CurrentState = state;
            }
        }
    }

    void Update()
    {
        if (CurrentState != $"{SceneManager.GetActiveScene().name}State")
        {
            ChangeState($"{SceneManager.GetActiveScene().name}State");
        }
        switch (CurrentState)
        {
            case "StartState":
                StartStateUpdate();
                break;
            case "PlayState":
                PlayStateUpdate();
                break;
            case "EndState":
                EndStateUpdate();
                break;
        }
    }

    private void StartStateUpdate()
    {
        if (MainMenuScript == null)
        {
            MainMenuScript = GameObject.Find("Main Menu").GetComponent<MainMenuScript>();
        }
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Backspace))
        {
            MainMenuScript.MenuBack();
        }
    }

    private void PlayStateUpdate()
    {
        if (MineControllerScript == null)
        {
            MineControllerScript = GameObject.Find("MineContainer").GetComponent<MineControllerScript>();
        }
        if (PlayerScript == null)
        {
            PlayerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        }
        // Time counter
        if  (_countingTime && !_countingOver)
        {
            _PlayTimer += Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Backspace))
        {
            if (!_infoWindow.activeInHierarchy)
            {
                PauseTimer();
                PlayerScript.CanInput = false;
                MineControllerScript.GetComponentInChildren<AudioSource>().Pause();
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.Confined;
                ShowInfoWindow("Do you want to return to the main menu?", () => {
                    Debug.Log("Returning to the Main Menu.");
                    MineControllerScript.GetComponentInChildren<AudioSource>().UnPause();
                    Time.timeScale = 1f;
                    StartCoroutine(LoadScene("Start"));
                }, () => {
                    Cursor.lockState = CursorLockMode.Locked;
                    PlayerScript.CanInput = true;
                    MineControllerScript.GetComponentInChildren<AudioSource>().UnPause();
                    Time.timeScale = 1f;
                    StartTimer();
                });
            }
            else
            {
                _infoWindow.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                PlayerScript.CanInput = true;
                MineControllerScript.GetComponentInChildren<AudioSource>().UnPause();
                Time.timeScale = 1f;
                StartTimer();
            }
        }
    }
    
    private void EndStateUpdate()
    {
        if (GameOverScript == null)
        {
            GameOverScript = GameObject.Find("End Screen Controller").GetComponent<GameOverScript>();
        }
        if (_fadeToBlack.gameObject.activeInHierarchy)
        {
            Time.timeScale = 1f;
            float transitionTimer = 0f;
            do
            {
                transitionTimer += Time.deltaTime;
                _fadeToBlackAlpha = Mathf.Lerp(1f, 0f, transitionTimer);
                _fadeToBlack.color = new Color(0f, 0f, 0f, _fadeToBlackAlpha);
            } while (transitionTimer < 1f);
            _fadeToBlack.gameObject.SetActive(false);
        }
    }

    public IEnumerator LoadScene(string sceneName)
    {
        Debug.Log("Coroutine Started.");
        _loadingScreenLoadBar.value = 0f;
        _loadingScreen.SetActive(true);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        Debug.Log("Scene is started loading & deactivated ");
        while (!asyncOperation.isDone)
        {
            float loadProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            _loadingScreenLoadBar.value = loadProgress;
            Debug.Log($"Progress bar's updated, {loadProgress}.");
            if (loadProgress > 0.9f)
            {
                break;
            }
            yield return null;
        }
        ChangeState($"{sceneName}State");
        _loadingScreen.SetActive(false);
        asyncOperation.allowSceneActivation = true;
        Debug.Log("Scene activated, breaking coroutine.");
    }

    public void StartPlaying(int height, int width, float difficulty)
    {
        MapHeightSetting = height;
        MapWidthSetting = width;
        DifficultySetting = difficulty;
        RestartTimer();
        Debug.Log("Settings assigned, loading Play scene.");
        StartCoroutine(LoadScene("Play"));
    }
    
    public void StartTimer()
    {
        _countingTime = true;
    }

    public void PauseTimer()
    {
        _countingTime = false;
    }

    public void StopTimer()
    {
        _countingOver = true;
    }

    public void RestartTimer()
    {
        _countingOver = false;
        _PlayTimer = 0f;
        StartTimer();
    }

    public string GetFormattedTimer()
    {
        // Seperating timer into parts.
        int milisecs = Mathf.FloorToInt((_PlayTimer % 1) * 100);
        int seconds = Mathf.FloorToInt(_PlayTimer % 60);
        int minutes = Mathf.FloorToInt(_PlayTimer / 60);
        int hours = Mathf.FloorToInt(_PlayTimer / 3600);
        if (hours > 0)
        {
            // If we're over 1 hour
            return new string(string.Format("{0:00}:{1:00}:{2:00}:{3:00}", hours, minutes, seconds, milisecs));
        }
        else
        {
            if (minutes > 0)
            {
                // If we're over 1 minute.
                return new string(string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milisecs));
            }
            else
            {
                // Always showing at least 1 second.
                return new string(string.Format("{0:00}:{1:00}", seconds, milisecs));
            }
        }
    }

    public IEnumerator GameOver(bool gameWon)
    {
        switch (gameWon)
        {
            case true:
                IsGameWon = true;
                break;
            case false:
                IsGameWon = false;
                PlayerScript.CanInput = false;
                Time.timeScale = 0.5f;
                break;
        }
        float transitionTimer = 0.00f;
        if (!MineControllerScript.gameObject.GetComponent<AudioSource>().isPlaying)
        {
            if (gameWon)
            {
                MineControllerScript.gameObject.GetComponent<AudioSource>().PlayOneShot(_gameOverSounds[1], AudioEffectVolume);

            }
            else
            {
                MineControllerScript.gameObject.GetComponent<AudioSource>().PlayOneShot(_gameOverSounds[0], AudioEffectVolume);
            }
        }
        _fadeToBlack.gameObject.SetActive(true);
        do
        {
            transitionTimer += Time.unscaledDeltaTime;
            if (transitionTimer >= 4f && gameWon)
            {
                _fadeToBlackAlpha = Mathf.Lerp(0f, 1f, transitionTimer % 1);
                _fadeToBlack.color = new Color(0f, 0f, 0f, _fadeToBlackAlpha);
            }
            if (!gameWon)
            {
                _fadeToBlackAlpha = Mathf.Lerp(0f, 1f, transitionTimer / 5f);
                _fadeToBlack.color = new Color(0f, 0f, 0f, _fadeToBlackAlpha);
            }
            yield return null;
        } while (transitionTimer < 5f);
        StartCoroutine(LoadScene("End"));
    }

    public void ShowInfoWindow(string text, Action yesAction, Action noAction)
    {
        _infoWindow.SetActive(true);
        _infoWindowText.text = text;
        _infoWindowYesButton.Select();
        _infoWindowYesButton.onClick.AddListener(() => {
            _infoWindow.SetActive(false);
            yesAction();
        });
        _infoWindowNoButton.onClick.AddListener(() => {
            _infoWindow.SetActive(false);
            noAction();
        });
    }
}
