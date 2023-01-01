using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CounterScript : MonoBehaviour
{
    [Header("Time Counter")]
    [SerializeField] private TextMeshProUGUI _timeCounterTMP;
    [SerializeField] private Color _timeCounterColor;
    [Space(10), Header("Mine Counter")]
    [SerializeField] private TextMeshProUGUI _mineCounterTMP;
    [SerializeField] private Color _mineCounterColor;

    void Update()
    {
        _timeCounterTMP.SetText(GameManager.Instance.GetFormattedTimer());
        _mineCounterTMP.SetText(string.Format("Mines:{0:000}/{1:000}", GameManager.Instance.MineControllerScript.FlaggedTilesCounter, GameManager.Instance.MineControllerScript._MineTotal));
    }
}
