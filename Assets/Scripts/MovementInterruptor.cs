using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInterruptor
{
    public event Action<MovementInterruptorType, float> OnMovementInterruption;

    public static void Interrupt(MovementSystem controller, MovementInterruptorType type, float duration)
    {
        controller.Interruptor.OnMovementInterruption?.Invoke(type, duration);
    }
}

public enum MovementInterruptorType
{
    Rush,
    Bash
}
