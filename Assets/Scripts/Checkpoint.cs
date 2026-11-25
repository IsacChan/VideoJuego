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

    private VidaJugador vidaJugador;
    private caminar jugadorActual;

    private bool jugadorDentro = false;
    private bool ignorarEntrada = false;

    private void Start()
    {
        if (panelGuardar != null)
            panelGuardar.SetActive(false);

        if (botonGuardar != null)
            botonGuardar.onClick.AddListener(GuardarPartida);

        if (botonCancelar != null)
            botonCancelar.onClick.AddListener(CancelarGuardado);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        caminar jugador = other.GetComponent<caminar>();

        if (jugador == null || jugadorDentro)
            return;

        jugadorDentro = true;

        if (ignorarEntrada)
        {
            Debug.Log("Entrada al checkpoint ignorada.");
            return;
        }

        VidaJugador vida = jugador.GetComponent<VidaJugador>();

        if (vida == null)
            return;

        jugadorActual = jugador;
        vidaJugador = vida;

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
        caminar jugador = other.GetComponent<caminar>();

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
            int llaves = InventarioJugador.Instancia != null
                ? InventarioJugador.Instancia.llavesActuales
                : 0;

            SistemaGuardado.Guardar(
                transform.position,
                jugadorActual.vidaActual,
                llaves
            );

            // Guardar ítems obtenidos del InventoryManager
            List<SlotGuardado> slotsActuales = InventoryManager.instancia != null
                ? InventoryManager.instancia.ObtenerSlotsGuardados()
                : new List<SlotGuardado>();

            string jsonSlots = JsonUtility.ToJson(
                new ContenedorSlotsPartida { slots = slotsActuales }
            );
            PlayerPrefs.SetString("SlotsInventarioGuardados", jsonSlots);

            // Aquí puedes guardar también tu variable de correr (ej. PlayerPrefs.SetFloat("EnergiaCorrerGuardada", ...))
            PlayerPrefs.Save();

            Debug.Log("Partida guardada correctamente.");

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