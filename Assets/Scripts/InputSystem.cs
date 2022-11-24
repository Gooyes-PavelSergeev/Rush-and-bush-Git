using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem
{
    [Header("Control Settings")]
    private readonly string _horizontal;
    private readonly string _vertical;
    private readonly int _rushActivate;
    private readonly int _rushDeactivate;

    private Vector2 _lastDirectionInput; // to make player move only if input has changed

    public event Action<Vector3> OnMoveInput;
    public event Action<bool> OnRushInput;

    public InputSystem (InputComponent inputComponent)
    {
        _horizontal = inputComponent.horizontal;
        _vertical = inputComponent.vertical;
        _rushActivate = inputComponent.rushMouseButton;
        _rushDeactivate = inputComponent.rushStopMouseButton;
    }

    public void ProcessInput()
    {
        CheckMoveInput();
        CheckRushInput();
    }

    private void CheckMoveInput()
    {
        float horizontal = Input.GetAxisRaw(_horizontal);
        float vertical = Input.GetAxisRaw(_vertical);
        Vector2 inputDirection = new Vector2(horizontal, vertical);
        if (_lastDirectionInput != inputDirection) OnMoveInput?.Invoke(new Vector3(horizontal, 0, vertical));
        _lastDirectionInput = inputDirection;
    }

    private void CheckRushInput()
    {
        if (Input.GetMouseButtonDown(_rushActivate)) OnRushInput?.Invoke(true);
        else if (Input.GetMouseButtonUp(_rushDeactivate)) OnRushInput?.Invoke(false);
    }
}
