
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class PiedraMagica : MonoBehaviour
{
    [Header("Identificador de progreso")]
    [SerializeField] private string idProgresoPiedra = "mapa1_piedra";

    [Header("Objeto del inventario")]
    [SerializeField] private ItemData itemPiedra;

    [Header("Portal")]
    [SerializeField] private GameObject portalSiguienteMapa;

    [SerializeField] private bool activarPortalAlRecoger = true;

    [Header("Detección")]
    [SerializeField] private string tagJugador = "Player";

    [Header("Efectos")]
    [SerializeField] private GameObject efectoRecogida;
    [SerializeField] private float tiempoDesaparicion = 1f;

    private Collider2D col;
    private SpriteRenderer spriteRenderer;

    private bool recogida = false;

    public bool DebeActivarPortal => activarPortalAlRecoger;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (portalSiguienteMapa != null)
            portalSiguienteMapa.SetActive(false);

        if (efectoRecogida != null)
            efectoRecogida.SetActive(false);
    }

    public bool EstaRecogida()
    {
        return ProgresoMundo.EstaCompletado(idProgresoPiedra);
    }

    // Aparecer en el lugar donde murió el jefe.
    public void AparecerEn(Vector3 posicion)
    {
        transform.position = posicion;

        recogida = false;

        if (efectoRecogida != null)
            efectoRecogida.SetActive(false);

        gameObject.SetActive(true);

        if (col != null)
            col.enabled = true;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (portalSiguienteMapa != null)
            portalSiguienteMapa.SetActive(false);
    }

    // Restaurar portal después de cargar una partida.
    public void RestaurarPortal()
    {
        if (portalSiguienteMapa != null)
        {
            portalSiguienteMapa.SetActive(
                activarPortalAlRecoger
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (recogida)
            return;

        if (!EsJugador(other))
            return;

        RecogerPiedra();
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

    void RecogerPiedra()
    {
        if (recogida)
            return;

        if (itemPiedra == null)
        {
            Debug.LogError(
                "No se asignó el ItemData de la Piedra Mágica."
            );
            return;
        }

        if (InventoryManager.instancia == null)
        {
            Debug.LogWarning(
                "No se encontró InventoryManager."
            );
            return;
        }

        bool agregado =
            InventoryManager.instancia.AñadirItem(itemPiedra, 1);

        if (!agregado)
        {
            Debug.Log(
                "Inventario lleno. No se pudo recoger la piedra."
            );

            InventoryNavigation navegacion =
                FindFirstObjectByType<InventoryNavigation>();

            if (navegacion != null)
            {
                navegacion.MostrarMensajeAlerta(
                    "¡Inventario lleno!\nLibera una casilla para recoger la Piedra Mágica."
                );
            }

            return;
        }

        recogida = true;

        ProgresoMundo.Registrar(idProgresoPiedra);

        if (col != null)
            col.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (efectoRecogida != null)
            efectoRecogida.SetActive(true);

        // Solo activar portal si está habilitada la opción.
        if (activarPortalAlRecoger)
        {
            if (portalSiguienteMapa != null)
            {
                portalSiguienteMapa.SetActive(true);

                Debug.Log(
                    "¡Piedra Mágica obtenida! Portal desbloqueado."
                );
            }
            else
            {
                Debug.LogWarning(
                    "La piedra está configurada para activar un portal, pero no tiene uno asignado."
                );
            }
        }
        else
        {
            Debug.Log(
                "¡Piedra Mágica final obtenida! No se necesita otro portal."
            );
        }

        StartCoroutine(Desaparecer());
    }

    IEnumerator Desaparecer()
    {
        yield return new WaitForSecondsRealtime(
            tiempoDesaparicion
        );

        gameObject.SetActive(false);
    }
}
