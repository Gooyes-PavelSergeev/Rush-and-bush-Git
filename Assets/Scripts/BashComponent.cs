using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BashComponent : PlayerComponent
{
    [Header("Bash Settings")]
    public HitConfig bashConfig;
    public Hitbox hitbox;

    private BashSystem _bashSystem;

    public event Action<bool> OnBeingBashed;

    private void OnEnable()
    {
        _bashSystem = new(this);

        _bashSystem.OnBeingBashed += SetBash;
        _entity.HitReciever.OnHitRecieve += _bashSystem.SetBashTrigger;
        _entity.RushComponent.OnRush += _bashSystem.SetRushTrigger;
    }

    private void OnDisable()
    {
        _bashSystem.OnBeingBashed -= SetBash;
        _entity.HitReciever.OnHitRecieve -= _bashSystem.SetBashTrigger;
        _entity.RushComponent.OnRush -= _bashSystem.SetRushTrigger;
    }

    private void Update()
    {
        if (!_updatable) return;
        _bashSystem.ProcessBash();
    }

    private void SetBash(bool toBashState)
    {
        _entity.Hurtbox.Active = !toBashState;
        OnBeingBashed?.Invoke(toBashState);
    }

    public override void OnEndGame(string winnerName)
    {
        base.OnEndGame(winnerName);
        _bashSystem.EndProcess();
    }

    public override void OnRestart()
    {
        base.OnRestart();
    }
}

[Serializable]
public class HitConfig
{
    public float duration; // pls do not set it to 60 or higher
    public PlayerEntity owner;
}
