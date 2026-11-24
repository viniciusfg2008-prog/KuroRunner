using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Câmera 2D (ortográfica) que segue um alvo de perto, com suavização,
/// e nunca mostra nada fora dos limites do cenário.
/// Se nenhum alvo for definido, procura o objeto chamado "Kuro_0".
/// </summary>
[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour
{
    [Header("Alvo")]
    public Transform target;
    public string targetName = "Kuro_0";

    [Header("Posição")]
    [Tooltip("Deslocamento em XY em relação ao alvo (Z da câmera é mantido).")]
    public Vector2 offset = new Vector2(0f, 0.3f);
    [Tooltip("Tempo de suavização. Menor = mais colada no alvo.")]
    public float smoothTime = 0.06f;

    [Header("Zoom (opcional)")]
    [Tooltip("Se > 0, define o orthographicSize para um enquadramento mais próximo.")]
    public float orthographicSize = 0f;

    [Header("Limites do cenário")]
    public bool limitToBounds = true;
    [Tooltip("Se definido, os limites vêm deste Tilemap. Se vazio, procura um Tilemap na cena.")]
    public Tilemap boundsTilemap;
    [Tooltip("Use para definir os limites manualmente (ignora o Tilemap).")]
    public bool useManualBounds = false;
    public Vector2 manualMin = new Vector2(-11f, -5f);
    public Vector2 manualMax = new Vector2(10.75f, 5.25f);

    private Vector3 velocity;
    private Camera _cam;
    private Camera cam { get { if (_cam == null) _cam = GetComponent<Camera>(); return _cam; } }
    private float fixedZ;
    private Vector2 boundsMin, boundsMax;
    private bool hasBounds;

    void Awake()
    {
        fixedZ = transform.position.z;
        if (orthographicSize > 0f) cam.orthographicSize = orthographicSize;
    }

    void Start()
    {
        RecalculateBounds();
        FindTarget();
        if (target != null) transform.position = Clamp(Desired());
    }

    [ContextMenu("Recalcular limites")]
    public void RecalculateBounds()
    {
        hasBounds = false;
        if (!limitToBounds) return;

        if (useManualBounds)
        {
            boundsMin = manualMin;
            boundsMax = manualMax;
            hasBounds = true;
            return;
        }

        if (boundsTilemap == null) boundsTilemap = FindObjectOfType<Tilemap>();
        if (boundsTilemap == null) return;

        boundsTilemap.CompressBounds();
        var r = boundsTilemap.GetComponent<TilemapRenderer>();
        if (r == null) return;

        Bounds b = r.bounds;
        boundsMin = b.min;
        boundsMax = b.max;
        hasBounds = true;
    }

    void FindTarget()
    {
        if (target != null) return;
        GameObject go = GameObject.Find(targetName);
        if (go != null) target = go.transform;
    }

    Vector3 Desired()
    {
        return new Vector3(target.position.x + offset.x, target.position.y + offset.y, fixedZ);
    }

    /// <summary>Mantém a área visível da câmera dentro dos limites do cenário.</summary>
    Vector3 Clamp(Vector3 pos)
    {
        if (!limitToBounds || !hasBounds) return pos;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float minX = boundsMin.x + halfW, maxX = boundsMax.x - halfW;
        float minY = boundsMin.y + halfH, maxY = boundsMax.y - halfH;

        // Se o cenário for menor que a tela em algum eixo, centraliza nesse eixo.
        pos.x = (minX > maxX) ? (boundsMin.x + boundsMax.x) * 0.5f : Mathf.Clamp(pos.x, minX, maxX);
        pos.y = (minY > maxY) ? (boundsMin.y + boundsMax.y) * 0.5f : Mathf.Clamp(pos.y, minY, maxY);
        return pos;
    }

    void LateUpdate()
    {
        if (target == null) { FindTarget(); if (target == null) return; }

        // Limita o destino (e não só o resultado) para a suavização não "vazar" para fora.
        Vector3 goal = Clamp(Desired());
        Vector3 next = Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime);
        transform.position = Clamp(next);
    }

    void OnDrawGizmosSelected()
    {
        if (!limitToBounds) return;
        if (!Application.isPlaying) RecalculateBounds();
        if (!hasBounds) return;
        Gizmos.color = Color.yellow;
        Vector3 c = new Vector3((boundsMin.x + boundsMax.x) * 0.5f, (boundsMin.y + boundsMax.y) * 0.5f, 0f);
        Vector3 s = new Vector3(boundsMax.x - boundsMin.x, boundsMax.y - boundsMin.y, 0f);
        Gizmos.DrawWireCube(c, s);
    }
}
