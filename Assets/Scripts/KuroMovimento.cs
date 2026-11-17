using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class KuroMovimento : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 5f;

    [Header("Gravidade e Pulo")]
    public float gravidade = 3f;
    public float forcaPulo = 12f;
    public float velocidadeMaximaQueda = 20f;

    [Header("Wall Jump")]
    public float velocidadeDeslizeParede = 2f;   // velocidade maxima de queda ao deslizar na parede
    public float wallJumpHorizontal = 7f;        // impulso para longe da parede
    public float wallJumpVertical = 12f;         // impulso para cima
    public float tempoTravaWallJump = 0.15f;     // tempo sem controle horizontal apos o wall jump
    public float tempoToleranciaParede = 0.1f;   // pode pular da parede logo depois de sair dela

    private Rigidbody2D rb;
    private Collider2D col;
    private ContactPoint2D[] contatos = new ContactPoint2D[16];

    private float inputX;
    private bool pulouPressionado;

    private bool noChao;
    private float normalParedeX;      // -1 ou 1 (lado para onde a parede "empurra"), 0 = sem parede
    private float ultimoTempoParede = -999f;
    private float ultimaNormalParede;
    private float travaHorizontalAte;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.gravityScale = gravidade;
        rb.freezeRotation = true;

        // Sem atrito para o personagem nao "grudar" nas paredes
        rb.sharedMaterial = new PhysicsMaterial2D("KuroSemAtrito") { friction = 0f, bounciness = 0f };
    }

    void Update()
    {
        float x = 0f;

#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.leftArrowKey.isPressed || kb.aKey.isPressed) x -= 1f;
            if (kb.rightArrowKey.isPressed || kb.dKey.isPressed) x += 1f;
            if (kb.spaceKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame)
                pulouPressionado = true;
        }
#else
        x = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            pulouPressionado = true;
#endif

        inputX = x;
    }

    void FixedUpdate()
    {
        rb.gravityScale = gravidade;
        DetectarChaoEParede();

        Vector2 vel = rb.velocity;

        // Movimento horizontal (travado brevemente apos um wall jump)
        if (Time.time >= travaHorizontalAte)
            vel.x = inputX * velocidade;

        // Deslizar na parede: so quando esta no ar, caindo e empurrando contra a parede
        bool empurrandoParede = normalParedeX != 0f && Mathf.Sign(inputX) == -normalParedeX && inputX != 0f;
        if (!noChao && empurrandoParede && vel.y < -velocidadeDeslizeParede)
            vel.y = -velocidadeDeslizeParede;

        // Pulo
        if (pulouPressionado)
        {
            if (noChao)
            {
                vel.y = forcaPulo;
            }
            else if (Time.time - ultimoTempoParede <= tempoToleranciaParede)
            {
                // Wall jump: impulsiona para longe da parede
                vel = new Vector2(ultimaNormalParede * wallJumpHorizontal, wallJumpVertical);
                travaHorizontalAte = Time.time + tempoTravaWallJump;
                ultimoTempoParede = -999f;
            }
            pulouPressionado = false;
        }

        // Limita a velocidade de queda
        if (vel.y < -velocidadeMaximaQueda)
            vel.y = -velocidadeMaximaQueda;

        rb.velocity = vel;
    }

    void DetectarChaoEParede()
    {
        noChao = false;
        normalParedeX = 0f;

        int n = col.GetContacts(contatos);
        for (int i = 0; i < n; i++)
        {
            Vector2 normal = contatos[i].normal;

            if (normal.y > 0.7f)
            {
                noChao = true;
            }
            else if (Mathf.Abs(normal.x) > 0.7f && Mathf.Abs(normal.y) < 0.3f)
            {
                normalParedeX = Mathf.Sign(normal.x);
            }
        }

        if (!noChao && normalParedeX != 0f)
        {
            ultimoTempoParede = Time.time;
            ultimaNormalParede = normalParedeX;
        }
    }
}
