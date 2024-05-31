using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject startUIScene;
    [SerializeField] private GameObject gameplayUIScene;
    [SerializeField] private GameObject gameOverUIScene;

    [SerializeField] private TextMeshProUGUI gameElapsedTimeText;

    public bool isInitialized { get; private set; } = false;
    public UIScene activeUIScene { get; private set; } = UIScene.None;

    private GameController gameController;

    private void Awake()
    {
        SetActiveUIScene(UIScene.None);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameController.Instance != null);
        gameController = GameController.Instance;
        gameController.OnGameStateChange += OnGameStateChange;

        isInitialized = true;
    }

    void OnGameStateChange(GameController.GameState state)
    {
        switch (state)
        {
            case GameController.GameState.Ready:
                SetActiveUIScene(UIScene.Start); break;
            case GameController.GameState.Started:
                SetActiveUIScene(UIScene.Gameplay); break;
            case GameController.GameState.Over:
                SetActiveUIScene(UIScene.GameOver); break;
            default: break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController == null) return;
        if (gameController.gameState == GameController.GameState.Started)
        {
            float elapsedTime = gameController.enemySpawner.elapsedTime;
            gameElapsedTimeText.text = TimeSpan.FromSeconds(elapsedTime).ToString(@"mm\:ss");
        }
    }

    void SetActiveUIScene(UIScene uiScene)
    {
        startUIScene.SetActive(false);
        gameplayUIScene.SetActive(false);
        gameOverUIScene.SetActive(false);

        switch (uiScene)
        {
            case UIScene.None:
                break;
            case UIScene.Start:
                startUIScene.SetActive(true);
                break;
            case UIScene.Gameplay:
                gameplayUIScene.SetActive(true);
                break;
            case UIScene.GameOver:
                gameOverUIScene.SetActive(true);
                break;
            default:
                break;
        }
    }


    public enum UIScene
    {
        None,
        Start,
        Gameplay,
        GameOver
    }
}
