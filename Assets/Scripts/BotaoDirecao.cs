using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Botão de direção (modo "botões" das configurações): segure para andar.</summary>
public class BotaoDirecao : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int direcao = 1; // -1 esquerda, 1 direita

    public void OnPointerDown(PointerEventData e) { JogadorKuro.EixoX = direcao; }
    public void OnPointerUp(PointerEventData e) { JogadorKuro.EixoX = 0f; }
}
