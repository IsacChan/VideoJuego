
using UnityEngine;
using System.Collections;

public class VidaJugador : MonoBehaviour
{
    [Header("Checkpoint")]
    public Transform checkpointInicial;

    [Header("Respawn")]
    public float tiempoRespawn = 2f;

    private Vector3 ultimoCheckpoint;

    private caminar jugador;
    private Rigidbody2D rb;

    private void Awake()
    {
        jugador = GetComponent<caminar>();
        rb = GetComponent<Rigidbody2D>();

        // Inicializar checkpoint desde Awake
        ultimoCheckpoint = checkpointInicial != null
            ? checkpointInicial.position
            : transform.position;
    }

    public void ActivarCheckpoint(Vector3 posicion)
    {
        ultimoCheckpoint = posicion;

        Debug.Log("Nuevo checkpoint: " + posicion);
    }

    public void IniciarRespawn()
    {
        StartCoroutine(Reaparecer());
    }

    private IEnumerator Reaparecer()
    {
        yield return new WaitForSeconds(tiempoRespawn);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;

            rb.position = new Vector2(
                ultimoCheckpoint.x,
                ultimoCheckpoint.y
            );
        }

        transform.position = ultimoCheckpoint;

        Physics2D.SyncTransforms();

        if (jugador != null)
            jugador.Revivir();

        Debug.Log(
            "Jugador reapareció en: " + ultimoCheckpoint
        );
    }
}
