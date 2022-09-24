using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _follow;
    [SerializeField] private Transform _lookAt;
    [Header("Axis Control")]
    private Vector3 _currentPosition;
    [Header("Orbits (X = Height, Y = Radius)")]
    [SerializeField] private Vector2 _topRig;
    [SerializeField] private Vector2 _middleRig;
    [SerializeField] private Vector2 _bottomRig;

    void Start()
    {
        _currentPosition = new Vector3(0.0f, _middleRig.x, _middleRig.y);
    }

    void Update()
    {
        
    }
}
