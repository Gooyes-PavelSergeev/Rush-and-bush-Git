using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSystem
{
    private readonly Transform _transform;
    private Vector3 _lastInputDirection;
    private Vector3 _velocity;
    private bool _ableToMove;
    private float _gravity;

    [Header("Movement Settings")]
    private readonly CharacterController _characterController;
    private readonly float _movementSpeed;
    private float _currentSpeed;
    private readonly float _turnSmoothTime;
    private float _turnSmoothVelocity;

    [Header("Ground Check")]
    private readonly Transform _groundCheck;
    private readonly LayerMask _groundMask;
    private readonly float _groundDistance;
    private bool _isGrounded;

    private readonly Camera _camera;
    private MovementInterruptor _interruptor;

    public MovementInterruptor Interruptor { get => _interruptor; }

    public event Action<float> OnSpeedChanged;

    #region Init
    public MovementSystem(MovementComponent movementComponent)
    {
        _transform = movementComponent.gameObject.transform.parent.transform;

        _characterController = movementComponent.characterController;
        _movementSpeed = movementComponent.movementSpeed;
        _turnSmoothTime = movementComponent.turnSmoothTime;

        _groundCheck = movementComponent.groundCheck;
        _groundMask = movementComponent.groundMask;
        _groundDistance = movementComponent.groundDistance;

        _camera = movementComponent.camera;

        Start();
    }
    private void Start()
    {
        _interruptor = new ();
        _interruptor.OnMovementInterruption += InterruptMovement;
        _ableToMove = true;
        _lastInputDirection = Vector3.zero;
        _gravity = -9.81f;
    }
    #endregion

    public void ProcessMovement()
    {
        MoveOnInput();
        UpdateSpeed();
        MoveDown();
        ProcessInterruption();
    }

    public void GetMoveDirection(Vector3 inputDirection)
    {
        Vector3 direction = inputDirection.normalized;
        _lastInputDirection = direction;
    }

    private void MoveOnInput()
    {
        if (!_ableToMove || _lastInputDirection == Vector3.zero) return;
        float targetAngle = Mathf.Atan2(_lastInputDirection.x, _lastInputDirection.z) * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;

        float rotationAngle = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
        _transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f);

        Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0f) * Vector3.forward;
        _characterController.Move(_currentSpeed * Time.deltaTime * moveDir.normalized);
    }

    private void UpdateSpeed()
    {
        float moveSpeed = _currentSpeed;

        if (_lastInputDirection == Vector3.zero) _currentSpeed = 0;
        else _currentSpeed = _movementSpeed;

        if (moveSpeed != _currentSpeed) OnSpeedChanged?.Invoke(_currentSpeed);
    }

    private void MoveDown()
    {
        _velocity.y = _isGrounded ? -4f : _velocity.y + _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
    }

    public void EndProcess()
    {
        _velocity = Vector3.zero;
        _lastInputDirection = Vector3.zero;
        _ableToMove = true;
        _currentSpeed = 0;
        _isGrounded = true;
        InterruptMovement(MovementInterruptorType.Rush, 0f);
    }

    #region Interruption
    private bool _isInterrupted;
    private float _timeSinceInterruption;
    private float _interruptionDuration;
    private MovementInterruptorType _interruptorType;

    public void InterruptMovement(MovementInterruptorType type, float duration)
    {
        _isInterrupted = true;
        _ableToMove = false;
        _timeSinceInterruption = 0;
        _interruptionDuration = duration;
        _interruptorType = type;
    }
    private void ProcessInterruption()
    {
        if (!_isInterrupted) return;
        if (_timeSinceInterruption >= _interruptionDuration)
        {
            _isInterrupted = false;
            _ableToMove = true;
        }
        _timeSinceInterruption += Time.deltaTime;
    }
    #endregion
}
