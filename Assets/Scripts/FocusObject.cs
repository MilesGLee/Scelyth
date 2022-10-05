using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusObject : MonoBehaviour
{
    [SerializeField] private Transform _focusTarget;
    [SerializeField] private Collider _focusCollider;

    public Transform FocusTarget { get { return _focusTarget; } }
    public Collider FocusCollider { get { return _focusCollider; } }

    private void Awake()
    {
        if (_focusTarget == null)
            _focusTarget = transform;
    }
}
