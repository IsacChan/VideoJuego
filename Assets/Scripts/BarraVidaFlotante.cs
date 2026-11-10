using UnityEngine;
using UnityEngine.UI;

public class BarraVidaFlotante : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabBarraVida; // Arrastra aquí tu BarraVidaPrefab desde la carpeta Prefabs
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Altura sobre la cabeza

    private GameObject barraInstanciada;
    private Image imagenRelleno;
    private float vidaMaxima;
    private float vidaActual;

    void Start()
    {
        // Intentamos leer la vida máxima del enemigo que tenga este script en su mismo objeto
        // (Funciona si usa enteros o floats según tu script de vida)
        CaballeroHealth saludCaballero = GetComponent<CaballeroHealth>();
        if (saludCaballero != null)
        {
            vidaMaxima = saludCaballero.vidaMaxima;
            vidaActual = vidaMaxima;
        }

        // Instanciar la barra en el Canvas principal de la escena
        Canvas canvasEscena = FindObjectOfType<Canvas>();
        if (prefabBarraVida != null && canvasEscena != null)
        {
            barraInstanciada = Instantiate(prefabBarraVida, canvasEscena.transform);
            
            Transform rellenoTransform = barraInstanciada.transform.Find("Relleno");
            if (rellenoTransform != null)
            {
                imagenRelleno = rellenoTransform.GetComponent<Image>();
            }
        }
    }

    void LateUpdate()
    {
        // Seguir al enemigo por la pantalla
        if (barraInstanciada != null)
        {
            Vector3 posicionPantalla = Camera.main.WorldToScreenPoint(transform.position + offset);
            barraInstanciada.transform.position = posicionPantalla;
        }
    }

    // Método universal para actualizar la barra cuando recibe daño
    public void ActualizarBarra(int vidaActualRestante, int vidaMax)
    {
        if (imagenRelleno != null)
        {
            imagenRelleno.fillAmount = (float)vidaActualRestante / vidaMax;
        }
    }

    void OnDestroy()
    {
        // Si el enemigo muere y se destruye, borramos su barra de la UI
        if (barraInstanciada != null)
        {
            Destroy(barraInstanciada);
        }
    }
}