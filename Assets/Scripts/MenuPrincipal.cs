using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [Tooltip("Nombre exacto de la escena del juego a la que se cambiará al pulsar Jugar")]
    public string nombreEscenaJuego = "mapa 1"; // Cambia esto si tu escena se llama distinto

    [Header("Paneles de Interfaz")]
    public GameObject panelOpciones; // Arrastra aquí tu panel de opciones desde la jerarquía
    public GameObject panelHistoria; // Arrastra aquí tu panel de historia desde la jerarquía

    public void Jugar()
    {
        // En lugar de cargar el juego de golpe, ahora abrimos el panel de historia
        if (panelHistoria != null)
        {
            panelHistoria.SetActive(true);
        }
        else
        {
            // Si por alguna razón no usas panel de historia, carga la escena directo
            SceneManager.LoadScene(nombreEscenaJuego);
        }
    }

    // Esta función la llamará el botón "Comenzar" dentro de la pantalla de historia
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
        
        // Nota: Application.Quit() no cierra el editor de Unity al hacer Play, 
        // pero funcionará perfectamente cuando exportes/compiles el juego.
    }
}