using UnityEngine;
using TMPro; // Necesario para TextMeshPro

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
        }
        else
        {
            Destroy(gameObject);
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
}