using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    private CharacterController _controller; //The character controller that this object has
    [SerializeField] private Transform _cameraTransform; //The transform of the camera used to inherit its direction for movement
    [SerializeField] private float _moveSpeed; //How fast the player will move
    private float _turnSmoothTime = 0.1f; //Used to smooth the turn rate of the players visual
    private float _turnSmoothVelocity; //Used to smooth the turn rate of the players visual

    private void Awake()
    {
        //On initialize, set this scripts character controller to the controller that is on this object
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //Get the horizontal and vertical input, defaults to WASD and Arrow controlls
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        //Create a vector 3 in the direction the player is inputting to move
        Vector3 direction = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;

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

            //Move the character controller
            _controller.Move(moveDirection.normalized * _moveSpeed * Time.deltaTime);
        }
    }
}
