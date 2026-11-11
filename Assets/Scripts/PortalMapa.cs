using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalMapa : MonoBehaviour
{
    [Header("Destino")]
    public string nombreEscenaHistoria = "EscenaHistoria";
    public string puntoDestino = "EntradaMapa2";

    private bool cambiandoMapa = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        caminar jugador = other.GetComponentInParent<caminar>();

        if (jugador == null || cambiandoMapa)
            return;

        cambiandoMapa = true;

        // 1. Detenemos al jugador por completo al cruzar el portal
        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
        jugador.enabled = false;

        // 2. Guardamos el punto de aparición para el siguiente mapa
        DatosTransporte.puntoAparicion = puntoDestino;

        // 3. Saltamos directo y limpio a la escena de la historia
        SceneManager.LoadScene(nombreEscenaHistoria);
    }
}