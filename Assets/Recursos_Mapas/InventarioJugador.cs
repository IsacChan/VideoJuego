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
    [SerializeField] private string nombreObjetoTextoUI = "TextoLlaves"; // Pon aquí el nombre exacto de tu TMP en la Hierarchy

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
        if (textoLlaves == null && !string.IsNullOrEmpty(nombreObjetoTextoUI))
        {
            GameObject objTexto = GameObject.Find(nombreObjetoTextoUI);
            if (objTexto != null)
            {
                textoLlaves = objTexto.GetComponent<TextMeshProUGUI>();
            }
        }

        if (scene.name.Contains("mapa") || scene.name.Contains("Mapa"))
        {
            llavesActuales = 0;
        }

        ActualizarUI();
    }

    private void Start()
    {
        ActualizarUI();
    }

    public void AgregarLlave()
    {
        llavesActuales++;
        ActualizarUI();
        Debug.Log("¡Llave obtenida! Total: " + llavesActuales);
    }

    private void ActualizarUI()
    {
        if (textoLlaves != null)
        {
            textoLlaves.text = $"{llavesActuales} / {llavesTotalesRequeridas}";
        }
    }

    public void RestaurarLlaves(int cantidad)
    {
        llavesActuales = Mathf.Max(0, cantidad);
        ActualizarUI();
    }
}