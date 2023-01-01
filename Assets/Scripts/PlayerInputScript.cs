using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputScript : MonoBehaviour
{
    [SerializeField] private PlayerScript _playerScript;


    void Update()
    {
        if (_playerScript.CanInput)
        {
            _playerScript.LookScript.enabled = true;
            _playerScript.MovementScript.enabled = true;
            _playerScript.JumpScript.enabled = true;
            if (_playerScript.MapOff)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Debug.Log("LMB pressed.");
                    //_mineGenScript.UncoverTile(_playerScript.RaycastFromPlayer());
                    GameManager.Instance.MineControllerScript.UncoverTile(_playerScript.RaycastFromPlayer());
                }
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Debug.Log("RMB pressed.");
                    GameManager.Instance.MineControllerScript.FlaggingTile(_playerScript.RaycastFromPlayer());
                    //_mineGenScript.FlaggingTile(_playerScript.RaycastFromPlayer());
                }
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    _playerScript.MapOff = false;
                }
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.Tab))
                {
                    _playerScript.MapOff = true;
                }
            }
        }
        else
        {
            _playerScript.LookScript.enabled = false;
            _playerScript.MovementScript.enabled = false;
            _playerScript.JumpScript.enabled = false;
        }
    }
}
