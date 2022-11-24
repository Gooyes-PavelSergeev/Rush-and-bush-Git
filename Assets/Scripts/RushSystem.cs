using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushSystem
{
    private readonly RushConfig _config;
    private readonly CharacterController _characterController;
    private readonly Transform _transform;

    private Vector3 _moveDir;
    private bool _isRushing;
    private bool _ableToRush;
    private float _lengthSinceRush;
    private float _cooldownTimer;

    public event Action<bool, RushConfig> OnRush;
    public event Action<float> OnTimerUpdate;

    #region Init
    public RushSystem(RushComponent rushComponent)
    {
        _config = rushComponent.rushConfig;
        _characterController = rushComponent.Entity.MovementComponent.characterController;
        _transform = rushComponent.gameObject.transform;

        Start();
    }
    private void Start()
    {
        _cooldownTimer = _config.cooldown;
        _lengthSinceRush = 0;
        _isRushing = false;
        _ableToRush = true;
    }
    #endregion

    public void ProcessRush()
    {
        if (!_isRushing)
        {
            if (_cooldownTimer < _config.cooldown)
            {
                float timeLeft = _config.cooldown - _cooldownTimer;
                if (timeLeft < 0) timeLeft = 0;
                OnTimerUpdate.Invoke(timeLeft);
                _cooldownTimer += Time.deltaTime;
            }
            return;
        }
        float length = _config.speed * Time.deltaTime;
        _characterController.Move(_moveDir.normalized * length);
        if (_lengthSinceRush >= _config.pathLength) FinishRush();
        _lengthSinceRush += length;
    }

    public void ExecuteRush(bool active)
    {
        if (!active)
        {
            if (_isRushing) FinishRush();
            return;
        }
        if (_cooldownTimer < _config.cooldown || !_ableToRush) return;
        _lengthSinceRush = 0;
        _cooldownTimer = 0;
        _isRushing = true;
        _moveDir = Quaternion.Euler(0, _transform.eulerAngles.y, 0f) * Vector3.forward;
        OnTimerUpdate?.Invoke(_config.cooldown);
        OnRush?.Invoke(true, _config);
    }

    private void FinishRush() //Finish by time
    {
        _isRushing = false;
        _lengthSinceRush = 0;
        _cooldownTimer = 0;
        OnTimerUpdate?.Invoke(_config.cooldown);
        OnRush?.Invoke(false, _config);
    }

    public void FinishRush(bool toBashState) //Finish from bash recieve
    {
        _ableToRush = !toBashState;
        if (toBashState && _isRushing) FinishRush();
    }

    public void EndProcess()
    {
        _isRushing = false;
        _ableToRush = true;
        _lengthSinceRush = 0;
        _cooldownTimer = _config.cooldown;
        _moveDir = Vector3.zero;
    }
}
