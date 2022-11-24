using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AnimationController : PlayerComponent
{
    [SerializeField] private Animator _animator;
    [SerializeField] private NetworkAnimator _networkAnimator;

    [SerializeField] private float _runDamp = 0.05f;

    private void OnEnable()
    {
        _entity.MovementComponent.OnSpeedChanged += ChangeSpeed;
        _entity.RushComponent.OnRush += SetRush;
        _entity.BashComponent.OnBeingBashed += SetBash;
    }

    private void OnDisable()
    {
        _entity.MovementComponent.OnSpeedChanged -= ChangeSpeed;
        _entity.BashComponent.OnBeingBashed -= SetBash;
        _entity.RushComponent.OnRush -= SetRush;
    }

    private void Update()
    {
        if (!_updatable) return;
        SetAnimParameterSmoothly();
    }

    private void ChangeSpeed(float speed)
    {
        float speedNormalized = speed / _entity.MovementComponent.movementSpeed;
        SetAnimParameterSmoothly("Speed", speedNormalized, _runDamp);
    }

    private bool _isBashed;
    private void SetRush(bool toRushMode, RushConfig config)
    {
        if (_isBashed) return;
        string triggerName = toRushMode ? "RushStart" : "RushEnd";
        _networkAnimator.SetTrigger(triggerName);
    }

    private void SetBash(bool toBashState)
    {
        _networkAnimator.ResetTrigger("RushEnd");
        _networkAnimator.ResetTrigger("RushStart");
        string triggerName = toBashState ? "BashStart" : "BashEnd";
        _isBashed = toBashState;
        _networkAnimator.SetTrigger(triggerName);
    }

    public override void OnEndGame(string winnerName)
    {
        _animationParameters.Clear();
        _networkAnimator.ResetTrigger("BashEnd");
        _networkAnimator.ResetTrigger("BashStart");
        _networkAnimator.ResetTrigger("RushEnd");
        _networkAnimator.ResetTrigger("RushStart");
    }

    public override void OnRestart()
    {
        _animator.SetFloat("Speed", 0);
        _animator.Play("MainMovement");
    }

    #region AnimationParameters
    private readonly List<AnimationParameterConfig> _animationParameters = new();
    private void SetAnimParameterSmoothly(string parameter, float targetValue, float damping)
    {
        if (parameter == null) return;
        AnimationParameterConfig config = new AnimationParameterConfig();
        config.parameterName = parameter;
        config.targetValue = targetValue;
        config.damping = damping;
        foreach (var animationParameter in _animationParameters)
            if (animationParameter.parameterName == parameter)
                if (animationParameter != null)
                {
                    _animationParameters.Remove(animationParameter);
                    SetAnimParameterSmoothly(parameter, targetValue, damping);
                    return;
                }
        _animationParameters.Add(config);
    }
    private void SetAnimParameterSmoothly()
    {
        if (_animationParameters.Count == 0) return;
        foreach (AnimationParameterConfig parameter in _animationParameters)
        {
            if (parameter.parameterName != null)
            {
                float currentValue = _animator.GetFloat(parameter.parameterName);
                _animator.SetFloat(parameter.parameterName, parameter.targetValue, parameter.damping, Time.deltaTime);
                bool isSet = Mathf.Abs(currentValue - parameter.targetValue) <= 0.01f;
                if (isSet)
                {
                    _animationParameters.Remove(parameter);
                    return;
                }
            }
        }
    }

    public class AnimationParameterConfig
    {
        public string parameterName;
        public float targetValue;
        public float damping;
    }
    #endregion
}
