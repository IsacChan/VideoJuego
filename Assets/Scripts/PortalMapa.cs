
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalMapa : MonoBehaviour
{
    [Header("Destino")]
    public string nombreMapa;
    public string puntoDestino;

    private bool cambiandoMapa = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        caminar jugador = other.GetComponentInParent<caminar>();

        if (jugador == null || cambiandoMapa)
            return;

        cambiandoMapa = true;

        // Guardar el punto donde aparecerá
        DatosTransporte.puntoAparicion = puntoDestino;

        // Cargar el siguiente mapa
        SceneManager.LoadScene(nombreMapa);
    }
}

