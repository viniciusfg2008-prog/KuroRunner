using UnityEngine;

/// <summary>Inimigo que vai e volta entre xMin e xMax (em loop).</summary>
public class InimigoPatrulha : MonoBehaviour
{
    public float xMin, xMax, velocidade = 1.5f;
    float dir = 1f;
    SpriteRenderer sr;

    void Awake() { sr = GetComponentInChildren<SpriteRenderer>(); }

    void Update()
    {
        Vector3 p = transform.position;
        p.x += dir * velocidade * Time.deltaTime;
        if (p.x >= xMax) { p.x = xMax; dir = -1f; }
        else if (p.x <= xMin) { p.x = xMin; dir = 1f; }
        transform.position = p;
        if (sr != null) sr.flipX = dir > 0f;
    }
}
