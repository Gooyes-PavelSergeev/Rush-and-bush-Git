using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerComponent : NetworkBehaviour
{
    protected PlayerEntity _entity;
    protected bool _updatable;

    public PlayerEntity Entity { get { return _entity; } set { _entity = value; } }
    public bool Updatable { get { return _updatable; } set { _updatable = value; } }

    public virtual void OnEndGame(string winnerName)
    {
        _updatable = false;
    }

    public virtual void OnRestart()
    {
        if (!_entity.isOwned || !_entity.isLocalPlayer) return;
        _updatable = true;
    }
}
