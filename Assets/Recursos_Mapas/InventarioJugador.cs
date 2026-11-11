using UnityEngine;
using UnityEngine.SceneManagement; // <-- IMPORTANTE: Necesario para detectar el cambio de escena
using TMPro; 

public class InventarioJugador : MonoBehaviour
{
    public static InventarioJugador Instancia;

    [Header("Progreso de Llaves")]
    public int llavesActuales = 0;
    public int llavesTotalesRequeridas = 3;

    [Header("UI - Referencia al Texto")]
    [SerializeField] private TextMeshProUGUI textoLlaves;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject); // Si tu objeto del inventario viaja de escena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Nos suscribimos al evento cuando se carga una escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Nos desuscribimos para evitar errores de memoria
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Este método se ejecuta solito cada vez que cargas un nivel nuevo
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Opcional: Si quieres que las llaves se reinicien SOLO cuando entras a un mapa de juego (puedes filtrar por nombre si gustas)
        if (scene.name.Contains("mapa") || scene.name.Contains("Mapa"))
        {
            llavesActuales = 0;
            ActualizarUI();
        }
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