using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, GameOver, LevelComplete }

    public GameState State { get; private set; }

    private int movesRemaining;
    private const int StartingMoves = 30;

    [SerializeField] private Board board;

    public event Action OnGameOver;
    public event Action OnLevelComplete;
    public event Action<int> OnMovesChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (board == null)
            board = FindObjectOfType<Board>();

        if (board != null)
            board.OnBoardStable += HandleBoardStable;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;

        StartGame();
    }

    private void HandleScoreChanged(int score)
    {
        CheckLevelComplete();
    }

    private void HandleBoardStable()
    {
        CheckGameOver();
    }

    public void StartGame()
    {
        State = GameState.Playing;
        movesRemaining = StartingMoves;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();

        if (board != null)
            board.Initialize();

        OnMovesChanged?.Invoke(movesRemaining);
    }

    public void OnMoveUsed()
    {
        if (State != GameState.Playing) return;

        movesRemaining--;
        OnMovesChanged?.Invoke(movesRemaining);

        CheckGameOver();
    }

    public void OnMatchFound(List<Gem> matches)
    {
        if (matches == null || matches.Count == 0) return;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(matches.Count, ScoreManager.Instance.ComboMultiplier);
    }

    public void CheckGameOver()
    {
        if (State != GameState.Playing) return;

        if (movesRemaining <= 0 && (ScoreManager.Instance == null || !ScoreManager.Instance.LevelGoalReached))
        {
            State = GameState.GameOver;
            OnGameOver?.Invoke();
        }
    }

    public void CheckLevelComplete()
    {
        if (State != GameState.Playing) return;

        if (ScoreManager.Instance != null && ScoreManager.Instance.LevelGoalReached)
        {
            State = GameState.LevelComplete;
            OnLevelComplete?.Invoke();
        }
    }

    public void RestartGame()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();

        if (board != null)
            board.Initialize();

        movesRemaining = StartingMoves;
        State = GameState.Playing;
        OnMovesChanged?.Invoke(movesRemaining);
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;

        if (board != null)
            board.OnBoardStable -= HandleBoardStable;
    }
}
