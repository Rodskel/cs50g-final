using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScript : MonoBehaviour
{
    [Header("Game Over Text")]
    [SerializeField] private RawImage _gameOverBackground;
    [SerializeField] private Color _winColor;
    [SerializeField] private Color _loseColor;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [Space(10), Header("Time")]
    [SerializeField] private TextMeshProUGUI _timerTMP;
    [Space(10), Header("Minefield")]
    [SerializeField] private TextMeshProUGUI _minefieldTMP;

    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        switch (GameManager.Instance.IsGameWon)
        {
            case true:
                _gameOverBackground.color = _winColor;
                _gameOverText.text = "MINEFIELD CLEAR!";
                break;
            case false:
                _gameOverBackground.color = _loseColor;
                _gameOverText.text = "YOU ARE DEAD!";
                break;
        }
        _timerTMP.text = GameManager.Instance.GetFormattedTimer();
        _minefieldTMP.text = $"{GameManager.Instance.MapHeightSetting}x{GameManager.Instance.MapWidthSetting} with {(GameManager.Instance.MapHeightSetting * GameManager.Instance.MapWidthSetting) * GameManager.Instance.DifficultySetting} mines";
        
    }
    public void GoBackToMainMenu()
    {
        StartCoroutine(GameManager.Instance.LoadScene("Start"));
    }

    public void RestartGameSameSettings()
    {
        GameManager.Instance.StartPlaying(GameManager.Instance.MapHeightSetting, GameManager.Instance.MapWidthSetting, GameManager.Instance.DifficultySetting);
    }
}
