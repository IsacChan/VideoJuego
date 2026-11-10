using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PortalMapa : MonoBehaviour
{
    [Header("Destino")]
    public string nombreMapa;
    public string puntoDestino;

    [Header("Referencias de UI (Prefab)")]
    public GameObject prefabPanelHistoria;      // Tu prefab del panel de historia
    private GameObject panelInstanciado;

    [Header("Configuración de Transición")]
    public float velocidadFade = 1.5f;          // Velocidad de desvanecimiento

    private bool cambiandoMapa = false;
    private bool cinemáticaLista = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        caminar jugador = other.GetComponentInParent<caminar>();

        if (jugador == null || cambiandoMapa)
            return;

        cambiandoMapa = true;

        // 1. Detenemos al jugador por completo al cruzar el portal
        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
        jugador.enabled = false;

        // 2. Instanciamos tu prefab del panel de historia en el Canvas principal de la escena
        Canvas canvasPrincipal = FindFirstObjectByType<Canvas>();
        if (prefabPanelHistoria != null && canvasPrincipal != null)
        {
            panelInstanciado = Instantiate(prefabPanelHistoria, canvasPrincipal.transform);
            
            RectTransform rect = panelInstanciado.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }

            // 3. Aplicamos el efecto de aparición suave (Fade-in)
            StartCoroutine(FadeInPanel(panelInstanciado));

            // 4. Buscamos tu botón 'BtnComenzar' dentro del prefab clonado y le enlazamos el cambio de escena suave
            Button[] botones = panelInstanciado.GetComponentsInChildren<Button>(true);
            foreach (Button b in botones)
            {
                if (b.gameObject.name == "BtnComenzar")
                {
                    b.onClick.RemoveAllListeners();
                    b.onClick.AddListener(() => StartCoroutine(FadeOutYCambiarEscena()));
                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("¡Falta asignar el Prefab del panel o no hay un Canvas en la escena!");
            StartCoroutine(FadeOutYCambiarEscena());
        }
    }

    // Corrutina para que el panel aparezca suavemente (Fade-in)
    IEnumerator FadeInPanel(GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        panel.SetActive(true);

        float tiempo = 0f;
        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime * velocidadFade;
            canvasGroup.alpha = Mathf.Clamp01(tiempo);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        cinemáticaLista = true;
    }

    // Corrutina para desvanecer el panel antes de cambiar de escena (Fade-out suave)
    IEnumerator FadeOutYCambiarEscena()
    {
        if (panelInstanciado != null)
        {
            CanvasGroup canvasGroup = panelInstanciado.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                float tiempo = 0f;
                while (tiempo < 1f)
                {
                    tiempo += Time.deltaTime * velocidadFade;
                    canvasGroup.alpha = Mathf.Clamp01(1f - tiempo);
                    yield return null;
                }
            }
            Destroy(panelInstanciado);
        }

        // Guardamos el punto de aparición para el siguiente mapa
        DatosTransporte.puntoAparicion = puntoDestino;

        // Cargamos el mapa de destino de manera fluida
        SceneManager.LoadScene(nombreMapa);
    }
}