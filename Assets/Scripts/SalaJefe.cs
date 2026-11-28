
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SalaJefe : MonoBehaviour
{
    [Header("Puerta del jefe")]
    public GameObject puerta;

    [Header("Jefe")]
    public GameObject jefe;

    [Header("Recompensa del jefe")]
    public GameObject piedraMagica;

    [Header("Portal al siguiente mapa")]
    public GameObject portalSiguienteMapa;

    [Header("Llaves")]
    public int llavesNecesarias = 3;

    [Header("Detección")]
    public float margenEntrada = 0.5f;
    public string tagJugador = "Player";

    private BoxCollider2D zona;
    private Collider2D colliderJugador;
    private Transform jugador;

    private bool combateIniciado = false;
    private bool jefeDerrotado = false;

    private InventarioJugador inventario;

    [Header("Sistema de guardado")]
    [SerializeField] private string idSalaJefe = "mapa1_jefe";

    void Awake()
    {
        zona = GetComponent<BoxCollider2D>();
        zona.isTrigger = true;
    }

    void Start()
    {
        inventario = FindFirstObjectByType<InventarioJugador>();

        if (inventario == null)
        {
            Debug.LogError(
                "No se encontró InventarioJugador en este mapa."
            );
        }

        bool jefeYaDerrotado =
            ProgresoMundo.EstaCompletado(idSalaJefe);

        if (jefeYaDerrotado)
        {
            jefeDerrotado = true;
            combateIniciado = false;

            CambiarPuerta(false);

            if (jefe != null)
                jefe.SetActive(false);

            RestaurarRecompensa();

            Debug.Log(
                "Jefe derrotado restaurado. Comprobando recompensa."
            );

            return;
        }

        jefeDerrotado = false;
        combateIniciado = false;

        CambiarPuerta(true);

        if (piedraMagica != null)
            piedraMagica.SetActive(false);

        if (portalSiguienteMapa != null)
            portalSiguienteMapa.SetActive(false);

        Debug.Log("Sala del jefe preparada.");
    }

    void RestaurarRecompensa()
    {
        if (piedraMagica == null)
        {
            Debug.LogWarning(
                "No se asignó la piedra mágica en SalaJefe."
            );
            return;
        }

        PiedraMagica piedra =
            piedraMagica.GetComponent<PiedraMagica>();

        if (piedra == null)
        {
            Debug.LogError(
                "El objeto piedra no tiene el script PiedraMagica."
            );
            return;
        }

        if (piedra.EstaRecogida())
        {
            piedraMagica.SetActive(false);

            // La piedra decide si debe restaurar el portal.
            piedra.RestaurarPortal();

            Debug.Log(
                "Piedra recogida. Recompensa restaurada."
            );
        }
        else
        {
            if (portalSiguienteMapa != null)
                portalSiguienteMapa.SetActive(false);

            piedraMagica.SetActive(true);

            Debug.Log(
                "La piedra mágica está disponible."
            );
        }
    }

    void Update()
    {
        if (jefeDerrotado)
            return;

        VerificarJefe();

        if (jefeDerrotado)
            return;

        if (combateIniciado)
        {
            if (!JugadorEstaEnSala())
                ReiniciarCombate();

            return;
        }

        VerificarLlaves();
    }

    void VerificarLlaves()
    {
        if (inventario == null)
        {
            CambiarPuerta(true);
            return;
        }

        int llaves = inventario.llavesActuales;

        if (llaves < llavesNecesarias)
        {
            CambiarPuerta(true);
            return;
        }

        CambiarPuerta(false);

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

        CambiarPuerta(true);

        Debug.Log(
            "Combate contra el jefe iniciado. Puerta cerrada."
        );
    }

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

    void ReiniciarCombate()
    {
        combateIniciado = false;
        colliderJugador = null;
        jugador = null;

        CambiarPuerta(!TieneLlavesNecesarias());

        Debug.Log(
            "Jugador fuera de la sala del jefe. Combate reiniciado."
        );
    }

    bool TieneLlavesNecesarias()
    {
        if (inventario == null)
            return false;

        return inventario.llavesActuales >= llavesNecesarias;
    }

    void VerificarJefe()
    {
        if (jefeDerrotado)
            return;

        if (jefe == null)
        {
            Debug.LogWarning(
                "La referencia del jefe no está asignada."
            );

            return;
        }

        // La muerte se notifica mediante
        // NotificarJefeDerrotado().
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

        if (portalSiguienteMapa != null)
            portalSiguienteMapa.SetActive(false);

        // Registrar jefe derrotado.
        ProgresoMundo.Registrar(idSalaJefe);

        // Desplegar piedra en el lugar de la muerte.
        if (piedraMagica != null)
        {
            DesplegarPiedra(
                jefe != null ? jefe.transform.position : transform.position
            );
        }
        else
        {
            Debug.LogError(
                "No se asignó la piedra mágica en SalaJefe."
            );
        }

        Debug.Log(
            "JEFE DERROTADO - PUERTA ABIERTA - RECOMPENSA DESPLEGADA"
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
                ReiniciarCombate();
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

        Debug.Log(
            "SalaJefe detectó la muerte del jefe."
        );

        CompletarSala();
    }

    public void DesplegarPiedra(Vector3 posicionMuerte)
    {
        if (piedraMagica == null)
        {
            Debug.LogWarning(
                "No se asignó la piedra mágica."
            );
            return;
        }

        PiedraMagica piedra =
            piedraMagica.GetComponent<PiedraMagica>();

        if (piedra != null)
        {
            piedra.AparecerEn(posicionMuerte);

            Debug.Log(
                "Piedra mágica desplegada en el lugar de la muerte del jefe."
            );
        }
        else
        {
            Debug.LogError(
                "El objeto asignado no tiene el componente PiedraMagica."
            );
        }
    }

    // Desplegar una recompensa independiente,
    // por ejemplo, la piedra del jefe final.
    public void DesplegarPiedraFinal(
        GameObject piedraFinal,
        Vector3 posicionMuerte)
    {
        if (piedraFinal == null)
        {
            Debug.LogError(
                "No se asignó la piedra del jefe final."
            );
            return;
        }

        PiedraMagica piedra =
            piedraFinal.GetComponent<PiedraMagica>();

        if (piedra == null)
        {
            Debug.LogError(
                "La piedra final no tiene el componente PiedraMagica."
            );
            return;
        }

        piedra.AparecerEn(posicionMuerte);

        Debug.Log(
            "¡Piedra del JEFE FINAL desplegada correctamente!"
        );
    }

}
