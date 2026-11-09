using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorDerrota : MonoBehaviour
{
    public static ControladorDerrota Instancia;

    [Header("Interfaz")]
    public GameObject panelDerrota;

    [Header("Jugador")]
    public VidaJugador vidaJugador;

    [Header("Menú principal")]
    public string escenaMenu = "MenuPrincipal";

    private bool derrotaActiva = false;

    private void Awake()
    {
        Instancia = this;

        if (panelDerrota != null)
            panelDerrota.SetActive(false);
    }

    public void MostrarDerrota()
    {
        if (derrotaActiva)
            return;

        derrotaActiva = true;

        if (panelDerrota != null)
            panelDerrota.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log("Pantalla de derrota activada.");
    }

    public void Continuar()
    {
        if (!derrotaActiva)
            return;

        if (vidaJugador == null)
        {
            Debug.LogError(
                "No se asignó VidaJugador al controlador."
            );
            return;
        }

        // Reactivar el tiempo
        Time.timeScale = 1f;

        // Cerrar pantalla
        derrotaActiva = false;

        if (panelDerrota != null)
            panelDerrota.SetActive(false);

        // Revivir en último checkpoint
        vidaJugador.RevivirEnCheckpoint();

        Debug.Log(
            "Continuar seleccionado. Partida no guardada."
        );
    }

    public void Salir()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(escenaMenu);
    }

    private void OnDestroy()
    {
        if (Instancia == this)
            Instancia = null;
    }
}