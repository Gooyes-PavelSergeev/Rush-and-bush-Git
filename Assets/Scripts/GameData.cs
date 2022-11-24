using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameData : MonoBehaviour
{
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private NetworkCustomManager _netManager;

    public static int gameRestartTime = 5;

    public static int hitsNumberGoal = 3;

    private float _timeSinceEnd;
    private bool _isEnded;

    public List<Transform> SpawnPoints { get { return _spawnPoints; } }
    public static GameData Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) Destroy(Instance);
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void EndGame()
    {
        _isEnded = true;
        _timeSinceEnd = 0;
    }

    private void Update()
    {
        if (!_isEnded) return;

        if (_timeSinceEnd > gameRestartTime)
        {
            _isEnded = false;
            _netManager.Restart();
        }
        _netManager.UpdateRestartTimer(GameData.gameRestartTime - (int)_timeSinceEnd);
        _timeSinceEnd += Time.deltaTime;
    }
}
