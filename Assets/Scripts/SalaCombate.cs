
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class SalaCombate : MonoBehaviour
{
    [Header("Puertas")]
    public GameObject[] puertas;

    [Header("Configuracion")]
    public string tagJugador = "Player";
    public string tagEnemigo = "Enemy";

    [Header("Sistema de guardado")]
[SerializeField] private string idSala = "mapa1_sala_01";

public bool EstaCompletada => salaCompletada;

    [Header("Deteccion")]
    public LayerMask capasDeteccion = ~0;

    [Header("Margen de entrada")]
    public float margenEntrada = 0.5f;

    private BoxCollider2D zona;

    private bool combateIniciado = false;
    private bool salaCompletada = false;
    private bool enemigosDetectados = false;

    private Transform jugadorDentro;

    void Awake()
    {
        zona = GetComponent<BoxCollider2D>();
        zona.isTrigger = true;
    }

    void Start()
{
    if (ProgresoMundo.EstaCompletado(idSala))
    {
        salaCompletada = true;
        combateIniciado = false;
        enemigosDetectados = false;

        CambiarPuertas(false);

        Debug.Log("Sala restaurada: " + idSala);
        return;
    }

    CambiarPuertas(false);
}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (salaCompletada || combateIniciado)
            return;

        if (!EsJugador(other))
            return;

        jugadorDentro = other.attachedRigidbody != null
            ? other.attachedRigidbody.transform
            : other.transform;

        Debug.Log("Jugador detectado entrando a la sala.");
    }

    void Update()
    {
        if (salaCompletada)
            return;

        // Detectar si el jugador fue transportado
        // fuera de la sala durante el combate.
        if (combateIniciado && !JugadorEstaEnSala())
        {
            ReiniciarCombate();
            return;
        }

        // Esperar a que entre completamente.
        if (!combateIniciado && jugadorDentro != null)
        {
            if (JugadorEntroCompletamente())
            {
                int enemigos = ContarEnemigos();

                Debug.Log(
                    "Enemigos detectados en sala: " + enemigos
                );

                if (enemigos > 0)
                {
                    combateIniciado = true;
                    enemigosDetectados = true;

                    CambiarPuertas(true);

                    Debug.Log(
                        "Jugador dentro. Sala cerrada."
                    );
                }
                else
                {
                    CompletarSala();
                }
            }
        }

        if (!combateIniciado)
            return;

        int enemigosVivos = ContarEnemigos();

        if (enemigosDetectados && enemigosVivos == 0)
        {
            CompletarSala();
        }
    }

    bool EsJugador(Collider2D other)
    {
        if (other.CompareTag(tagJugador))
            return true;

        if (other.attachedRigidbody != null &&
            other.attachedRigidbody.CompareTag(tagJugador))
            return true;

        return false;
    }

    // Comprobar solamente X e Y.
    // No utilizar Bounds.Contains porque incluye Z.
    bool JugadorEstaEnSala()
    {
        if (jugadorDentro == null)
            return false;

        Bounds limites = zona.bounds;
        Vector3 posicion = jugadorDentro.position;

        return
            posicion.x >= limites.min.x &&
            posicion.x <= limites.max.x &&
            posicion.y >= limites.min.y &&
            posicion.y <= limites.max.y;
    }

    bool JugadorEntroCompletamente()
    {
        if (jugadorDentro == null)
            return false;

        Bounds limites = zona.bounds;
        Vector3 posicion = jugadorDentro.position;

        return
            posicion.x > limites.min.x + margenEntrada &&
            posicion.x < limites.max.x - margenEntrada &&
            posicion.y > limites.min.y + margenEntrada &&
            posicion.y < limites.max.y - margenEntrada;
    }

    void ReiniciarCombate()
    {
        combateIniciado = false;
        enemigosDetectados = false;
        jugadorDentro = null;

        CambiarPuertas(false);

        Debug.Log(
            "Jugador fuera de la sala. Puertas abiertas."
        );
    }

    void CompletarSala()
{
    if (salaCompletada)
        return;

    salaCompletada = true;
    combateIniciado = false;
    enemigosDetectados = false;
    jugadorDentro = null;

    CambiarPuertas(false);

    // Registrar el progreso en memoria
    ProgresoMundo.Registrar(idSala);

    Debug.Log(
        "Sala completada y registrada: " + idSala
    );
}

    int ContarEnemigos()
    {
        Collider2D[] objetos = Physics2D.OverlapBoxAll(
            zona.bounds.center,
            zona.bounds.size,
            0f,
            capasDeteccion
        );

        HashSet<GameObject> enemigos =
            new HashSet<GameObject>();

        foreach (Collider2D objeto in objetos)
        {
            Transform actual = objeto.transform;

            while (actual != null)
            {
                if (actual.CompareTag(tagEnemigo))
                {
                    enemigos.Add(actual.gameObject);
                    break;
                }

                actual = actual.parent;
            }
        }

        return enemigos.Count;
    }

    void CambiarPuertas(bool cerrar)
    {
        foreach (GameObject puerta in puertas)
        {
            if (puerta != null)
                puerta.SetActive(cerrar);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!EsJugador(other))
            return;

        if (combateIniciado && !salaCompletada)
        {
            // Solo reiniciar cuando realmente
            // esté fuera de los límites de la sala.
            if (!JugadorEstaEnSala())
            {
                ReiniciarCombate();
            }
        }
        else
        {
            jugadorDentro = null;
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
