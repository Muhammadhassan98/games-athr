using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI highScoreText;

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject gameOverPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (winPanel) winPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        UpdateScore(0);
        if (highScoreText) highScoreText.text = $"Best: {ScoreManager.Instance?.HighScore}";
    }

    public void UpdateScore(int score)
    {
        if (scoreText) scoreText.text = $"Score: {score}";
    }

    public void UpdateMoves(int moves)
    {
        if (movesText) movesText.text = $"Moves: {moves}";
    }

    public void UpdateTarget(int target)
    {
        if (targetText) targetText.text = $"Target: {target}";
    }

    public void ShowWin()
    {
        if (winPanel) winPanel.SetActive(true);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    public void OnRestartButton()
    {
        GameManager.Instance?.RestartGame();
    }
}
