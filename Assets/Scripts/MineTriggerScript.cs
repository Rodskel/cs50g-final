using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineTriggerScript : MonoBehaviour
{
    [SerializeField] private TileScript _tileScript;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && _tileScript.IsMine && !_tileScript.IsFlagged && !_tileScript.IsMineExploded)
        {
            Debug.Log(string.Format("Collision with {0} triggered!", _tileScript.gameObject.name));
            //
            _tileScript.IsExploding = true;
        }
    }
}
