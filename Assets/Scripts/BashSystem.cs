using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BashSystem : IHitResponder
{
    private readonly Hitbox _hitbox;
    private readonly HitReciever _hitReciever;
    private readonly HitConfig _config;
    private List<GameObject> _objectHits;

    private bool _isBashed;
    private bool _isRushing;
    private float _timeSinceBash;
    private float _bashDuration;

    public event Action<bool> OnBeingBashed;

    HitReciever IHitResponder.HitReciever { get { return _hitReciever; } }
    HitConfig IHitResponder.HitConfig { get { return _config; } }

    #region Init
    public BashSystem (BashComponent bashComponent)
    {
        _config = bashComponent.bashConfig;
        _hitbox = bashComponent.hitbox;

        Start();
    }
    private void Start()
    {
        _hitbox.HitResponder = this;
        _isRushing = false;
        _objectHits = new List<GameObject>();
    }
    #endregion

    public void ProcessBash()
    {
        if (_isRushing)
        {
            _hitbox.CheckHit();
        }

        if (_isBashed)
        {
            if (_timeSinceBash >= _bashDuration)
            {
                _timeSinceBash = 0;
                _bashDuration = 0;
                _isBashed = false;
                OnBeingBashed?.Invoke(false);
            }
            _timeSinceBash += Time.deltaTime;
        }
    }

    public void SetRushTrigger(bool toRushMode, RushConfig config)
    {
        if (_isBashed) return;
        _isRushing = toRushMode;
        _objectHits.Clear();
    }

    public void SetBashTrigger(HitConfig hitConfig)
    {
        _timeSinceBash = 0;
        _bashDuration = hitConfig.duration;
        _isBashed = true;
        OnBeingBashed?.Invoke(true);
    }

    public void EndProcess()
    {
        _isRushing = false;
        _isBashed = false;
        _timeSinceBash = 0;
        _bashDuration = 0;
        _objectHits.Clear();
    }

    bool IHitResponder.CheckHit(HitData hitData)
    {
        if (_objectHits.Contains(hitData.hurtbox.Owner)) return false;
        return true;
    }

    void IHitResponder.Response(HitData hitData)
    {
        _objectHits.Add(hitData.hurtbox.Owner);
    }
}
