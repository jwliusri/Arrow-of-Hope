using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private UIController _uiController;
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject[] activateOnStart;

    public static GameController Instance;
    private static bool startImmediately = false;

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
        //DontDestroyOnLoad(gameObject); // restart by reload scene
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        if (activateOnStart != null)
        {
            foreach (var go in activateOnStart)
            {
                go.SetActive(true);
            }
        }
        _playerController.enabled = false;
        yield return new WaitUntil(() =>
        {
            return
                _uiController.isInitialized
                && _enemySpawner.isInitialized
            ;
        });
        ChangeGameState(GameState.Ready);
        if (startImmediately) StartGame();
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
            _playerController.enabled = false;
        }
    }

    public void ReloadScene()
    {
        Instance = null;
        startImmediately = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
