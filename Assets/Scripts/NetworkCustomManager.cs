using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkCustomManager : NetworkManager
{
    private PlayerEntity _playerObj;

    public void SetPlayer(PlayerEntity player)
    {
        _playerObj = player;
    }

    public void SetPlayerName() // for canvas button
    {
        _playerObj.PlayerData.SetName();
    }

    public void Restart()
    {
        _playerObj.PlayerData.Restart();
    }

    public void UpdateRestartTimer(int time)
    {
        _playerObj.UIManager.UpdateTimer(time);
    }
}
