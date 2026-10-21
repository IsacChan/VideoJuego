
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
    public string tagJugador = "Player";

    private BoxCollider2D zona;
    private Collider2D colliderJugador;
    private Transform jugador;

    private bool combateIniciado = false;
    private bool jefeDerrotado = false;

    [Header("Sistema de guardado")]
[SerializeField] private string idSalaJefe = "mapa1_jefe";

    void Awake()
    {
        zona = GetComponent<BoxCollider2D>();
        zona.isTrigger = true;
    }

    void Start()
{
    if (ProgresoMundo.EstaCompletado(idSalaJefe))
    {
        jefeDerrotado = true;
        combateIniciado = false;

        CambiarPuerta(false);

        if (jefe != null)
            jefe.SetActive(false);

        Debug.Log("Jefe derrotado restaurado.");
        return;
    }

    CambiarPuerta(true);
}

    void Update()
{
    if (jefeDerrotado)
        return;

    // Comprobar SIEMPRE si el jefe murió.
    VerificarJefe();

    if (jefeDerrotado)
        return;

    if (combateIniciado)
    {
        if (!JugadorEstaEnSala())
        {
            ReiniciarCombate();
        }

        return;
    }

    VerificarLlaves();
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

        // El jugador tiene las llaves necesarias.
        CambiarPuerta(false);

        // Verificar si entró completamente.
        VerificarEntrada();
    }

    void VerificarEntrada()
    {
        if (colliderJugador == null)
            return;

        Bounds sala = zona.bounds;
        Bounds jugadorBounds = colliderJugador.bounds;

        bool dentro =
            jugadorBounds.min.x > sala.min.x + margenEntrada &&
            jugadorBounds.max.x < sala.max.x - margenEntrada &&
            jugadorBounds.min.y > sala.min.y + margenEntrada &&
            jugadorBounds.max.y < sala.max.y - margenEntrada;

        if (!dentro)
            return;

        combateIniciado = true;

        // Cerrar la puerta detrás del jugador.
        CambiarPuerta(true);

        Debug.Log(
            "Combate contra el jefe iniciado. Puerta cerrada."
        );
    }

    // Comprobar si el jugador sigue dentro.
    // Solo utilizamos X e Y para evitar problemas
    // con el eje Z en juegos 2D.
    bool JugadorEstaEnSala()
    {
        if (jugador == null)
            return false;

        Bounds limites = zona.bounds;
        Vector3 posicion = jugador.position;

        return
            posicion.x >= limites.min.x &&
            posicion.x <= limites.max.x &&
            posicion.y >= limites.min.y &&
            posicion.y <= limites.max.y;
    }

    // Se ejecuta cuando el jugador sale de la sala,
    // incluyendo cuando reaparece en el checkpoint.
    void ReiniciarCombate()
    {
        combateIniciado = false;
        colliderJugador = null;
        jugador = null;

        // Abrir solamente si conserva las llaves.
        if (TieneLlavesNecesarias())
        {
            CambiarPuerta(false);
        }
        else
        {
            CambiarPuerta(true);
        }

        Debug.Log(
            "Jugador fuera de la sala del jefe. Combate reiniciado."
        );
    }

    bool TieneLlavesNecesarias()
    {
        if (InventarioJugador.Instancia == null)
            return false;

        return
            InventarioJugador.Instancia.llavesActuales
            >= llavesNecesarias;
    }

    void VerificarJefe()
{
    if (jefeDerrotado)
        return;

    if (jefe == null)
    {
        Debug.LogWarning(
            "El jefe ya no existe. Completando sala."
        );

        CompletarSala();
        return;
    }

    // Buscar SaludJefe incluso si está en un objeto hijo
    SaludJefe salud = jefe.GetComponentInChildren<SaludJefe>(true);

    if (salud == null)
    {
        Debug.LogWarning(
            "No se encontró SaludJefe en: " + jefe.name
        );

        return;
    }

    // Detectar muerte por estado o por vida
    if (salud.estaMuerto || salud.vidaActual <= 0)
    {
        Debug.Log(
            "Muerte del jefe detectada. Abriendo puerta."
        );

        CompletarSala();
    }
}

    void CompletarSala()
{
    if (jefeDerrotado)
        return;

    jefeDerrotado = true;
    combateIniciado = false;

    colliderJugador = null;
    jugador = null;

    CambiarPuerta(false);

    ProgresoMundo.Registrar(idSalaJefe);

    Debug.Log(
        "JEFE DERROTADO - PUERTA ABIERTA - PROGRESO GUARDADO"
    );
}

    void CambiarPuerta(bool cerrar)
    {
        if (puerta != null &&
            puerta.activeSelf != cerrar)
        {
            puerta.SetActive(cerrar);
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!EsJugador(other))
            return;

        if (jefeDerrotado)
            return;

        colliderJugador = other;

        jugador = other.attachedRigidbody != null
            ? other.attachedRigidbody.transform
            : other.transform;

        Debug.Log(
            "Jugador detectado entrando a la sala del jefe."
        );
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!EsJugador(other))
            return;

        if (combateIniciado && !jefeDerrotado)
        {
            if (!JugadorEstaEnSala())
            {
                ReiniciarCombate();
            }
        }
        else if (other == colliderJugador)
        {
            colliderJugador = null;
            jugador = null;
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

    public void NotificarJefeDerrotado()
{
    if (jefeDerrotado)
        return;

    Debug.Log("SalaJefe detectó la muerte del jefe.");

    CompletarSala();
}
}
