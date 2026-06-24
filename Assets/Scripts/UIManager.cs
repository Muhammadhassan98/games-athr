using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject levelCompletePanel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        HideAllPanels();
    }

    private void Start()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged += UpdateScore;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += HandleGameOver;
            GameManager.Instance.OnLevelComplete += HandleLevelComplete;
            GameManager.Instance.OnMovesChanged += UpdateMoves;
        }
    }

    private void HandleGameOver()
    {
        int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0;
        ShowGameOver(finalScore);
    }

    private void HandleLevelComplete()
    {
        int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0;
        ShowLevelComplete(finalScore);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void UpdateMoves(int moves)
    {
        if (movesText != null)
            movesText.text = "Moves: " + moves;
    }

    public void ShowGameOver(int finalScore)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverScoreText != null)
            gameOverScoreText.text = "Score: " + finalScore;
    }

    public void ShowLevelComplete(int finalScore)
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Score: " + finalScore;
    }

    public void HideAllPanels()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
    }

    public void OnRestartButtonClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();

        HideAllPanels();
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= UpdateScore;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= HandleGameOver;
            GameManager.Instance.OnLevelComplete -= HandleLevelComplete;
            GameManager.Instance.OnMovesChanged -= UpdateMoves;
        }
    }
}
