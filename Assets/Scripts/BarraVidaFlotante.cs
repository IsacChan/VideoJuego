using UnityEngine;
using UnityEngine.UI;

public class BarraVidaFlotante : MonoBehaviour
{
    public static bool OcultarPorInventario { get; private set; } = false;

    [Header("Configuración")]
    public GameObject prefabBarraVida; // Arrastra aquí tu BarraVidaPrefab
    [Range(0.3f, 2.5f)]
    public float factorAncho = 1.35f; // Tamaño equilibrado a lo ancho
    [Range(0.3f, 2.5f)]
    public float factorAlto = 1.2f; // Tamaño equilibrado a lo alto
    public float margenArriba = 0.25f; // Separación extra por encima de la cabeza/bounds
    public Vector3 offset = new Vector3(0, 0.8f, 0); // Respaldo si no hay SpriteRenderer ni Collider2D

    [Header("Ajuste por cámara (2D ortográfica)")]
    public bool ajustarPorCamaraOrtografica = true;
    public float tamañoCamaraReferencia = 5f;
    public float escalaMinimaCamara = 0.85f;
    public float escalaMaximaCamara = 1.25f;

    private GameObject barraInstanciada;
    private Image imagenRelleno;
    private SpriteRenderer spriteRenderer;
    private Collider2D colisor2D;
    private Vector3 escalaBasePrefab = Vector3.one;
    private bool visiblePorCombate = true;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        colisor2D = GetComponentInChildren<Collider2D>();

        Canvas canvasEscena = BuscarCanvasPrincipal();

        if (prefabBarraVida != null && canvasEscena != null)
        {
            escalaBasePrefab = prefabBarraVida.transform.localScale;
            barraInstanciada = Instantiate(prefabBarraVida, canvasEscena.transform);
            barraInstanciada.transform.localRotation = Quaternion.identity;

            AplicarProporcion();

            Transform rellenoTransform = barraInstanciada.transform.Find("Relleno");
            if (rellenoTransform != null)
            {
                imagenRelleno = rellenoTransform.GetComponent<Image>();
                if (imagenRelleno != null)
                {
                    imagenRelleno.fillAmount = 1f;
                }
            }

            AplicarEstadoVisual();
            ActualizarPosicionCentrada();
        }
    }

    private Canvas BuscarCanvasPrincipal()
    {
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Canvas respaldo = null;

        foreach (Canvas c in canvases)
        {
            if (c == null || !c.gameObject.activeInHierarchy || !c.isActiveAndEnabled || c.renderMode == RenderMode.WorldSpace)
                continue;

            if (c.gameObject.name == "Canvas")
                return c;

            if (respaldo == null)
                respaldo = c;
        }

        return respaldo;
    }

    void LateUpdate()
    {
        if (barraInstanciada == null)
            return;

        if (OcultarPorInventario || !visiblePorCombate)
        {
            if (barraInstanciada.activeSelf)
                barraInstanciada.SetActive(false);
            return;
        }

        AplicarProporcion();
        ActualizarPosicionCentrada();
    }

    private void AplicarProporcion()
    {
        if (barraInstanciada == null)
            return;

        float multiplicadorCamara = 1f;

        if (ajustarPorCamaraOrtografica && Camera.main != null && Camera.main.orthographic && Camera.main.orthographicSize > 0.01f && tamañoCamaraReferencia > 0.01f)
        {
            multiplicadorCamara = Mathf.Clamp(
                tamañoCamaraReferencia / Camera.main.orthographicSize,
                escalaMinimaCamara,
                escalaMaximaCamara
            );
        }

        float escalaX = escalaBasePrefab.x * factorAncho * multiplicadorCamara;
        float escalaY = escalaBasePrefab.y * factorAlto * multiplicadorCamara;

        barraInstanciada.transform.localScale = new Vector3(
            escalaX,
            escalaY,
            escalaBasePrefab.z
        );
    }

    private Vector3 ObtenerPosicionMundoCentrada()
    {
        if (spriteRenderer != null)
        {
            Bounds b = spriteRenderer.bounds;
            return new Vector3(b.center.x, b.max.y + margenArriba, b.center.z);
        }

        if (colisor2D != null)
        {
            Bounds b = colisor2D.bounds;
            return new Vector3(b.center.x, b.max.y + margenArriba, b.center.z);
        }

        return transform.position + offset;
    }

    private void ActualizarPosicionCentrada()
    {
        if (barraInstanciada == null || Camera.main == null)
            return;

        Vector3 posicionMundo = ObtenerPosicionMundoCentrada();
        Vector3 posicionPantalla = Camera.main.WorldToScreenPoint(posicionMundo);

        if (posicionPantalla.z < 0f)
        {
            if (barraInstanciada.activeSelf)
                barraInstanciada.SetActive(false);
            return;
        }

        if (!barraInstanciada.activeSelf)
            barraInstanciada.SetActive(true);

        barraInstanciada.transform.position = new Vector3(
            Mathf.Round(posicionPantalla.x),
            Mathf.Round(posicionPantalla.y),
            posicionPantalla.z
        );
    }

    public void ActualizarBarra(int vidaActualRestante, int vidaMax)
    {
        if (imagenRelleno != null && vidaMax > 0)
        {
            imagenRelleno.fillAmount = Mathf.Clamp01((float)vidaActualRestante / vidaMax);
        }
    }

    public void SetVisibleCombate(bool visible)
    {
        visiblePorCombate = visible;
        AplicarEstadoVisual();
    }

    private void AplicarEstadoVisual()
    {
        if (barraInstanciada == null)
            return;

        bool debeMostrarse = !OcultarPorInventario && visiblePorCombate;
        if (barraInstanciada.activeSelf != debeMostrarse)
        {
            barraInstanciada.SetActive(debeMostrarse);
        }
    }

    // Llama esto al abrir (ocultar = true) o cerrar (ocultar = false) el inventario
    public static void ConfigurarVisibilidadInventario(bool ocultar)
    {
        OcultarPorInventario = ocultar;

        BarraVidaFlotante[] barras = Object.FindObjectsByType<BarraVidaFlotante>(FindObjectsSortMode.None);
        foreach (BarraVidaFlotante barra in barras)
        {
            if (barra != null)
            {
                barra.AplicarEstadoVisual();
            }
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