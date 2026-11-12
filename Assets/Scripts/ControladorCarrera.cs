using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorCarrera : MonoBehaviour
{
    [Header("Sistema de Carrera")]
    public caminar jugador;

    [Header("Indicador Circular")]
    public Image rellenoCarrera;

    private void OnEnable()
    {
        // Nos suscribimos al evento de cambio de escena para reconectar al nuevo jugador
        SceneManager.sceneLoaded += OnSceneLoaded;
        BuscarJugador();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cada vez que cargue un mapa nuevo, buscamos al nuevo jugador de esa escena
        BuscarJugador();
    }

    private void BuscarJugador()
    {
        if (jugador == null)
        {
            jugador = FindFirstObjectByType<caminar>();
        }
    }

    private void Update()
    {
        // Si por algún motivo aún no lo encuentra, lo sigue intentando buscar
        if (jugador == null)
        {
            jugador = FindFirstObjectByType<caminar>();
            return;
        }

        if (rellenoCarrera == null)
            return;

        rellenoCarrera.fillAmount = jugador.ProgresoCarrera;
    }
}