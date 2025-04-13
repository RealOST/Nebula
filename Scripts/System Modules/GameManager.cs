using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : PersistentSingleton<GameManager> {
    public static System.Action onGameOver;

    public static GameState GameState {
        get => Instance.gameState;
        set => Instance.gameState = value;
    }

    [SerializeField] private GameState gameState = GameState.Playing;
}

public enum GameState {
    Playing,
    Paused,
    GameOver,
    Scoring
}