using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class PlayerUIManager : PlayerComponent
{
    [Header("Cooldown Colors")]
    [SerializeField] private Color _nonReadyColor;
    [SerializeField] private Color _readyColor;

    [Header("Player Name Setter UI")]
    [SerializeField] private TextMeshPro _playerNameTextObject;
    private TMP_InputField _playerNameInputField;

    [Header("InGameUI")]
    private TextMeshProUGUI _hitScore;
    private TextMeshProUGUI _cooldownTimer;

    [Header("Game End UI")]
    private GameObject _announcement;
    private TextMeshProUGUI _winnerName;
    private TextMeshProUGUI _timer;

    public string PlayerInputName { get { return _playerNameInputField.text; } }

    private void Start()
    {
        if (!_entity.isLocalPlayer || !_entity.isOwned) return;

        var goList = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in goList)
            if (go.CompareTag("Announcement"))
            {
                _announcement = go;
                break;
            }
        var tmpUIList = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (var tmpUI in tmpUIList)
        {
            if (tmpUI.CompareTag("Winner Name")) 
                _winnerName = tmpUI;
            if (tmpUI.CompareTag("Game End Timer"))
                _timer = tmpUI;
            if (tmpUI.CompareTag("Hit Score"))
                _hitScore = tmpUI;
            if (tmpUI.CompareTag("Cooldown Timer"))
                _cooldownTimer = tmpUI;
        }
        _playerNameInputField = GameObject.FindObjectOfType<TMP_InputField>(true);
        if (_announcement != null) _announcement.SetActive(false);
        if (_playerNameInputField != null) _playerNameInputField.gameObject.transform.parent.gameObject.SetActive(true);

        UpdateCooldownTimer(0);
        UpdateNames();
    }

    public void SetName(string name, bool startSetter)
    {
        _playerNameTextObject.text = name;
        if (startSetter) return;

        if (_entity.isOwned
            &&
            !string.IsNullOrEmpty(name)
            &&
            name != _entity.PlayerData.playerDefaultName)

            _playerNameInputField.gameObject.transform.parent.gameObject.SetActive(false);
    }

    private void UpdateNames()
    {
        foreach (NetworkIdentity identity in NetworkClient.spawned.Values)
        {
            PlayerEntity player = identity.gameObject.GetComponent<PlayerEntity>();
            player.UIManager.SetName(player.PlayerData.playerName, true);
        }
    }

    public override void OnEndGame(string winnerName)
    {
        if(_announcement != null) _announcement.SetActive(true);
        if(_winnerName != null) _winnerName.text = winnerName;
    }

    public void UpdateTimer(int time)
    {
        if(_timer != null) _timer.text = time.ToString() + "...";
    }

    public void UpdateCooldownTimer(float time)
    {
        if (_cooldownTimer == null) return;

        if (time <= 0.09f)
        {
            _cooldownTimer.color = _readyColor;
            time = 0;
        }
        else _cooldownTimer.color = _nonReadyColor;

        string timeString = time.ToString("0.00");
        _cooldownTimer.text = timeString;
    }

    public void UpdateHitScore(int hitNum)
    {
        if (_hitScore != null) _hitScore.text = hitNum.ToString() + '/' + GameData.hitsNumberGoal;
    }

    public override void OnRestart()
    {
        if (_announcement != null) _announcement.SetActive(false);
        if (_winnerName != null) _winnerName.text = " ";
        if (_timer != null) _timer.text = " ";
        if (_hitScore != null) _hitScore.text = "0/" + GameData.hitsNumberGoal;
    }

    public override void OnStopLocalPlayer()
    {
        if (_announcement != null) _announcement.SetActive(true);
    }
}
