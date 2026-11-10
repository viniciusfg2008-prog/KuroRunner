using UnityEngine;

/// <summary>Item/obstáculo da fase (trigger). Tipo define o efeito no jogador.</summary>
public class ItemFase : MonoBehaviour
{
    public enum Tipo { Moeda, Dano, VidaExtra, Chegada, Inimigo }
    public Tipo tipo;

    void OnTriggerEnter2D(Collider2D c) { Tratar(c); }

    void OnTriggerStay2D(Collider2D c)
    {
        if (tipo == Tipo.Dano || tipo == Tipo.Inimigo) Tratar(c);
    }

    void Tratar(Collider2D c)
    {
        JogadorKuro j = c.GetComponent<JogadorKuro>();
        if (j == null) return;
        switch (tipo)
        {
            case Tipo.Moeda: j.AdicionarMoeda(); gameObject.SetActive(false); break;
            case Tipo.VidaExtra: j.GanharVida(); gameObject.SetActive(false); break;
            case Tipo.Chegada: j.Vencer(); break;
            default: j.ReceberDano(transform.position); break;
        }
    }
}
