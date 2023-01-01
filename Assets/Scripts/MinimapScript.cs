using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapScript : MonoBehaviour
{
    [Header("Maps")]
    [SerializeField] private GameObject _minimap;
    [SerializeField] private GameObject _megamap;
    [Space(10), Header("Camera")]
    [SerializeField] private Camera _cam;
    [SerializeField] private float _camMarginX;
    [SerializeField] private float _camMarginZ;
    [Space(10), Header("References")]
    [SerializeField] private GameObject _player;

    void Update()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        // Calculating camera margins to get camera's center point
        _camMarginX = (_cam.orthographicSize * 3f) / 2f;
        _camMarginZ = (_cam.orthographicSize * 2f) / 2f;
        //
        if (_player.gameObject.GetComponent<PlayerScript>().MapOff)
        {
            _minimap.SetActive(true);
            _megamap.SetActive(false);
            _cam.orthographicSize = 5f;
            //Vector3 playerPos = new Vector3(_player.transform.position.x, 5f, _player.transform.position.z);
            //playerPos.x = Mathf.Clamp(playerPos.x, _camMarginX - 0.5f, (_mineGenScript.MapHeight - 0.5f) - _camMarginX);
            //playerPos.z = Mathf.Clamp(playerPos.z, _camMarginZ - 0.5f, (_mineGenScript.MapWidth - 0.5f) - _camMarginZ);
            //transform.position = playerPos;

            transform.position = new Vector3(Mathf.Clamp(_player.transform.position.x, _camMarginX - 0.5f, (GameManager.Instance.MineControllerScript._MapHeight - 0.5f) - _camMarginX),
                5f, Mathf.Clamp(_player.transform.position.z, _camMarginZ - 0.5f, (GameManager.Instance.MineControllerScript._MapWidth - 0.5f) - _camMarginZ));
        }
        else
        {
            _minimap.SetActive(false);
            _megamap.SetActive(true);
            _cam.orthographicSize = Mathf.Max(GameManager.Instance.MineControllerScript._MapHeight / 3f, GameManager.Instance.MineControllerScript._MapWidth / 2f);
            transform.position = new Vector3((GameManager.Instance.MineControllerScript._MapHeight / 2f) - 0.5f, 5f, (GameManager.Instance.MineControllerScript._MapWidth / 2f) - 0.5f);

            // size 5 => 13.33x10 / map 20x10 => 

            // NEED TO CHECK ON CLICK & DRAG STUFF

        }
    }
}
