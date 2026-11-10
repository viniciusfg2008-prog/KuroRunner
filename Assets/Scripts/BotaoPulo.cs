using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Botão de pulo (lado direito da tela).</summary>
public class BotaoPulo : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData e) { JogadorKuro.PuloSolicitado = true; }
}
