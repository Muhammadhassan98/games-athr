using UnityEngine;

public class GemSwapper : MonoBehaviour
{
    private Gem selectedGem;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!GameManager.Instance.CanMove() || Board.Instance.IsProcessing()) return;

        if (Input.GetMouseButtonDown(0))
        {
            Gem gem = GetGemAtMouse();
            if (gem != null)
            {
                if (selectedGem == null)
                {
                    selectedGem = gem;
                    HighlightGem(gem, true);
                }
                else if (selectedGem == gem)
                {
                    HighlightGem(selectedGem, false);
                    selectedGem = null;
                }
                else if (IsAdjacent(selectedGem, gem))
                {
                    HighlightGem(selectedGem, false);
                    StartCoroutine(Board.Instance.SwapGems(
                        selectedGem.Column, selectedGem.Row,
                        gem.Column, gem.Row));
                    selectedGem = null;
                }
                else
                {
                    HighlightGem(selectedGem, false);
                    selectedGem = gem;
                    HighlightGem(gem, true);
                }
            }
        }
    }

    Gem GetGemAtMouse()
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null)
            return hit.collider.GetComponent<Gem>();
        return null;
    }

    bool IsAdjacent(Gem a, Gem b)
    {
        return (Mathf.Abs(a.Column - b.Column) == 1 && a.Row == b.Row) ||
               (Mathf.Abs(a.Row - b.Row) == 1 && a.Column == b.Column);
    }

    void HighlightGem(Gem gem, bool highlight)
    {
        gem.transform.localScale = highlight ? Vector3.one * 1.15f : Vector3.one;
    }
}
