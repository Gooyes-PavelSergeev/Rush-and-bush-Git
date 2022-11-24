using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class HitReciever : PlayerComponent, ITargetable, IHurtResponder
{
    [SerializeField] private bool _targetable = true;
    [SerializeField] private Transform _targetTransform;

    private List<Hurtbox> _hurtboxes = new ();

    public event Action<HitConfig> OnHitRecieve;

    bool ITargetable.Targetable { get { return _targetable; } }
    Transform ITargetable.TargetTransform { get { return _targetTransform; } }

    private void Start()
    {
        _hurtboxes = new List<Hurtbox>(GetComponentsInChildren<Hurtbox>());
        foreach (var hurtbox in _hurtboxes)
        {
            hurtbox.HurtResponder = this;
        }
    }

    bool IHurtResponder.CheckHit(HitData hitData)
    {
        return true;
    }

    void IHurtResponder.Response(HitData hitData)
    {

    }

    public void ResponseFromServer(float hitDuration, GameObject senderGO)
    {
        if (senderGO == null)
        {
            Debug.Log("No hitData");
            return;
        }
        HitConfig hitConfig = new HitConfig()
        {
            duration = hitDuration,
            owner = senderGO.GetComponent<PlayerEntity>()
        };

        OnHitRecieve?.Invoke(hitConfig);
    }
}
