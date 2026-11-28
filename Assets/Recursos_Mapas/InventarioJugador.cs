using UnityEngine;
using TMPro;

public class InventarioJugador : MonoBehaviour
{
    [Header("Progreso de Llaves")]
    public int llavesActuales = 0;
    public int llavesTotalesRequeridas = 3;

    [Header("UI - Referencia al Texto")]
    [SerializeField] private TextMeshProUGUI textoLlaves;
    [SerializeField] private string nombreObjetoTextoUI = "TextoLlaves";

    private void Awake()
    {
        llavesActuales = 0;

        if (textoLlaves == null && !string.IsNullOrEmpty(nombreObjetoTextoUI))
        {
            GameObject objTexto = GameObject.Find(nombreObjetoTextoUI);
            if (objTexto != null)
            {
                textoLlaves = objTexto.GetComponent<TextMeshProUGUI>();
            }
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

    public void RestaurarLlaves(int cantidad)
    {
        llavesActuales = Mathf.Max(0, cantidad);
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