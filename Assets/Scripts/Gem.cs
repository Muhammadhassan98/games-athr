using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GemType { Red, Blue, Green, Yellow, Purple, Orange }

public class Gem : MonoBehaviour
{
    public GemType Type { get; private set; }
    public int Row;
    public int Col;
    public bool IsSelected { get; private set; }

    private SpriteRenderer spriteRenderer;
    private Coroutine pulseCoroutine;

    private static readonly Color[] GemColors = new Color[]
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        new Color(0.5f, 0f, 0.8f),
        new Color(1f, 0.5f, 0f)
    };

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
    }

    public void Initialize(GemType type, int row, int col)
    {
        Type = type;
        Row = row;
        Col = col;
        transform.localScale = Vector3.one;
        SetColor();
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;

        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        if (IsSelected)
        {
            pulseCoroutine = StartCoroutine(PulseCoroutine());
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    public void SetColor()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            spriteRenderer.color = GemColors[(int)Type];
    }

    private void OnMouseDown()
    {
        if (GemSwapper.Instance == null) return;
        GemSwapper.Instance.OnGemClicked(this);
    }

    private IEnumerator PulseCoroutine()
    {
        float scaleUp = 1.2f;
        float scaleDown = 1.0f;
        float speed = 3.0f;

        while (IsSelected)
        {
            float t = 0f;
            while (t < 1f && IsSelected)
            {
                t += Time.deltaTime * speed;
                float s = Mathf.Lerp(scaleDown, scaleUp, t);
                transform.localScale = new Vector3(s, s, 1f);
                yield return null;
            }

            t = 0f;
            while (t < 1f && IsSelected)
            {
                t += Time.deltaTime * speed;
                float s = Mathf.Lerp(scaleUp, scaleDown, t);
                transform.localScale = new Vector3(s, s, 1f);
                yield return null;
            }
        }

        transform.localScale = Vector3.one;
    }

    public IEnumerator AnimateToPosition(Vector3 target)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        transform.position = target;
    }
}
