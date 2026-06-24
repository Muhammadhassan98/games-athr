using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    public List<List<Gem>> FindAllMatches(Gem[,] grid)
    {
        List<List<Gem>> groups = new List<List<Gem>>();
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Horizontal runs
        for (int r = 0; r < rows; r++)
        {
            int c = 0;
            while (c < cols)
            {
                if (grid[r, c] == null)
                {
                    c++;
                    continue;
                }
                GemType type = grid[r, c].Type;
                int start = c;
                while (c < cols && grid[r, c] != null && grid[r, c].Type == type)
                    c++;
                int length = c - start;
                if (length >= 3)
                {
                    List<Gem> group = new List<Gem>();
                    for (int i = start; i < start + length; i++)
                        group.Add(grid[r, i]);
                    groups.Add(group);
                }
            }
        }

        // Vertical runs
        for (int c = 0; c < cols; c++)
        {
            int r = 0;
            while (r < rows)
            {
                if (grid[r, c] == null)
                {
                    r++;
                    continue;
                }
                GemType type = grid[r, c].Type;
                int start = r;
                while (r < rows && grid[r, c] != null && grid[r, c].Type == type)
                    r++;
                int length = r - start;
                if (length >= 3)
                {
                    List<Gem> group = new List<Gem>();
                    for (int i = start; i < start + length; i++)
                        group.Add(grid[i, c]);
                    groups.Add(group);
                }
            }
        }

        return MergeGroups(groups);
    }

    public List<Gem> FindMatches(Gem[,] grid)
    {
        List<List<Gem>> allGroups = FindAllMatches(grid);
        HashSet<Gem> seen = new HashSet<Gem>();
        List<Gem> result = new List<Gem>();

        foreach (List<Gem> group in allGroups)
        {
            foreach (Gem gem in group)
            {
                if (seen.Add(gem))
                    result.Add(gem);
            }
        }

        return result;
    }

    public bool IsMatch(Gem[,] grid)
    {
        return FindMatches(grid).Count > 0;
    }

    private List<List<Gem>> MergeGroups(List<List<Gem>> groups)
    {
        List<List<Gem>> merged = new List<List<Gem>>();

        foreach (List<Gem> group in groups)
        {
            bool foundOverlap = false;
            foreach (List<Gem> existing in merged)
            {
                // Check if any gem in group is in existing
                foreach (Gem gem in group)
                {
                    if (existing.Contains(gem))
                    {
                        // Merge group into existing
                        foreach (Gem g in group)
                        {
                            if (!existing.Contains(g))
                                existing.Add(g);
                        }
                        foundOverlap = true;
                        break;
                    }
                }
                if (foundOverlap) break;
            }

            if (!foundOverlap)
                merged.Add(new List<Gem>(group));
        }

        // Second pass to catch transitive merges
        bool changed = true;
        while (changed)
        {
            changed = false;
            for (int i = 0; i < merged.Count; i++)
            {
                for (int j = i + 1; j < merged.Count; j++)
                {
                    bool overlaps = false;
                    foreach (Gem gem in merged[i])
                    {
                        if (merged[j].Contains(gem))
                        {
                            overlaps = true;
                            break;
                        }
                    }
                    if (overlaps)
                    {
                        foreach (Gem g in merged[j])
                        {
                            if (!merged[i].Contains(g))
                                merged[i].Add(g);
                        }
                        merged.RemoveAt(j);
                        changed = true;
                        break;
                    }
                }
                if (changed) break;
            }
        }

        return merged;
    }
}
