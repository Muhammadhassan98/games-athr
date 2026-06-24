using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public const int Rows = 8;
    public const int Cols = 8;
    private const float CellSize = 1.0f;

    [SerializeField] private GameObject gemPrefab;

    private Gem[,] gems = new Gem[Rows, Cols];
    private Dictionary<GemType, Queue<Gem>> gemPool = new Dictionary<GemType, Queue<Gem>>();

    private MatchFinder matchFinder;

    public event Action OnBoardStable;

    private void Awake()
    {
        matchFinder = GetComponent<MatchFinder>();
        if (matchFinder == null)
            matchFinder = gameObject.AddComponent<MatchFinder>();

        foreach (GemType type in Enum.GetValues(typeof(GemType)))
            gemPool[type] = new Queue<Gem>();
    }

    public void Initialize()
    {
        // Clear existing gems back to pool
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (gems[r, c] != null)
                {
                    ReturnToPool(gems[r, c]);
                    gems[r, c] = null;
                }
            }
        }

        // Spawn all gems ensuring no initial 3-in-a-row matches
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                GemType type = GetRandomTypeNoMatch(r, c);
                gems[r, c] = SpawnGem(type, r, c);
            }
        }
    }

    private GemType GetRandomTypeNoMatch(int row, int col)
    {
        GemType[] allTypes = (GemType[])Enum.GetValues(typeof(GemType));
        List<GemType> candidates = new List<GemType>(allTypes);

        // Shuffle
        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            GemType tmp = candidates[i];
            candidates[i] = candidates[j];
            candidates[j] = tmp;
        }

        foreach (GemType type in candidates)
        {
            if (!WouldCauseMatch(row, col, type))
                return type;
        }

        return candidates[0];
    }

    private bool WouldCauseMatch(int row, int col, GemType type)
    {
        // Check horizontal match (two to the left)
        if (col >= 2 &&
            gems[row, col - 1] != null && gems[row, col - 1].Type == type &&
            gems[row, col - 2] != null && gems[row, col - 2].Type == type)
            return true;

        // Check vertical match (two below, since we fill bottom-up by row)
        if (row >= 2 &&
            gems[row - 1, col] != null && gems[row - 1, col].Type == type &&
            gems[row - 2, col] != null && gems[row - 2, col].Type == type)
            return true;

        return false;
    }

    public Gem SpawnGem(GemType type, int row, int col)
    {
        Gem gem = null;

        if (gemPool.ContainsKey(type) && gemPool[type].Count > 0)
        {
            gem = gemPool[type].Dequeue();
            gem.gameObject.SetActive(true);
        }
        else
        {
            GameObject go;
            if (gemPrefab != null)
            {
                go = Instantiate(gemPrefab, transform);
            }
            else
            {
                go = new GameObject("Gem_" + type);
                go.transform.SetParent(transform);
                go.AddComponent<SpriteRenderer>();
                go.AddComponent<CircleCollider2D>();
            }

            gem = go.GetComponent<Gem>();
            if (gem == null)
                gem = go.AddComponent<Gem>();
        }

        gem.transform.position = GridToWorld(row, col);
        gem.Initialize(type, row, col);
        gems[row, col] = gem;
        return gem;
    }

    public Gem GetGem(int row, int col)
    {
        if (!IsValidPosition(row, col)) return null;
        return gems[row, col];
    }

    public void SetGem(int row, int col, Gem gem)
    {
        if (!IsValidPosition(row, col)) return;
        gems[row, col] = gem;
        if (gem != null)
        {
            gem.Row = row;
            gem.Col = col;
        }
    }

    public bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < Rows && col >= 0 && col < Cols;
    }

    public Vector3 GridToWorld(int row, int col)
    {
        float startX = -(Cols * CellSize) / 2f + CellSize / 2f;
        float startY = -(Rows * CellSize) / 2f + CellSize / 2f;
        return new Vector3(startX + col * CellSize, startY + row * CellSize, 0f);
    }

    public Vector2Int WorldToGrid(Vector3 world)
    {
        float startX = -(Cols * CellSize) / 2f + CellSize / 2f;
        float startY = -(Rows * CellSize) / 2f + CellSize / 2f;
        int col = Mathf.RoundToInt((world.x - startX) / CellSize);
        int row = Mathf.RoundToInt((world.y - startY) / CellSize);
        return new Vector2Int(row, col);
    }

    public void ClearGems(List<Gem> toRemove)
    {
        foreach (Gem gem in toRemove)
        {
            if (gem == null) continue;
            int r = gem.Row;
            int c = gem.Col;
            if (IsValidPosition(r, c) && gems[r, c] == gem)
                gems[r, c] = null;
            ReturnToPool(gem);
        }
    }

    private void ReturnToPool(Gem gem)
    {
        if (gem == null) return;
        gem.gameObject.SetActive(false);
        if (!gemPool.ContainsKey(gem.Type))
            gemPool[gem.Type] = new Queue<Gem>();
        gemPool[gem.Type].Enqueue(gem);
    }

    private IEnumerator ApplyGravity()
    {
        bool anyMoved = false;

        for (int c = 0; c < Cols; c++)
        {
            int writeRow = 0;
            for (int r = 0; r < Rows; r++)
            {
                if (gems[r, c] != null)
                {
                    if (r != writeRow)
                    {
                        gems[writeRow, c] = gems[r, c];
                        gems[r, c] = null;
                        gems[writeRow, c].Row = writeRow;
                        gems[writeRow, c].Col = c;
                        Vector3 target = GridToWorld(writeRow, c);
                        StartCoroutine(gems[writeRow, c].AnimateToPosition(target));
                        anyMoved = true;
                    }
                    writeRow++;
                }
            }
        }

        if (anyMoved)
            yield return new WaitForSeconds(0.25f);
    }

    private IEnumerator RefillBoard()
    {
        bool anySpawned = false;

        for (int c = 0; c < Cols; c++)
        {
            int emptyCount = 0;
            for (int r = 0; r < Rows; r++)
            {
                if (gems[r, c] == null)
                {
                    emptyCount++;
                    GemType type = (GemType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(GemType)).Length);
                    Gem gem = SpawnGem(type, r, c);
                    // Spawn above board then animate down
                    gem.transform.position = GridToWorld(Rows + emptyCount, c);
                    StartCoroutine(gem.AnimateToPosition(GridToWorld(r, c)));
                    anySpawned = true;
                }
            }
        }

        if (anySpawned)
            yield return new WaitForSeconds(0.3f);
    }

    public void TriggerProcessBoard(List<Gem> matchedGems)
    {
        StartCoroutine(ProcessBoard(matchedGems));
    }

    private IEnumerator ProcessBoard(List<Gem> matchedGems)
    {
        // Handle the first set of matches passed in
        if (matchedGems != null && matchedGems.Count > 0)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnMatchFound(matchedGems);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMatch();

            ClearGems(matchedGems);
            yield return new WaitForSeconds(0.1f);
        }

        yield return StartCoroutine(ApplyGravity());
        yield return StartCoroutine(RefillBoard());
        yield return new WaitForSeconds(0.1f);

        // Check for chain reactions
        List<Gem> newMatches = matchFinder.FindMatches(gems);
        while (newMatches != null && newMatches.Count > 0)
        {
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.IncrementCombo();

            if (GameManager.Instance != null)
                GameManager.Instance.OnMatchFound(newMatches);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMatch();

            ClearGems(newMatches);
            yield return new WaitForSeconds(0.1f);

            yield return StartCoroutine(ApplyGravity());
            yield return StartCoroutine(RefillBoard());
            yield return new WaitForSeconds(0.1f);

            newMatches = matchFinder.FindMatches(gems);
        }

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetCombo();

        OnBoardStable?.Invoke();
    }

    public Gem[,] GetGrid()
    {
        return gems;
    }
}
