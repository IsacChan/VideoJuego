
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
    private Coroutine respawnActivo;

    private void Awake()
    {
        jugador = GetComponent<caminar>();
        rb = GetComponent<Rigidbody2D>();

        // La posición inicial siempre será donde aparece
        // originalmente el jugador en la escena.
        ultimoCheckpoint = transform.position;

        Debug.Log(
            "Posición inicial de reaparición: " +
            ultimoCheckpoint
        );
    }

    public void ActivarCheckpoint(Vector3 posicion)
    {
        ultimoCheckpoint = posicion;

        Debug.Log(
            "Nuevo punto de reaparición activado: " +
            ultimoCheckpoint
        );
    }

    public void IniciarRespawn()
    {
        if (respawnActivo != null)
            return;

        respawnActivo = StartCoroutine(Reaparecer());
    }

    private IEnumerator Reaparecer()
    {
        yield return new WaitForSeconds(tiempoRespawn);

        respawnActivo = null;

        RevivirEnCheckpoint();
    }

    public void RevivirEnCheckpoint()
    {
        // Detener cualquier respawn anterior.
        if (respawnActivo != null)
        {
            StopCoroutine(respawnActivo);
            respawnActivo = null;
        }

        // Evitar que se active la ventana de guardado
        // al reaparecer dentro de un checkpoint.
        Checkpoint[] checkpoints =
            FindObjectsByType<Checkpoint>(
                FindObjectsSortMode.None
            );

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (Vector2.Distance(
                checkpoint.transform.position,
                ultimoCheckpoint
            ) < 0.5f)
            {
                checkpoint.IgnorarEntradaInicial();
            }
        }

        // Detener movimiento del jugador.
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;

            rb.position = new Vector2(
                ultimoCheckpoint.x,
                ultimoCheckpoint.y
            );
        }

        // Mover al último punto de aparición.
        transform.position = ultimoCheckpoint;

        Physics2D.SyncTransforms();

        // Restaurar vida.
        if (jugador != null)
        {
            jugador.Revivir();
        }

        Debug.Log(
            "Jugador revivido en: " +
            ultimoCheckpoint
        );
    }
}
