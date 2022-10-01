using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorStateBehavior : MonoBehaviour
{
    private ThirdPersonMovement _thirdPersonMovement;
    [SerializeField] private Material _walkMaterial;
    [SerializeField] private Material _sprintMaterial;
    [SerializeField] private Material _dodgeMaterial;
    [SerializeField] private MeshRenderer[] _meshes;

    void Start()
    {
        _thirdPersonMovement = GetComponent<ThirdPersonMovement>();
    }

    void Update()
    {
        if (_thirdPersonMovement.MyMovementState == MovementState.WALKING)
            ApplyMaterial(_walkMaterial);
        else if (_thirdPersonMovement.MyMovementState == MovementState.SPRINTING)
            ApplyMaterial(_sprintMaterial);
        else if (_thirdPersonMovement.MyMovementState == MovementState.DODGING)
            ApplyMaterial(_dodgeMaterial);
    }

    void ApplyMaterial(Material mat) 
    {
        for(int i = 0; i < _meshes.Length; i++) 
        {
            _meshes[i].material = mat;
        }
    }
}
