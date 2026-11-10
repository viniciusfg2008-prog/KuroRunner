using UnityEngine;

/// <summary>Câmera 2D que segue o alvo sem sair dos limites da fase.</summary>
public class CameraSegue : MonoBehaviour
{
    public Transform alvo;
    public Vector2 min = Vector2.zero, max = new Vector2(64, 32);

    void LateUpdate()
    {
        if (alvo == null) return;
        Camera cam = GetComponent<Camera>();
        float h = cam.orthographicSize, w = h * cam.aspect;
        float x = Mathf.Clamp(alvo.position.x, min.x + w, Mathf.Max(min.x + w, max.x - w));
        float y = Mathf.Clamp(alvo.position.y, min.y + h, Mathf.Max(min.y + h, max.y - h));
        Vector3 destino = new Vector3(x, y, -10f);
        transform.position = Vector3.Lerp(transform.position, destino, 1f - Mathf.Exp(-10f * Time.unscaledDeltaTime));
    }
}
