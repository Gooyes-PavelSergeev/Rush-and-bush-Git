using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Hurtbox : PlayerComponent, IHurtbox
{
    [SerializeField] private bool _active = true;
    [SerializeField] private GameObject _owner = null;
    [SerializeField] private HurtboxType _hurtboxType = HurtboxType.Enemy;
    private IHurtResponder _hurtResponder;


    public bool Active { get { return _active; } set { _active = value; } }

    public GameObject Owner { get => _owner; }

    public Transform Transform { get => transform; }

    public IHurtResponder HurtResponder { get => _hurtResponder; set => _hurtResponder = value; }

    public HurtboxType Type { get => _hurtboxType; }

    public bool CheckHit(HitData hitData)
    {
        if (_hurtResponder == null)
        {
            Debug.Log("No responder");
        }
        return true;
    }

    private void OnEnable()
    {
        _entity.BashComponent.OnBeingBashed += HandleHurtbox;
    }

    private void OnDisable()
    {
        _entity.BashComponent.OnBeingBashed -= HandleHurtbox;
    }

    [Client]
    private void HandleHurtbox(bool disable)
    {
        _entity.PlayerData.CmdHandleHurtbox(disable, _entity.netId);
    }

    public override void OnEndGame(string winnerName)
    {
        base.OnEndGame(winnerName);
    }

    public override void OnRestart()
    {
        base.OnRestart();
        HandleHurtbox(false);
    }
}
