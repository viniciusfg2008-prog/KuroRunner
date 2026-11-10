using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Joystick virtual (lado esquerdo da tela).</summary>
public class JoystickVirtual : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform fundo, alca;
    public float raio = 110f;

    public void OnPointerDown(PointerEventData e) { OnDrag(e); }

    public void OnDrag(PointerEventData e)
    {
        Vector2 p;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(fundo, e.position, e.pressEventCamera, out p);
        p = Vector2.ClampMagnitude(p, raio);
        if (alca) alca.anchoredPosition = p;
        JogadorKuro.EixoX = p.x / raio;
        JogadorKuro.EixoY = p.y / raio;
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (alca) alca.anchoredPosition = Vector2.zero;
        JogadorKuro.EixoX = JogadorKuro.EixoY = 0f;
    }
}
