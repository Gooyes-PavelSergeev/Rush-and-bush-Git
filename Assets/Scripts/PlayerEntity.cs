using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public sealed class PlayerEntity : NetworkBehaviour
{
    #region PlayerComponents
    [Header("Player Components")]
    [SerializeField] private MovementComponent _movementComponent;
    [SerializeField] private InputComponent _inputComponent;
    [SerializeField] private RushComponent _rushComponent;
    [SerializeField] private BashComponent _bashComponent;
    [SerializeField] private HitReciever _hitReciever;
    [SerializeField] private Hurtbox _hurtbox;
    [SerializeField] private Hitbox _hitbox;
    [SerializeField] private AnimationController _animationController;
    [SerializeField] private ColorComponent _colorComponent;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private PlayerUIManager _UIManager;

    public MovementComponent MovementComponent { get { return _movementComponent; } }
    public InputComponent InputComponent { get { return _inputComponent; } }
    public RushComponent RushComponent { get { return _rushComponent; } }
    public BashComponent BashComponent { get { return _bashComponent; } }
    public HitReciever HitReciever { get { return _hitReciever; } }
    public Hurtbox Hurtbox { get { return _hurtbox; } }
    public Hitbox Hitbox { get { return _hitbox; } }
    public ColorComponent ColorComponent { get { return _colorComponent; } }
    public PlayerUIManager UIManager { get { return _UIManager; } }

    private PlayerComponent[] _playerComponents;
    #endregion

    [Header("Network Settings")]
    private NetworkCustomManager _netManager;
    [SerializeField] private PlayerData _playerData;
    public NetworkCustomManager NetManager { get { return _netManager; } }
    public PlayerData PlayerData { get { return _playerData; } }

    public bool IsOwned { get => isOwned; }

    private void Awake()
    {
        _playerComponents = new PlayerComponent[] {_movementComponent, _inputComponent,
                                                   _rushComponent, _bashComponent,
                                                   _hitReciever, _hurtbox, _UIManager,
                                                   _hitbox, _animationController,
                                                   _colorComponent, _cameraController};
        foreach (var component in _playerComponents)
            component.Entity = this;
        _playerData.Entity = this;
    }

    private void Start()
    {
        if (!isOwned || !isLocalPlayer) return;
        _netManager = GameObject.FindObjectOfType<NetworkCustomManager>();
        _netManager.SetPlayer(this);

        foreach (var component in _playerComponents)
            component.Updatable = true;
    }

    public void EndGame(string winnerName)
    {
        foreach (var component in _playerComponents)
            component.OnEndGame(winnerName);
    }

    public void Restart()
    {
        foreach (var component in _playerComponents)
            component.OnRestart();
        _playerData.Respawn();
    }
}
