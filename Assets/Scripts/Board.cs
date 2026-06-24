using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    [Header("Board Settings")]
    public int width = 8;
    public int height = 8;
    public float gemSize = 1f;
    public float gemSpacing = 0.1f;

    public Gem[,] Gems { get; private set; }
    private bool isProcessing;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Gems = new Gem[width, height];
        FillBoard();
        CenterCamera();
    }

    void CenterCamera()
    {
        float totalWidth = (width - 1) * (gemSize + gemSpacing);
        float totalHeight = (height - 1) * (gemSize + gemSpacing);
        Camera.main.transform.position = new Vector3(totalWidth / 2f, totalHeight / 2f, -10f);
        Camera.main.orthographicSize = Mathf.Max(totalWidth, totalHeight) / 2f + 1f;
    }

    void FillBoard()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                SpawnGem(x, y, GetRandomTypeWithoutMatch(x, y));
    }

    Gem.GemType GetRandomTypeWithoutMatch(int x, int y)
    {
        List<Gem.GemType> excluded = new List<Gem.GemType>();

        if (x >= 2 && Gems[x-1, y] != null && Gems[x-2, y] != null &&
            Gems[x-1, y].Type == Gems[x-2, y].Type)
            excluded.Add(Gems[x-1, y].Type);

        if (y >= 2 && Gems[x, y-1] != null && Gems[x, y-2] != null &&
            Gems[x, y-1].Type == Gems[x, y-2].Type)
            excluded.Add(Gems[x, y-1].Type);

        List<Gem.GemType> allTypes = new List<Gem.GemType>((Gem.GemType[])System.Enum.GetValues(typeof(Gem.GemType)));
        foreach (var e in excluded) allTypes.Remove(e);

        return allTypes[Random.Range(0, allTypes.Count)];
    }

    public void SpawnGem(int x, int y, Gem.GemType type)
    {
        Vector3 pos = GetWorldPosition(x, y);
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = $"Gem_{x}_{y}";
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * gemSize;
        go.transform.SetParent(transform);

        // Remove 3D collider, add 2D
        Destroy(go.GetComponent<MeshCollider>());
        go.AddComponent<BoxCollider2D>();

        // Remove 3D renderer, add sprite renderer
        Destroy(go.GetComponent<MeshRenderer>());
        Destroy(go.GetComponent<MeshFilter>());

        Gem gem = go.AddComponent<Gem>();
        gem.Init(type, x, y);
        Gems[x, y] = gem;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * (gemSize + gemSpacing), y * (gemSize + gemSpacing), 0);
    }

    public IEnumerator SwapGems(int x1, int y1, int x2, int y2)
    {
        isProcessing = true;

        Gem gem1 = Gems[x1, y1];
        Gem gem2 = Gems[x2, y2];

        Gems[x1, y1] = gem2;
        Gems[x2, y2] = gem1;
        gem1.Column = x2; gem1.Row = y2;
        gem2.Column = x1; gem2.Row = y1;

        gem1.MoveTo(GetWorldPosition(x2, y2));
        gem2.MoveTo(GetWorldPosition(x1, y1));

        yield return new WaitUntil(() => !gem1.IsMoving() && !gem2.IsMoving());

        List<Gem> matches = MatchFinder.Instance.FindAllMatches(Gems, width, height);

        if (matches.Count == 0)
        {
            // Swap back
            Gems[x1, y1] = gem1;
            Gems[x2, y2] = gem2;
            gem1.Column = x1; gem1.Row = y1;
            gem2.Column = x2; gem2.Row = y2;
            gem1.MoveTo(GetWorldPosition(x1, y1));
            gem2.MoveTo(GetWorldPosition(x2, y2));
            yield return new WaitUntil(() => !gem1.IsMoving() && !gem2.IsMoving());
        }
        else
        {
            GameManager.Instance.UseMove();
            yield return StartCoroutine(ProcessMatches());
        }

        isProcessing = false;
    }

    IEnumerator ProcessMatches()
    {
        while (true)
        {
            List<Gem> matches = MatchFinder.Instance.FindAllMatches(Gems, width, height);
            if (matches.Count == 0) break;

            ScoreManager.Instance.AddScore(matches.Count * 50);
            AudioManager.Instance?.PlayMatch();

            foreach (Gem g in matches)
            {
                if (Gems[g.Column, g.Row] == g)
                    Gems[g.Column, g.Row] = null;
                Destroy(g.gameObject);
            }

            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(ApplyGravity());
            yield return new WaitForSeconds(0.1f);
            yield return StartCoroutine(FillEmpty());
            yield return new WaitForSeconds(0.2f);
        }

        GameManager.Instance.CheckWin();
    }

    IEnumerator ApplyGravity()
    {
        bool moved = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 1; y < height; y++)
            {
                if (Gems[x, y] == null) continue;
                int fallY = y;
                while (fallY > 0 && Gems[x, fallY - 1] == null) fallY--;
                if (fallY != y)
                {
                    Gems[x, fallY] = Gems[x, y];
                    Gems[x, y] = null;
                    Gems[x, fallY].Row = fallY;
                    Gems[x, fallY].MoveTo(GetWorldPosition(x, fallY));
                    moved = true;
                }
            }
        }

        if (moved)
        {
            yield return new WaitForSeconds(0.3f);
            bool anyMoving = true;
            while (anyMoving)
            {
                anyMoving = false;
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        if (Gems[x, y] != null && Gems[x, y].IsMoving())
                            anyMoving = true;
                if (anyMoving) yield return null;
            }
        }
    }

    IEnumerator FillEmpty()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Gems[x, y] == null)
                {
                    Gem.GemType type = (Gem.GemType)Random.Range(0, System.Enum.GetValues(typeof(Gem.GemType)).Length);
                    SpawnGem(x, y, type);
                    Gems[x, y].transform.position = GetWorldPosition(x, height);
                    Gems[x, y].MoveTo(GetWorldPosition(x, y));
                }
            }
        }

        yield return new WaitForSeconds(0.3f);
    }

    public bool IsProcessing() => isProcessing;
}
