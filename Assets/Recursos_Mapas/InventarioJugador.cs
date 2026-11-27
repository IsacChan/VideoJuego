
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InventarioJugador : MonoBehaviour
{
    [Header("Progreso de Llaves")]
    public int llavesActuales = 0;
    public int llavesTotalesRequeridas = 3;

    [Header("UI - Referencia al Texto")]
    [SerializeField] private TextMeshProUGUI textoLlaves;

    private void Awake()
    {
        // Este inventario pertenece únicamente a su mapa.
        // No usar DontDestroyOnLoad.
        llavesActuales = 0;
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

    public void RestaurarLlaves(int cantidad)
    {
        llavesActuales = Mathf.Max(0, cantidad);
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (textoLlaves != null)
        {
            textoLlaves.text =
                $"{llavesActuales} / {llavesTotalesRequeridas}";
        }
    }
}
