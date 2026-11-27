
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[Serializable]
public class ContenedorSlotsPartida
{
    public List<SlotGuardado> slots = new List<SlotGuardado>();
}

public class Checkpoint : MonoBehaviour
{
    [Header("Interfaz")]
    public GameObject panelGuardar;
    public Button botonGuardar;
    public Button botonCancelar;

    [Header("Referencias del jugador")]
    private VidaJugador vidaJugador;
    private caminar jugadorActual;

    [Header("Inventario del mapa")]
    private InventarioJugador inventario;

    private bool jugadorDentro = false;
    private bool ignorarEntrada = false;

    private void Start()
    {
        // Obtener el inventario perteneciente a esta escena.
        BuscarInventario();

        if (panelGuardar != null)
            panelGuardar.SetActive(false);

        if (botonGuardar != null)
            botonGuardar.onClick.AddListener(GuardarPartida);

        if (botonCancelar != null)
            botonCancelar.onClick.AddListener(CancelarGuardado);
    }

    private void BuscarInventario()
    {
        inventario = FindFirstObjectByType<InventarioJugador>();

        if (inventario == null)
        {
            Debug.LogError(
                "Checkpoint: No se encontró InventarioJugador en este mapa."
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        caminar jugador = other.GetComponentInParent<caminar>();

        if (jugador == null || jugadorDentro)
            return;

        if (ignorarEntrada)
        {
            jugadorDentro = true;
            Debug.Log("Entrada al checkpoint ignorada.");
            return;
        }

        VidaJugador vida = jugador.GetComponent<VidaJugador>();

        if (vida == null)
            vida = jugador.GetComponentInParent<VidaJugador>();

        if (vida == null)
        {
            Debug.LogWarning(
                "No se encontró VidaJugador en el jugador."
            );
            return;
        }

        jugadorDentro = true;

        jugadorActual = jugador;
        vidaJugador = vida;

        // Activar checkpoint de reaparición.
        vidaJugador.ActivarCheckpoint(transform.position);

        Debug.Log(
            "Checkpoint activado: " + transform.position
        );

        if (panelGuardar != null)
        {
            panelGuardar.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        caminar jugador = other.GetComponentInParent<caminar>();

        if (jugador == null)
            return;

        jugadorDentro = false;
        ignorarEntrada = false;

        Debug.Log("Jugador salió del checkpoint.");
    }

    public void GuardarPartida()
    {
        if (vidaJugador == null || jugadorActual == null)
            return;

        try
        {
            // Obtener las llaves del inventario de este mapa.
            if (inventario == null)
                BuscarInventario();

            int llaves = inventario != null
                ? inventario.llavesActuales
                : 0;

            // Guardar progreso principal.
            SistemaGuardado.Guardar(
                transform.position,
                jugadorActual.vidaActual,
                llaves
            );

            // Guardar objetos del inventario.
            List<SlotGuardado> slotsActuales =
                InventoryManager.instancia != null
                ? InventoryManager.instancia.ObtenerSlotsGuardados()
                : new List<SlotGuardado>();

            string jsonSlots = JsonUtility.ToJson(
                new ContenedorSlotsPartida
                {
                    slots = slotsActuales
                }
            );

            PlayerPrefs.SetString(
                "SlotsInventarioGuardados",
                jsonSlots
            );

            PlayerPrefs.Save();

            Debug.Log(
                "Partida guardada correctamente. Llaves: " + llaves
            );

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
        Debug.Log(
            "Guardado cancelado. Checkpoint sigue activo."
        );

        CerrarPanel();
    }

    private void CerrarPanel()
    {
        if (panelGuardar != null)
            panelGuardar.SetActive(false);

        Time.timeScale = 1f;
    }

    public void IgnorarEntradaInicial()
    {
        ignorarEntrada = true;
        jugadorDentro = true;

        if (panelGuardar != null)
            panelGuardar.SetActive(false);

        Debug.Log(
            "Checkpoint preparado para reaparición."
        );
    }

    private void OnDisable()
    {
        if (panelGuardar != null &&
            panelGuardar.activeSelf)
        {
            panelGuardar.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void OnDestroy()
    {
        if (botonGuardar != null)
            botonGuardar.onClick.RemoveListener(GuardarPartida);

        if (botonCancelar != null)
            botonCancelar.onClick.RemoveListener(CancelarGuardado);
    }
}
