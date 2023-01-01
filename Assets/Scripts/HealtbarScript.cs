using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealtbarScript : MonoBehaviour
{
    [SerializeField] private float _minWidth = 0f;
    [SerializeField] private float _maxWidth = 100f;
    [SerializeField] private float _height = 50f;
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private Color _healthBarColor;
    [SerializeField] private GameObject _emptyBar;
    [SerializeField] private Color _emptyBarColor;
    private PlayerScript _playerScript;
    
    void Update()
    {
        if (_playerScript == null)
        {
            _playerScript = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PlayerScript>();
        }
        _healthBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Max(_playerScript.Health * 4.8f, _minWidth), _height);
        _healthBar.gameObject.GetComponent<CanvasRenderer>().SetColor(_healthBarColor);
        _emptyBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Min((_maxWidth - _playerScript.Health) * 4.8f, (_maxWidth * 4.8f)), _height);
        _emptyBar.gameObject.GetComponent<CanvasRenderer>().SetColor(_emptyBarColor);
    }
}
