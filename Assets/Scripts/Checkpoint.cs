using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        caminar jugador = other.GetComponent<caminar>();

        if (jugador != null)
        {
            VidaJugador vida = jugador.GetComponent<VidaJugador>();

            if (vida != null)
            {
                vida.ActivarCheckpoint(transform.position);
            }
        }
    }
}