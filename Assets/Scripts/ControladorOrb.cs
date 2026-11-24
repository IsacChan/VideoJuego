using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorOrb : MonoBehaviour
{
    [Header("Sistema de Embestida")]
    public EmbestidaJugador embestida;

    [Header("Indicador Circular")]
    public Image rellenoOrb;

    private void OnEnable()
    {
        // Nos suscribimos al evento de cambio de escena para reconectar al nuevo jugador
        SceneManager.sceneLoaded += OnSceneLoaded;
        BuscarEmbestida();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cada vez que cargue un mapa nuevo, buscamos la embestida del nuevo Kael
        BuscarEmbestida();
    }

    private void BuscarEmbestida()
    {
        if (embestida == null)
        {
            embestida = FindFirstObjectByType<EmbestidaJugador>();
        }
    }

    private void Update()
    {
        // Si por algún motivo aún no lo encuentra, lo sigue intentando buscar
        if (embestida == null)
        {
            embestida = FindFirstObjectByType<EmbestidaJugador>();
            return;
        }

        if (rellenoOrb == null)
            return;

        rellenoOrb.fillAmount = embestida.ProgresoRecarga;
    }
}