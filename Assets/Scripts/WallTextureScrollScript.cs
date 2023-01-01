using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTextureScrollScript : MonoBehaviour
{
    [SerializeField] private float _scrollSpeed = 0.5f;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private Renderer _topWall;
    [SerializeField] private Renderer _leftWall;
    [SerializeField] private Renderer _rightWall;
    [SerializeField] private Renderer _bottomWall;

    private void Update()
    {
        // Update the texture offset
        _offset.x += _scrollSpeed * Time.deltaTime;

        // Apply the texture offset to all four walls
        _topWall.material.mainTextureOffset = _offset;
        _leftWall.material.mainTextureOffset = _offset;
        _rightWall.material.mainTextureOffset = _offset;
        _bottomWall.material.mainTextureOffset = _offset;
    }
}
