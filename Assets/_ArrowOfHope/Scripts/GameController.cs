using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private UIController _uiController;
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private PlayerController _playerController;

    public static GameController Instance;

    public UIController uiController => _uiController;
    public EnemySpawner enemySpawner => _enemySpawner;
    

    public GameState gameState { get; private set; } = GameState.Initializing;
    public Action<GameState> OnGameStateChange;


    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        _playerController.enabled = false;
        yield return new WaitUntil(() =>
        {
            return
                _uiController.isInitialized
                && _enemySpawner.isInitialized
            ;
        });
        ChangeGameState(GameState.Ready);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        if (gameState == GameState.Ready)
        {
            ChangeGameState(GameState.Started);
            _playerController.enabled = true;
        }
    }

    public void GameOver()
    {
        if (gameState == GameState.Started)
        {
            ChangeGameState(GameState.Over);
        }
    }

    private void ChangeGameState(GameState state)
    {
        gameState = state;
        OnGameStateChange?.Invoke(state);
    }

    public enum GameState
    {
        Initializing,
        Ready,
        Started,
        Over
    }
}
