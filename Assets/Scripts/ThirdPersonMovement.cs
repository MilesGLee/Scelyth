using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Generic Movement")]
    [SerializeField] private Transform _cameraTransform; //The transform of the camera used to inherit its direction for movement
    [SerializeField] private float _moveSpeed; //How fast the player will move
    private CharacterController _controller; //The character controller that this object has
    private float _turnSmoothTime = 0.1f; //Used to smooth the turn rate of the players visual
    private float _turnSmoothVelocity; //Used to smooth the turn rate of the players visual
    private bool _grounded;
    

    //Temporary
    [Header("Temp")]
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _invincibleMaterial;
    [SerializeField] private Material _sprintingMaterial;
    [SerializeField] private MeshRenderer _bodyPart1;
    [SerializeField] private MeshRenderer _bodyPart2;

    [Header("Dodging and Sprinting")]
    //Sprint Variables
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _sprintSpeed;
    private float _sprintWindDown = 0.5f;
    private bool _isSprinting;
    private bool _sprintCheck;
    //Dodge Variables
    private float _invincibleDuration = 0.5f;
    private float _dodgeCooldown = 1.0f;
    private bool _isDodging;
    private bool _isInvincible;
    private bool _dodgeCheck;
    [SerializeField] private float _dodgeInputTime;


    private void Awake()
    {
        //On initialize, set default variables
        _controller = GetComponent<CharacterController>();
        _grounded = true;
        _moveSpeed = _walkSpeed;
        _sprintCheck = false;
        _isSprinting = false;

        //Temporary
        _bodyPart1.material = _defaultMaterial;
        _bodyPart2.material = _defaultMaterial;

        //Set cursor settings
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        #region Generic Moving
        //Get the horizontal and vertical input, defaults to WASD and Arrow controlls
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        //Create a vector 3 in the direction the player is inputting to move
        Vector3 direction = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;

        if (_isSprinting)
        {
            _moveSpeed = _sprintSpeed;
        }
        else 
        {
            _moveSpeed = _walkSpeed;
        }

        //If the direction is more than 0.1, which means if the player has inputted a move direction at all
        if (direction.magnitude >= 0.1f) 
        {
            //float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg; //This is an old line used if we don't want to inherit the direction from anything

            //Get the target angle the player needs to rotate towards, inheriting the cameras direction, so moving forwards will always be in the direction the camera is facing
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
            //Use math smooth damp angle to create a turn rate over time to rotate the player in, rather than having the player snap instantly to the desired angle
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            //Rotate the player transform
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

            //Create a move direction using the inhertied direction from the camera, and multiply it by the forward vector
            Vector3 moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            //_controller.Move(direction * _moveSpeed * Time.deltaTime); //This is an old line used if we don't want to inherit the direction from anything

            //Move the character controller in the move direction
            _controller.Move(moveDirection.normalized * _moveSpeed * Time.deltaTime);
        }
        #endregion

        #region Sprinting

        if (Input.GetKeyDown(KeyCode.Space) && !_sprintCheck) 
        {
            _sprintCheck = true;
            RoutineBehavior.Instance.StartNewTimedAction(args => OnSprintStart(), TimedActionCountType.SCALEDTIME, _sprintWindDown);
        }
        if (Input.GetKeyUp(KeyCode.Space) && _sprintCheck) 
        {
            _sprintCheck = false;
            RoutineBehavior.Instance.StartNewTimedAction(args => OnSprintEnd(), TimedActionCountType.SCALEDTIME, _sprintWindDown / 2);
        }

        #endregion

        #region Dodging
        if (Input.GetKeyDown(KeyCode.Space) && !_dodgeCheck) 
        {
            _dodgeCheck = true;
        }
        if (Input.GetKeyUp(KeyCode.Space) && _dodgeCheck) 
        {
            _dodgeCheck = false;
        }

        if (_dodgeCheck) 
        {
            _dodgeInputTime += Time.deltaTime;
        }

        #endregion

        if (_isInvincible)
        {
            _bodyPart1.material = _invincibleMaterial;
            _bodyPart2.material = _invincibleMaterial;
        }
        else if (_isSprinting)
        {
            _bodyPart1.material = _sprintingMaterial;
            _bodyPart2.material = _sprintingMaterial;
        }
        else if (!_isSprinting && !_isInvincible)
        {
            _bodyPart1.material = _defaultMaterial;
            _bodyPart2.material = _defaultMaterial;
        }
    }

    private void OnSprintStart() 
    {
        if (_sprintCheck)
        {
            _isSprinting = true;
        }
    }

    private void OnSprintEnd() 
    {
        _isSprinting = false;
    }

    private void OnDodge() 
    {
        _isDodging = true;
        _isInvincible = true;
        RoutineBehavior.Instance.StartNewTimedAction(args => _isDodging = false, TimedActionCountType.SCALEDTIME, _dodgeCooldown);
        RoutineBehavior.Instance.StartNewTimedAction(args => _isInvincible = false, TimedActionCountType.SCALEDTIME, _invincibleDuration);
    }
}
