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
    }

    private void Start()
    {
        if (checkpointInicial != null)
        {
            ultimoCheckpoint = checkpointInicial.position;
        }
        else
        {
            ultimoCheckpoint = transform.position;
        }
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

        // Detener velocidad
        rb.linearVelocity = Vector2.zero;

        // Teletransportar al checkpoint
        rb.position = new Vector2(
            ultimoCheckpoint.x,
            ultimoCheckpoint.y
        );

        // Restaurar jugador
        jugador.Revivir();

        Debug.Log("Jugador reapareció en el checkpoint");
    }
}