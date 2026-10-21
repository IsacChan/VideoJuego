
using UnityEngine;
using UnityEngine.UI;
using System;

public class Checkpoint : MonoBehaviour
{
    [Header("Interfaz")]
    public GameObject panelGuardar;
    public Button botonGuardar;
    public Button botonCancelar;

    private VidaJugador vidaJugador;
    private caminar jugadorActual;

    private bool jugadorDentro = false;
    private bool ignorarEntrada = false;

    private void Start()
    {
        panelGuardar.SetActive(false);

        botonGuardar.onClick.AddListener(GuardarPartida);
        botonCancelar.onClick.AddListener(CancelarGuardado);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        caminar jugador = other.GetComponent<caminar>();

        if (jugador == null || jugadorDentro)
            return;

        jugadorDentro = true;

        // Ignorar entrada después de cargar partida
        if (ignorarEntrada)
            return;

        VidaJugador vida = jugador.GetComponent<VidaJugador>();

        if (vida == null)
            return;

        jugadorActual = jugador;
        vidaJugador = vida;

        panelGuardar.SetActive(true);

        Time.timeScale = 0f;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<caminar>() != null)
        {
            jugadorDentro = false;
            ignorarEntrada = false;
        }
    }

    public void GuardarPartida()
    {
        if (vidaJugador == null || jugadorActual == null)
            return;

        try
        {
            int llaves = InventarioJugador.Instancia != null
                ? InventarioJugador.Instancia.llavesActuales
                : 0;

            SistemaGuardado.Guardar(
                transform.position,
                jugadorActual.vidaActual,
                llaves
            );

            vidaJugador.ActivarCheckpoint(transform.position);

            Debug.Log("Partida guardada correctamente");

            CerrarPanel();
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Error al guardar: " + e.Message
            );
        }
    }

    public void CancelarGuardado()
    {
        CerrarPanel();
    }

    private void CerrarPanel()
    {
        panelGuardar.SetActive(false);
        Time.timeScale = 1f;
    }

    // Se llama al cargar una partida
    public void IgnorarEntradaInicial()
    {
        ignorarEntrada = true;
        jugadorDentro = true;

        if (panelGuardar != null)
            panelGuardar.SetActive(false);
    }

    private void OnDisable()
    {
        if (panelGuardar != null &&
            panelGuardar.activeSelf)
        {
            Time.timeScale = 1f;
        }
    }
}
