using System.Collections.Generic;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    public static MatchFinder Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public List<Gem> FindAllMatches(Gem[,] gems, int width, int height)
    {
        HashSet<Gem> matched = new HashSet<Gem>();

        // Horizontal
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width - 2; x++)
            {
                Gem g1 = gems[x, y];
                Gem g2 = gems[x+1, y];
                Gem g3 = gems[x+2, y];
                if (g1 != null && g2 != null && g3 != null &&
                    g1.Type == g2.Type && g2.Type == g3.Type)
                {
                    matched.Add(g1); matched.Add(g2); matched.Add(g3);
                    int nx = x + 3;
                    while (nx < width && gems[nx, y] != null && gems[nx, y].Type == g1.Type)
                    {
                        matched.Add(gems[nx, y]);
                        nx++;
                    }
                }
            }
        }

        // Vertical
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 2; y++)
            {
                Gem g1 = gems[x, y];
                Gem g2 = gems[x, y+1];
                Gem g3 = gems[x, y+2];
                if (g1 != null && g2 != null && g3 != null &&
                    g1.Type == g2.Type && g2.Type == g3.Type)
                {
                    matched.Add(g1); matched.Add(g2); matched.Add(g3);
                    int ny = y + 3;
                    while (ny < height && gems[x, ny] != null && gems[x, ny].Type == g1.Type)
                    {
                        matched.Add(gems[x, ny]);
                        ny++;
                    }
                }
            }
        }

        return new List<Gem>(matched);
    }
}
