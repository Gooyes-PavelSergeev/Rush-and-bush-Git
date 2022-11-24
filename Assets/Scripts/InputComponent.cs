using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputComponent : PlayerComponent
{
    [Header("Components")]
    private InputSystem _inputSystem;

    [Header("Input Settings")]
    public string vertical = "Vertical";
    public string horizontal = "Horizontal";
    [Range(0, 1)] public int rushMouseButton = 0;
    [Range(0, 1)] public int rushStopMouseButton = 1;

    public event Action<Vector3> OnMoveInput;
    public event Action<bool> OnRushInput;

    private void Update()
    {
        if (!_updatable) return;
        _inputSystem.ProcessInput();
    }

    private void OnEnable()
    {
        _inputSystem = new(this);

        _inputSystem.OnMoveInput += SendMoveCommand;
        _inputSystem.OnRushInput += SendRushCommand;
    }

    private void OnDisable()
    {
        _inputSystem.OnMoveInput -= SendMoveCommand;
        _inputSystem.OnRushInput -= SendRushCommand;
    }

    private void SendMoveCommand(Vector3 direction)
    {
        OnMoveInput?.Invoke(direction);
    }

    private void SendRushCommand(bool active)
    {
        OnRushInput?.Invoke(active);
    }

    public override void OnEndGame(string winnerName)
    {
        base.OnEndGame(winnerName);
    }

    public override void OnRestart()
    {
        base.OnRestart();
    }
}
