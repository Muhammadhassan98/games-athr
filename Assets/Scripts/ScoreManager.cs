using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }
    public int ComboMultiplier { get; private set; } = 1;
    public const int LevelGoalScore = 500;
    public bool LevelGoalReached => Score >= LevelGoalScore;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnComboChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddScore(int gemCount, int comboLevel)
    {
        int points = gemCount * 30 * comboLevel;
        Score += points;
        OnScoreChanged?.Invoke(Score);
    }

    public void ResetCombo()
    {
        ComboMultiplier = 1;
        OnComboChanged?.Invoke(ComboMultiplier);
    }

    public void IncrementCombo()
    {
        ComboMultiplier++;
        OnComboChanged?.Invoke(ComboMultiplier);
    }

    public void ResetScore()
    {
        Score = 0;
        ComboMultiplier = 1;
        OnScoreChanged?.Invoke(0);
        OnComboChanged?.Invoke(1);
    }
}
