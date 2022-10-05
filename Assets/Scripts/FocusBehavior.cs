using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FocusBehavior : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private CinemachineFreeLook _cinemachine;
    [SerializeField] private Transform _playerGFX;
    private FocusObject[] _focusObjects;
    private Transform _target;

    private InputManager _input;

    void Start()
    {
        _target = null;
        _camera = Camera.main;

        _focusObjects = GameObject.FindObjectsOfType<FocusObject>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2)) 
        {
            if (_target == null)
            {
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);
                float minDist = Mathf.Infinity;
                for (int i = 0; i < _focusObjects.Length; i++) 
                {
                    float dist = Vector3.Distance(_focusObjects[i].FocusCollider.transform.position, _camera.transform.position);
                    if (dist < minDist) 
                    {
                        if (GeometryUtility.TestPlanesAABB(planes, _focusObjects[i].FocusCollider.bounds)) 
                        {
                            _target = _focusObjects[i].FocusTarget;
                            minDist = dist;
                        }
                    }
                }
            }
            else 
            {
                //_cinemachine.LookAt = _playerGFX;
                _target = null;
            }
        }

        if (_target != null) 
        {
            //_cinemachine.LookAt = _target;
            _camera.transform.LookAt(_target);
        }
    }
}
