using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [Tooltip("Nombre exacto de la escena del juego a la que se cambiará al pulsar Jugar")]
    public string nombreEscenaJuego = "mapa 1";

    [Header("Paneles de Interfaz")]
    public GameObject panelOpciones;   // Arrastra aquí tu panel de opciones desde la jerarquía
    public GameObject panelHistoria;   // Arrastra aquí tu panel de historia desde la jerarquía
    public GameObject panelControles;  // Arrastra aquí tu panel de controles desde la jerarquía

    public void Jugar()
    {
        // Abre el panel de historia al hacer clic en Jugar
        if (panelHistoria != null)
        {
            panelHistoria.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(nombreEscenaJuego);
        }
    }

    // Esta función la llama el botón "Continuar" dentro de la pantalla de historia
    public void IrAControles()
    {
        if (panelHistoria != null) panelHistoria.SetActive(false);
        if (panelControles != null) panelControles.SetActive(true);
    }

    // Esta función la llama el botón "¡A Jugar!" dentro de la pantalla de controles
    public void CargarJuegoReal()
    {
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void AbrirOpciones()
    {
        if (panelOpciones != null)
        {
            panelOpciones.SetActive(true);
        }
        else
        {
            Debug.LogWarning("El panel de opciones no está asignado en el Inspector.");
        }
    }

    public void CerrarOpciones()
    {
        if (panelOpciones != null)
        {
            panelOpciones.SetActive(false);
        }
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}