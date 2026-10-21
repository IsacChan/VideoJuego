
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegresarMenu : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreEscenaMenu = "MenuPrincipal";

    public void VolverAlMenu()
    {
        // Restaurar el tiempo por si el juego está pausado
        Time.timeScale = 1f;

        // Limpiar datos pendientes de transporte
        DatosTransporte.puntoAparicion = "";

        // Regresar al menú principal
        SceneManager.LoadScene(nombreEscenaMenu);

        Debug.Log("Regresando al menú principal...");
    }
}

