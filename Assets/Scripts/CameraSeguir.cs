using UnityEngine;

// Faz a câmera seguir a personagem principal suavemente (jogo 2D).
public class CameraSeguir : MonoBehaviour
{
    [Header("Alvo")]
    public Transform alvo;

    [Header("Movimento")]
    public Vector3 deslocamento = new Vector3(0f, 1f, -10f);
    [Range(0.01f, 1f)] public float suavidade = 0.125f;

    [Header("Limites (opcional)")]
    public bool usarLimites = false;
    public Vector2 limiteMinimo;
    public Vector2 limiteMaximo;

    void LateUpdate()
    {
        if (alvo == null) return;

        Vector3 desejada = alvo.position + deslocamento;
        Vector3 posicao = Vector3.Lerp(transform.position, desejada, suavidade);

        if (usarLimites)
        {
            posicao.x = Mathf.Clamp(posicao.x, limiteMinimo.x, limiteMaximo.x);
            posicao.y = Mathf.Clamp(posicao.y, limiteMinimo.y, limiteMaximo.y);
        }

        posicao.z = deslocamento.z;
        transform.position = posicao;
    }
}
