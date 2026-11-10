using UnityEngine;

/// <summary>Controle da Kuro: move com joystick/botões/teclado, pula, perde vidas e coleta estrelas.</summary>
[RequireComponent(typeof(Rigidbody2D))]
public class JogadorKuro : MonoBehaviour
{
    // Preenchidos pelos controles virtuais (JoystickVirtual / BotaoPulo / BotaoDirecao)
    public static float EixoX, EixoY;
    public static bool PuloSolicitado;

    [Header("Movimento")]
    public float velocidade = 5f;
    public float forcaPulo = 10.5f;

    [Header("Vida")]
    public int vidas = 3;
    public int vidasMax = 5;
    public float tempoInvulneravel = 1.5f;

    public int Moedas { get; private set; }

    Rigidbody2D rb;
    SpriteRenderer sr;
    float invulAte, coyote, buffer;
    bool morto;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        EixoX = EixoY = 0f; PuloSolicitado = false;
    }

    void Start()
    {
        if (sr != null) sr.color = GameData.Skins[GameData.SkinSelecionada].cor;
        GerenciadorFase.Atualizar(vidas, Moedas);
    }

    void Update()
    {
        if (morto) return;

        if (PuloSolicitado || Input.GetButtonDown("Jump") || EixoY > 0.75f) buffer = 0.15f;
        PuloSolicitado = false;
        buffer -= Time.deltaTime;

        if (NoChao()) coyote = 0.12f; else coyote -= Time.deltaTime;

        if (buffer > 0f && coyote > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, forcaPulo);
            buffer = 0f; coyote = 0f;
        }

        // Pisca durante a invulnerabilidade
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Time.time < invulAte ? (Mathf.Sin(Time.time * 40f) > 0 ? 0.35f : 1f) : 1f;
            sr.color = c;
        }
    }

    void FixedUpdate()
    {
        if (morto) return;
        float x = Mathf.Abs(EixoX) > 0.15f ? EixoX : Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(Mathf.Clamp(x, -1f, 1f) * velocidade, rb.velocity.y);
        if (sr != null && Mathf.Abs(x) > 0.1f) sr.flipX = x < 0f;
    }

    bool NoChao()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)transform.position + Vector2.down * 0.28f, 0.14f);
        foreach (var h in hits)
            if (!h.isTrigger && h.gameObject != gameObject) return true;
        return false;
    }

    public void ReceberDano(Vector2 origem)
    {
        if (morto || Time.time < invulAte) return;
        vidas--;
        invulAte = Time.time + tempoInvulneravel;
        GerenciadorFase.Atualizar(vidas, Moedas);
        if (vidas <= 0) { Morrer(); return; }
        float dir = transform.position.x >= origem.x ? 1f : -1f;
        rb.velocity = new Vector2(dir * 4f, 6f);
    }

    public void AdicionarMoeda() { Moedas++; GerenciadorFase.Atualizar(vidas, Moedas); }

    public void GanharVida()
    {
        vidas = Mathf.Min(vidasMax, vidas + 1);
        GerenciadorFase.Atualizar(vidas, Moedas);
    }

    void Morrer()
    {
        morto = true;
        rb.velocity = Vector2.zero;
        GerenciadorFase.Instance.Derrota(Moedas);
    }

    public void Vencer()
    {
        if (morto) return;
        morto = true;
        rb.velocity = Vector2.zero;
        GerenciadorFase.Instance.Vitoria(Moedas);
    }
}
