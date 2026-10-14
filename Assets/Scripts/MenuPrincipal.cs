using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [Tooltip("Nombre exacto de la escena del juego a la que se cambiará al pulsar Jugar")]
    public string nombreEscenaJuego = "mapa 1"; // Cambia esto si tu escena se llama distinto

    [Header("Paneles de Interfaz")]
    public GameObject panelOpciones; // Arrastra aquí tu panel de opciones desde la jerarquía

    public void Jugar()
    {
        // Carga la escena del juego
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