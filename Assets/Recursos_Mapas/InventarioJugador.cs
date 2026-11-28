using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InventarioJugador : MonoBehaviour
{
    public static InventarioJugador Instancia;

    [Header("Progreso de Llaves")]
    public int llavesActuales = 0;
    public int llavesTotalesRequeridas = 3;

    [Header("UI - Referencia al Texto")]
    [SerializeField] private TextMeshProUGUI textoLlaves;
    [SerializeField] private string nombreObjetoTextoUI = "TextoLlaves";

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            if (transform.parent == null)
                DontDestroyOnLoad(gameObject);
        }
        else if (Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        llavesActuales = 0;
        ReenlazarTextoUI();
    }

    public static InventarioJugador ObtenerOCrear()
    {
        if (Instancia != null)
            return Instancia;

        InventarioJugador existente = FindFirstObjectByType<InventarioJugador>(FindObjectsInactive.Include);
        if (existente != null)
        {
            Instancia = existente;
            if (existente.transform.parent == null)
                DontDestroyOnLoad(existente.gameObject);
            existente.ReenlazarTextoUI();
            existente.ActualizarUI();
            return existente;
        }

        GameObject go = new GameObject("SistemaInventario");
        InventarioJugador nuevo = go.AddComponent<InventarioJugador>();
        DontDestroyOnLoad(go);
        Instancia = nuevo;
        return nuevo;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReenlazarTextoUI();

        if (scene.name.Contains("mapa") || scene.name.Contains("Mapa"))
        {
            llavesActuales = 0;
        }

        ActualizarUI();
    }

    private void ReenlazarTextoUI()
    {
        textoLlaves = null;

        Canvas[] canvases = FindObjectsByType<Canvas>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        // 1. Buscar por el nombre exacto configurado en Nombre Objeto Texto UI
        if (!string.IsNullOrEmpty(nombreObjetoTextoUI))
        {
            foreach (Canvas canvas in canvases)
            {
                if (canvas == null) continue;

                Transform[] hijos = canvas.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in hijos)
                {
                    if (t != null && t.name == nombreObjetoTextoUI)
                    {
                        textoLlaves = t.GetComponent<TextMeshProUGUI>();
                        if (textoLlaves != null)
                            return;
                    }
                }
            }
        }

        // 2. Respaldo: buscar algún TMP cuyo nombre contenga "llave"
        foreach (Canvas canvas in canvases)
        {
            if (canvas == null) continue;

            TextMeshProUGUI[] textos = canvas.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI tmp in textos)
            {
                if (tmp != null && tmp.name.ToLower().Contains("llave"))
                {
                    textoLlaves = tmp;
                    return;
                }
            }
        }
    }

    private void Start()
    {
        ReenlazarTextoUI();
        ActualizarUI();
    }

    public void AgregarLlave()
    {
        llavesActuales++;
        ActualizarUI();
        Debug.Log("¡Llave obtenida! Total: " + llavesActuales);
    }

    public void RestaurarLlaves(int cantidad)
    {
        llavesActuales = Mathf.Max(0, cantidad);
        ReenlazarTextoUI();
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (textoLlaves != null)
        {
            textoLlaves.text = $"{llavesActuales} / {llavesTotalesRequeridas}";
        }
    }
}