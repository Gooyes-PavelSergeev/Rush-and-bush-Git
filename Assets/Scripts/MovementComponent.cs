using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : PlayerComponent
{
    [Header ("Components")]
    private MovementSystem _movementSystem;
    public Camera camera;

    [Header("Movement Settings")]
    public CharacterController characterController;
    public float movementSpeed = 4f;
    public float turnSmoothTime = 0.1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.125f;

    public event Action<float> OnSpeedChanged;

    private void Awake()
    {
        camera = Camera.FindObjectOfType<Camera>();
    }

    private void OnEnable()
    {
        _movementSystem = new(this);
        _movementSystem.OnSpeedChanged += SendSpeedChangeMessage;

        _entity.BashComponent.OnBeingBashed += InterruptByBash;
        _entity.RushComponent.OnRush += InterruptByRush;

        _entity.InputComponent.OnMoveInput += _movementSystem.GetMoveDirection;
    }

    private void OnDisable()
    {
        _entity.InputComponent.OnMoveInput -= _movementSystem.GetMoveDirection;

        _entity.BashComponent.OnBeingBashed -= InterruptByBash;
        _entity.RushComponent.OnRush -= InterruptByRush;

        _movementSystem.OnSpeedChanged -= SendSpeedChangeMessage;
    }

    private void Update()
    {
        if (!_updatable) return;
        _movementSystem.ProcessMovement();
    }

    private void SendSpeedChangeMessage(float speed)
    {
        OnSpeedChanged?.Invoke(speed);
    }

    private void InterruptByRush(bool toRushMode, RushConfig config)
    {
        if (toRushMode) MovementInterruptor.Interrupt(_movementSystem, MovementInterruptorType.Rush, config.pathLength / config.speed);
    }

    private void InterruptByBash(bool toBashState)
    {
        if (toBashState) MovementInterruptor.Interrupt(_movementSystem, MovementInterruptorType.Bash, 60f);
        else MovementInterruptor.Interrupt(_movementSystem, MovementInterruptorType.Bash, 0f);
    }

    public override void OnEndGame(string winnerName)
    {
        base.OnEndGame(winnerName);
        _movementSystem.EndProcess();
        StartCoroutine(TurnOffCCForRestartTime());
    }

    public override void OnRestart()
    {
        base.OnRestart();
    }

    private IEnumerator TurnOffCCForRestartTime()
    {
        characterController.enabled = false;
        yield return new WaitForSeconds(GameData.gameRestartTime + 0.25f);
        characterController.enabled = true;
    }
}
