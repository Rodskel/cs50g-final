using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Player Attributes")]
    [Tooltip("Player maximum health.")]
    public float Health = 100f;
    public bool MapOff = false;
    public bool CanInput = true;
    [Space(10), Header("Extra Player Scripts")]
    public PlayerInputScript InputScript;
    public FirstPersonMovement MovementScript;
    public FirstPersonLook LookScript;
    public Jump JumpScript;
    public GroundCheck GroundCheckScript;
    public FirstPersonAudio AudioScript;
    [Space(10), Header("Other")]
    [Tooltip("FPS camera that player uses.")]
    [SerializeField] private GameObject _playerCam;
    [Tooltip("How far player can click.")]
    [SerializeField] private float _raycastDistance = 3f;
    [Tooltip("Player's minimap indicator.")]
    [SerializeField] private SpriteRenderer _mapIndicator;
    [Tooltip("Minimap indicator's color.")]
    [SerializeField] private Color _indicatorColor;

    void Start()
    {
        _mapIndicator.color = _indicatorColor;
        MapOff = true;
        _playerCam.tag = "MainCamera";
    }

    public Vector2Int RaycastFromPlayer()
    {
        RaycastHit hit;
        Physics.Raycast(_playerCam.transform.position, _playerCam.transform.forward, out hit, _raycastDistance);
        if (hit.transform.CompareTag("Tile"))
        {
            return new Vector2Int((int)Mathf.Round(hit.transform.position.x), (int)Mathf.Round(hit.transform.position.z));
        }
        else
        {
            return new Vector2Int(-1, -1);
        }
    }

    public void DamagePlayer(float damageTaken)
    {
        Health = Mathf.Max(0, Health - damageTaken);
        if (Health <= 0)
        {
            CanInput = false;
        }
    }
}
