using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip matchClip;
    [SerializeField] private AudioClip swapClip;
    [SerializeField] private AudioClip invalidSwapClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip levelCompleteClip;

    private AudioSource matchSource;
    private AudioSource swapSource;
    private AudioSource invalidSource;
    private AudioSource gameOverSource;
    private AudioSource levelCompleteSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        matchSource = gameObject.AddComponent<AudioSource>();
        swapSource = gameObject.AddComponent<AudioSource>();
        invalidSource = gameObject.AddComponent<AudioSource>();
        gameOverSource = gameObject.AddComponent<AudioSource>();
        levelCompleteSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        if (GemSwapper.Instance != null)
        {
            GemSwapper.Instance.OnSwapAttempted += HandleSwapAttempted;
            GemSwapper.Instance.OnInvalidSwap += PlayInvalidSwap;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += PlayGameOver;
            GameManager.Instance.OnLevelComplete += PlayLevelComplete;
        }
    }

    private void HandleSwapAttempted(bool success)
    {
        if (success)
            PlaySwap();
    }

    public void PlayMatch()
    {
        if (matchClip != null)
            matchSource.PlayOneShot(matchClip);
    }

    public void PlaySwap()
    {
        if (swapClip != null)
            swapSource.PlayOneShot(swapClip);
    }

    public void PlayInvalidSwap()
    {
        if (invalidSwapClip != null)
            invalidSource.PlayOneShot(invalidSwapClip);
    }

    public void PlayGameOver()
    {
        if (gameOverClip != null)
            gameOverSource.PlayOneShot(gameOverClip);
    }

    public void PlayLevelComplete()
    {
        if (levelCompleteClip != null)
            levelCompleteSource.PlayOneShot(levelCompleteClip);
    }

    private void OnDestroy()
    {
        if (GemSwapper.Instance != null)
        {
            GemSwapper.Instance.OnSwapAttempted -= HandleSwapAttempted;
            GemSwapper.Instance.OnInvalidSwap -= PlayInvalidSwap;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= PlayGameOver;
            GameManager.Instance.OnLevelComplete -= PlayLevelComplete;
        }
    }
}
