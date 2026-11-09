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

    // Por defecto, reaparecer donde inicia el jugador
    ultimoCheckpoint = transform.position;

    // Si existe un punto inicial configurado,
    // utilizar su posición.
    if (checkpointInicial != null)
    {
        ultimoCheckpoint = checkpointInicial.position;
    }

    Debug.Log(
        "Punto inicial de reaparición: " +
        ultimoCheckpoint
    );
}
    public void ActivarCheckpoint(Vector3 posicion)
    {
        ultimoCheckpoint = posicion;

        Debug.Log("Nuevo checkpoint: " + posicion);
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

    // Revivir inmediatamente sin guardar partida
    public void RevivirEnCheckpoint()
{
    // 1. Detener respawn anterior
    if (respawnActivo != null)
    {
        StopCoroutine(respawnActivo);
        respawnActivo = null;
    }

    // =====================================
    // 2. EVITAR VENTANA DE GUARDADO AL REVIVIR
    // =====================================

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

            Debug.Log(
                "Checkpoint ignorado durante reaparición."
            );
        }
    }

    // =====================================
    // 3. MOVER JUGADOR AL CHECKPOINT
    // =====================================

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

    // =====================================
    // 4. RESTAURAR VIDA DEL JUGADOR
    // =====================================

    if (jugador != null)
    {
        jugador.Revivir();
    }

    Debug.Log(
        "Jugador revivido en checkpoint: " +
        ultimoCheckpoint
    );
}
}