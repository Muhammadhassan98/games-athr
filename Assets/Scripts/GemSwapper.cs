using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSwapper : MonoBehaviour
{
    public static GemSwapper Instance { get; private set; }

    private Gem firstSelected;
    private Board board;
    private MatchFinder matchFinder;
    private bool isSwapping;

    public event Action<bool> OnSwapAttempted;
    public event Action OnInvalidSwap;

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
        board = FindObjectOfType<Board>();
        matchFinder = FindObjectOfType<MatchFinder>();
        if (matchFinder == null)
            matchFinder = gameObject.AddComponent<MatchFinder>();
    }

    public void OnGemClicked(Gem gem)
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        if (isSwapping) return;

        if (firstSelected == null)
        {
            firstSelected = gem;
            gem.SetSelected(true);
        }
        else if (firstSelected == gem)
        {
            gem.SetSelected(false);
            firstSelected = null;
        }
        else if (IsAdjacent(firstSelected, gem))
        {
            StartCoroutine(AttemptSwap(firstSelected, gem));
        }
        else
        {
            firstSelected.SetSelected(false);
            firstSelected = gem;
            gem.SetSelected(true);
        }
    }

    private bool IsAdjacent(Gem a, Gem b)
    {
        return Mathf.Abs(a.Row - b.Row) + Mathf.Abs(a.Col - b.Col) == 1;
    }

    private IEnumerator AttemptSwap(Gem a, Gem b)
    {
        isSwapping = true;

        a.SetSelected(false);
        b.SetSelected(false);
        firstSelected = null;

        int aRow = a.Row, aCol = a.Col;
        int bRow = b.Row, bCol = b.Col;

        // Swap in board array
        board.SetGem(aRow, aCol, b);
        board.SetGem(bRow, bCol, a);

        // Animate both gems simultaneously
        Vector3 aTarget = board.GridToWorld(bRow, bCol);
        Vector3 bTarget = board.GridToWorld(aRow, aCol);

        Coroutine animA = StartCoroutine(a.AnimateToPosition(aTarget));
        Coroutine animB = StartCoroutine(b.AnimateToPosition(bTarget));

        yield return animA;
        yield return animB;

        // Check for matches
        Gem[,] grid = board.GetGrid();
        List<Gem> matches = matchFinder.FindMatches(grid);

        if (matches == null || matches.Count == 0)
        {
            // Swap back
            board.SetGem(aRow, aCol, a);
            board.SetGem(bRow, bCol, b);

            Coroutine revertA = StartCoroutine(a.AnimateToPosition(board.GridToWorld(aRow, aCol)));
            Coroutine revertB = StartCoroutine(b.AnimateToPosition(board.GridToWorld(bRow, bCol)));

            yield return revertA;
            yield return revertB;

            OnInvalidSwap?.Invoke();
        }
        else
        {
            OnSwapAttempted?.Invoke(true);

            if (GameManager.Instance != null)
                GameManager.Instance.OnMoveUsed();

            board.TriggerProcessBoard(matches);
        }

        isSwapping = false;
    }
}
