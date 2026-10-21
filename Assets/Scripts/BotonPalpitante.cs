
using UnityEngine;
using UnityEngine.EventSystems;

public class BotonPalpitante : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Configuración de palpitación")]
    public float velocidad = 5f;
    public float intensidad = 0.1f;

    [Header("Suavizado")]
    public float velocidadRetorno = 10f;

    private RectTransform rectTransform;
    private Vector3 escalaOriginal;
    private bool cursorEncima = false;
    private float tiempo = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        escalaOriginal = rectTransform.localScale;
    }

    private void Update()
    {
        if (cursorEncima)
        {
            tiempo += Time.unscaledDeltaTime * velocidad;

            float pulso = (Mathf.Sin(tiempo) + 1f) / 2f;

            Vector3 nuevaEscala =
                escalaOriginal * (1f + pulso * intensidad);

            rectTransform.localScale = nuevaEscala;
        }
        else
        {
            rectTransform.localScale = Vector3.Lerp(
                rectTransform.localScale,
                escalaOriginal,
                Time.unscaledDeltaTime * velocidadRetorno
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cursorEncima = true;
        tiempo = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursorEncima = false;
    }

    private void OnDisable()
    {
        cursorEncima = false;
        tiempo = 0f;

        if (rectTransform != null)
            rectTransform.localScale = escalaOriginal;
    }
}
