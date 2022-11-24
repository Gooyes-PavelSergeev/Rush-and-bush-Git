using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerData : NetworkBehaviour
{
    #region Entity
    private PlayerEntity _entity;
    public PlayerEntity Entity { get { return _entity; } set { _entity = value; } }
    #endregion

    [SyncVar] public string playerName;
    public string playerDefaultName = "New Player";

    public int numOfHits;

 
    private void Start()
    {
        if (!_entity.isOwned) return;
        playerName = playerDefaultName;
        SetName(playerDefaultName, true);
    }

    #region NameNetworkSetter

    public void SetName()
    {
        string playerName = _entity.UIManager.PlayerInputName;
        if (string.IsNullOrEmpty(playerName) || playerName == playerDefaultName) return;
        SetName(_entity.UIManager.PlayerInputName, false);
    }

    [Client]
    private void SetName(string value, bool startSetter)
    {
        CmdChangeName(NetworkClient.localPlayer.netId, value, startSetter);
    }

    [Command]
    private void CmdChangeName(uint targetId, string value, bool startSetter)
    {
        NetworkIdentity targetIdentity = NetworkServer.spawned[targetId];
        if (targetIdentity == null) return;
        RpcChangeName(targetIdentity, value, startSetter);
    }

    [ClientRpc]
    private void RpcChangeName(NetworkIdentity identity, string value, bool startSetter)
    {
        playerName = value;
        PlayerUIManager uIManager = identity.gameObject.GetComponent<PlayerEntity>().UIManager;
        uIManager.SetName(value, startSetter);
    }
    #endregion


    #region HitNetworkDetection
    [Client]
    public void RegisterHit(HitData hitData)
    {
        uint recieverId = hitData.hurtbox.Owner.GetComponent<PlayerEntity>().netId;
        CmdRegisterHit(hitData.hitConfig.duration, _entity.gameObject, recieverId, _entity.netId);
    }

    [Command]
    private void CmdRegisterHit(float hitDuration, GameObject senderGO, uint recieverId, uint senderId)
    {
        PlayerData senderData = NetworkServer.spawned[senderId].gameObject.GetComponent<PlayerEntity>().PlayerData;

        senderData.numOfHits++;
        RpcSenderRegisterHit(NetworkServer.spawned[senderId].connectionToClient, senderData.numOfHits);

        RpcTargetRegisterHit(NetworkServer.spawned[recieverId].connectionToClient, hitDuration, senderGO);

        if (senderData.numOfHits >= GameData.hitsNumberGoal)
            RpcEndGame(senderData.playerName);
    }

    [TargetRpc]
    private void RpcSenderRegisterHit(NetworkConnection senderConnection, int hitNum)
    {
        senderConnection.identity.gameObject.GetComponent<PlayerEntity>().UIManager.UpdateHitScore(hitNum);
    }

    [TargetRpc]
    private void RpcTargetRegisterHit(NetworkConnection targetConnection, float hitDuration, GameObject senderGO)
    {
        targetConnection.identity.gameObject.GetComponent<PlayerEntity>().HitReciever.ResponseFromServer(hitDuration,senderGO);
    }
    #endregion


    #region HurtboxDisable
    [Command]
    public void CmdHandleHurtbox(bool disable, uint targetId)
    {
        NetworkIdentity targetIdentity = NetworkServer.spawned[targetId];
        if (targetIdentity == null) return;
        RpcHandleHurtbox(targetIdentity, disable);
    }

    [ClientRpc]
    private void RpcHandleHurtbox(NetworkIdentity identity, bool disable)
    {
        Hurtbox hurtbox = identity.gameObject.GetComponent<PlayerEntity>().Hurtbox;
        hurtbox.Active = !disable;
    }
    #endregion


    #region ColorRegister
    [Client]
    public void RegisterMaterial(int colorConfigId)
    {
        CmdRegisterMaterial(NetworkClient.localPlayer.netId, colorConfigId);
    }

    [Command]
    private void CmdRegisterMaterial(uint targetId, int configId)
    {
        NetworkIdentity targetIdentity = NetworkServer.spawned[targetId];
        if (targetIdentity == null) return;
        RpcRegisterMaterial(targetIdentity, configId);
    }

    [ClientRpc]
    private void RpcRegisterMaterial(NetworkIdentity identity, int configId)
    {
        ColorComponent colorComponent = identity.gameObject.GetComponent<PlayerEntity>().ColorComponent;
        colorComponent.SetMaterial(configId);
    }
    #endregion


    #region Restart

    [ClientRpc]
    private void RpcEndGame(string winnerName)
    {
        NetworkClient.localPlayer.gameObject.GetComponent<PlayerEntity>().EndGame(winnerName);
        GameData.Instance.EndGame();
    }

    public void Restart()
    {
        ClientRestart();
    }

    [Client]
    private void ClientRestart()
    {
        CmdRestart();
    }

    [Command]
    private void CmdRestart()
    {
        foreach (NetworkIdentity identity in NetworkServer.spawned.Values)
        {
            PlayerData playerData = identity.gameObject.GetComponent<PlayerEntity>().PlayerData;
            playerData.numOfHits = 0;
        }
        RpcRestart();
    }

    [ClientRpc]
    private void RpcRestart()
    {
        if (!isOwned) return;
        PlayerEntity player = NetworkClient.localPlayer.gameObject.GetComponent<PlayerEntity>();
        player.Restart();
    }

    public void Respawn()
    {
        _entity.gameObject.transform.position = GameData.Instance.SpawnPoints
            [UnityEngine.Random.Range(0, GameData.Instance.SpawnPoints.Count - 1)].
            position;
    }
    #endregion
}
