using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RushComponent : PlayerComponent
{
    [Header("Components")]
    private RushSystem _rushSystem;

    [Header("Rush Settings")]
    public RushConfig rushConfig;

    public event Action<bool, RushConfig> OnRush;

    private void OnEnable()
    {
        _rushSystem = new(this);
        _rushSystem.OnRush += SendRushMessage;
        _rushSystem.OnTimerUpdate += _entity.UIManager.UpdateCooldownTimer;

        _entity.BashComponent.OnBeingBashed += _rushSystem.FinishRush;
        _entity.InputComponent.OnRushInput += _rushSystem.ExecuteRush;
    }

    private void OnDisable()
    {
        _rushSystem.OnRush -= SendRushMessage;
        _rushSystem.OnTimerUpdate -= _entity.UIManager.UpdateCooldownTimer;

        _entity.BashComponent.OnBeingBashed -= _rushSystem.FinishRush;
        _entity.InputComponent.OnRushInput -= _rushSystem.ExecuteRush;
    }

    private void Update()
    {
        if (!_updatable) return;
        _rushSystem.ProcessRush();
    }

    private void SendRushMessage(bool toRushMode, RushConfig config)
    {
        OnRush?.Invoke(toRushMode, config);
    }

    public override void OnEndGame(string winnerName)
    {
        base.OnEndGame(winnerName);
        _rushSystem.EndProcess();
    }

    public override void OnRestart()
    {
        base.OnRestart();
    }
}

[Serializable]
public class RushConfig
{
    public float speed;
    public float pathLength;
    public float cooldown;
}