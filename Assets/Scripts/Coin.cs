using UnityEngine;

/// <summary>
/// Moeda coletável. Precisa de um Collider2D marcado como "Is Trigger".
/// O jogador precisa ter a tag "Player" (e Rigidbody2D + Collider2D).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [SerializeField] int value = 1;
    [SerializeField] AudioClip collectSound;
    [SerializeField, Range(0f, 1f)] float soundVolume = 1f;

    [Header("Animação (flutuar)")]
    [SerializeField] float bobHeight = 0.05f;
    [SerializeField] float bobSpeed = 3f;

    Vector3 startPos;
    bool collected;

    void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (collected) return;
        float offset = Mathf.Sin(Time.time * bobSpeed + startPos.x) * bobHeight;
        transform.position = startPos + Vector3.up * offset;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player")) return;
        collected = true;

        if (CoinManager.Instance != null)
            CoinManager.Instance.AddCoins(value);

        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position, soundVolume);

        Destroy(gameObject);
    }
}
