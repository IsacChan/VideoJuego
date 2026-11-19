using UnityEngine;
using UnityEngine.UI;

public class BarraVidaFlotante : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabBarraVida; // Arrastra aquí tu BarraVidaPrefab
    public Vector3 offset = new Vector3(0, 0.8f, 0); // Altura sobre la cabeza

    private GameObject barraInstanciada;
    private Image imagenRelleno;

    void Start()
    {
        Canvas canvasEscena = null;
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);

        foreach (Canvas c in canvases)
        {
            if (c != null && c.gameObject.activeInHierarchy && c.isActiveAndEnabled && c.renderMode != RenderMode.WorldSpace)
            {
                canvasEscena = c;
                break;
            }
        }

        if (prefabBarraVida != null && canvasEscena != null)
        {
            barraInstanciada = Instantiate(prefabBarraVida, canvasEscena.transform);
            barraInstanciada.transform.localScale = Vector3.one;

            Transform rellenoTransform = barraInstanciada.transform.Find("Relleno");
            if (rellenoTransform != null)
            {
                imagenRelleno = rellenoTransform.GetComponent<Image>();
                if (imagenRelleno != null)
                {
                    imagenRelleno.fillAmount = 1f;
                }
            }
        }
    }

    void LateUpdate()
    {
        if (barraInstanciada != null && Camera.main != null)
        {
            Vector3 posicionPantalla = Camera.main.WorldToScreenPoint(transform.position + offset);
            barraInstanciada.transform.position = posicionPantalla;
        }
    }

    public void ActualizarBarra(int vidaActualRestante, int vidaMax)
    {
        if (imagenRelleno != null && vidaMax > 0)
        {
            imagenRelleno.fillAmount = Mathf.Clamp01((float)vidaActualRestante / vidaMax);
        }
    }

    void OnDestroy()
    {
        if (barraInstanciada != null)
        {
            Destroy(barraInstanciada);
        }
    }
}