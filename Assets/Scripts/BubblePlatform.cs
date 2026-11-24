using UnityEngine;

/// <summary>
/// Bolha: plataforma que levita o jogador quando ele fica parado em cima dela.
/// - Jogador parado por "tempoParadoParaSubir" segundos -> a bolha sobe com ele.
/// - Se o jogador se mexe, a bolha para na altura atual (da pra sair andando pra uma plataforma mais alta).
/// - Quando o jogador sai da bolha, ela volta devagar ao ponto original.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BubblePlatform : MonoBehaviour
{
    [Header("Levitacao")]
    public float tempoParadoParaSubir = 0.6f;   // tempo parado ate a bolha comecar a subir
    public float velocidadeSubida = 2.5f;
    public float alturaMaxima = 3f;             // altura maxima acima do ponto original
    public float velocidadeVolta = 1.5f;
    public float tempoAntesDeVoltar = 1f;       // espera depois que o jogador sai
    public float limiteParado = 0.15f;          // velocidade horizontal abaixo disso = parado

    [Header("Visual")]
    public Color corNormal = new Color(0.6f, 0.9f, 1f, 0.55f);
    public Color corAtiva = new Color(0.8f, 1f, 1f, 0.85f);

    private Rigidbody2D rb;
    private Collider2D meuCol;
    private SpriteRenderer sr;
    private Rigidbody2D jogadorRb;
    private Collider2D jogadorCol;

    private Vector2 posInicial;
    private float contadorParado;
    private float saiuEm = -999f;
    private bool jogadorEmCima;
    private bool subindo;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        meuCol = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        posInicial = rb.position;
        if (sr != null) sr.color = corNormal;
    }

    void FixedUpdate()
    {
        // jogador ainda em cima? (confirmado em OnCollisionStay2D a cada passo de fisica)
        bool emCima = jogadorEmCima;
        jogadorEmCima = false; // sera reativado por OnCollisionStay2D no proximo passo

        Vector2 pos = rb.position;
        float topo = posInicial.y + alturaMaxima;

        if (emCima && jogadorRb != null)
        {
            saiuEm = Time.time;

            // "parado" = sem movimento horizontal e sem pular
            bool parado = Mathf.Abs(jogadorRb.velocity.x) < limiteParado;
            if (parado) contadorParado += Time.fixedDeltaTime;
            else contadorParado = 0f;

            if (contadorParado >= tempoParadoParaSubir) subindo = true;
            else if (!parado) subindo = false; // mexeu: a bolha segura onde esta

            if (subindo && pos.y < topo)
                pos.y = Mathf.MoveTowards(pos.y, topo, velocidadeSubida * Time.fixedDeltaTime);
        }
        else
        {
            contadorParado = 0f;
            subindo = false;

            // volta ao ponto original depois de um tempo sem jogador
            if (Time.time - saiuEm >= tempoAntesDeVoltar && pos.y > posInicial.y)
                pos.y = Mathf.MoveTowards(pos.y, posInicial.y, velocidadeVolta * Time.fixedDeltaTime);
        }

        rb.MovePosition(pos);

        if (sr != null)
            sr.color = subindo ? corAtiva : corNormal;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;

        // so conta se o jogador esta por cima da bolha (pes acima do topo da bolha)
        Bounds b = meuCol.bounds;
        Bounds p = col.collider.bounds;
        bool porCima = p.min.y >= b.max.y - 0.15f && p.max.x > b.min.x && p.min.x < b.max.x;
        if (!porCima) return;

        jogadorEmCima = true;
        jogadorCol = col.collider;
        jogadorRb = col.rigidbody != null ? col.rigidbody : col.collider.attachedRigidbody;
    }
}
