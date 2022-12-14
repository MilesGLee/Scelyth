using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementState 
{
    WALKING,
    SPRINTING,
    DODGING
}

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
    private bool _hasControl;
    private bool _mobilityInput;
    private bool _mobilityTrigger;
    private float _mobilityInputTime;
    private float _mobilityThreshold = 0.5f;
    [SerializeField] private FocusBehaviour _focus;
    private MovementState _myMovementState;
    public MovementState MyMovementState { get { return _myMovementState; } }

    [Header("Dodging and Sprinting")]
    //Sprint Variables
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _sprintSpeed;
    private float _sprintWindDown = 0.5f;
    private bool _isSprinting;
    //Dodge Variables
    private float _invincibleDuration = 0.5f;
    private bool _canDodge;
    private float _dodgeCooldown = 1.0f;
    private bool _isInvincible;
    private float _dodgeAngle;


    private void Awake()
    {
        //On initialize, set default variables
        _controller = GetComponent<CharacterController>();
        _grounded = true;
        _moveSpeed = _walkSpeed;
        _isSprinting = false;
        _myMovementState = MovementState.WALKING;
        _canDodge = true;
        _hasControl = true;

        //Set cursor settings
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MyInput();
        Dodge();
        Sprint();

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

        if (direction.magnitude >= 0.1f && _hasControl) 
        {
            if (!_focus.IsFocused)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
                Vector3 moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;

                _controller.Move(moveDirection.normalized * _moveSpeed * Time.deltaTime);
            }
            if (_focus.IsFocused) 
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _cameraTransform.eulerAngles.y, ref _turnSmoothVelocity, _turnSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
                Vector3 moveDir = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;

                _controller.Move(moveDir.normalized * _moveSpeed * Time.deltaTime);
            }
        }
        #endregion
    }

    private void MyInput() 
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_mobilityInput) 
        {
            _mobilityInputTime = 0;
            _mobilityInput = true;
        }
        if (Input.GetKeyUp(KeyCode.Space) && _mobilityInput)
        {
            _mobilityInput = false;
            _mobilityTrigger = true;
        }
        if (_mobilityInput) 
        {
            _mobilityInputTime += Time.deltaTime;
        }
    }

    private void Dodge() 
    {
        if (_mobilityTrigger)
        {
            _mobilityTrigger = false;
            if (_mobilityInputTime <= _mobilityThreshold && _canDodge)
            {
                float inputX = Input.GetAxisRaw("Horizontal");
                float inputY = Input.GetAxisRaw("Vertical");
                Vector3 direction = new Vector3(inputX, 0.0f, inputY).normalized;
                _dodgeAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;

                _canDodge = false;
                _hasControl = false;
                _isInvincible = true;
                _myMovementState = MovementState.DODGING;
                RoutineBehaviour.Instance.StartNewTimedAction(args => _isInvincible = false, TimedActionCountType.SCALEDTIME, _invincibleDuration);
                RoutineBehaviour.Instance.StartNewTimedAction(args => _hasControl = true, TimedActionCountType.SCALEDTIME, _invincibleDuration);
                RoutineBehaviour.Instance.StartNewTimedAction(args => _myMovementState = MovementState.WALKING, TimedActionCountType.SCALEDTIME, _invincibleDuration);
                RoutineBehaviour.Instance.StartNewTimedAction(args => _canDodge = true, TimedActionCountType.SCALEDTIME, _dodgeCooldown);
            }
        }
        if (_myMovementState == MovementState.DODGING) 
        {
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _dodgeAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

            Vector3 moveDirection = Quaternion.Euler(0.0f, _dodgeAngle, 0.0f) * Vector3.forward;

            _controller.Move(moveDirection.normalized * (_moveSpeed * 5.0f) * Time.deltaTime);
        }
    }

    private void Sprint() 
    {
        if (_mobilityInput)
        {
            if (_mobilityInputTime >= _mobilityThreshold)
            {
                _isSprinting = true;
                if(_myMovementState != MovementState.DODGING)
                    _myMovementState = MovementState.SPRINTING;
            }
        }
        else 
        {
            _isSprinting = false;
            if (_myMovementState != MovementState.DODGING)
                _myMovementState = MovementState.WALKING;
        }
    }
}
