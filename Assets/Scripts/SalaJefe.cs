
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SalaJefe : MonoBehaviour
{
    [Header("Puerta del jefe")]
    public GameObject puerta;

    [Header("Jefe")]
    public GameObject jefe;

    [Header("Llaves")]
    public int llavesNecesarias = 3;

    [Header("Deteccion")]
    public float margenEntrada = 0.5f;

    private BoxCollider2D zona;
    private Collider2D colliderJugador;

    private bool combateIniciado = false;
    private bool jefeDerrotado = false;

    void Awake()
    {
        zona = GetComponent<BoxCollider2D>();
        zona.isTrigger = true;
    }

    void Start()
    {
        // Comienza bloqueada
        CambiarPuerta(true);
    }

    void Update()
    {
        if (jefeDerrotado)
            return;

        if (!combateIniciado)
        {
            VerificarLlaves();
        }
        else
        {
            VerificarJefe();
        }
    }

    void VerificarLlaves()
    {
        if (InventarioJugador.Instancia == null)
        {
            CambiarPuerta(true);
            return;
        }

        int llaves =
            InventarioJugador.Instancia.llavesActuales;

        if (llaves < llavesNecesarias)
        {
            CambiarPuerta(true);
            return;
        }

        // Ya tiene las 3 llaves
        CambiarPuerta(false);

        // Comprobar si entro a la sala
        VerificarEntrada();
    }

    void VerificarEntrada()
    {
        if (colliderJugador == null)
            return;

        Bounds sala = zona.bounds;
        Bounds jugador = colliderJugador.bounds;

        bool dentro =
            jugador.min.x > sala.min.x + margenEntrada &&
            jugador.max.x < sala.max.x - margenEntrada &&
            jugador.min.y > sala.min.y + margenEntrada &&
            jugador.max.y < sala.max.y - margenEntrada;

        if (!dentro)
            return;

        combateIniciado = true;

        // Cerrar la puerta detras del jugador
        CambiarPuerta(true);

        Debug.Log("Combate contra el jefe iniciado");
    }

    void VerificarJefe()
    {
        // Funciona si el jefe se destruye
        // o se desactiva al morir
        if (jefe == null || !jefe.activeInHierarchy)
        {
            jefeDerrotado = true;
            combateIniciado = false;

            CambiarPuerta(false);

            Debug.Log("Jefe derrotado. Puerta abierta.");
        }
    }

    void CambiarPuerta(bool cerrar)
    {
        if (puerta != null &&
            puerta.activeSelf != cerrar)
        {
            puerta.SetActive(cerrar);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            colliderJugador = other;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == colliderJugador)
        {
            colliderJugador = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();

        if (box == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(box.offset, box.size);
    }
}

