
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

    [Header("Detección")]
    [SerializeField] private string tagJugador = "Player";

    [Header("Efectos")]
    [SerializeField] private GameObject efectoRecogida;
    [SerializeField] private float tiempoDesaparicion = 1f;

    private Collider2D col;
    private SpriteRenderer spriteRenderer;

    private bool recogida = false;

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

        gameObject.SetActive(true);

        if (col != null)
            col.enabled = true;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
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

        // Comprobar que el objeto esté configurado.
        if (itemPiedra == null)
        {
            Debug.LogError(
                "No se asignó el ItemData de la Piedra Mágica."
            );
            return;
        }

        // Comprobar que exista el inventario.
        if (InventoryManager.instancia == null)
        {
            Debug.LogWarning(
                "No se encontró InventoryManager."
            );
            return;
        }

        // Intentar agregar la piedra al inventario.
        bool agregado =
            InventoryManager.instancia.AñadirItem(itemPiedra, 1);

        // Si no hay espacio, no recoger la piedra.
        if (!agregado)
        {
            Debug.Log("Inventario lleno. No se pudo recoger la piedra.");

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

        // Registrar que la piedra fue obtenida.
        ProgresoMundo.Registrar(idProgresoPiedra);

        // Desactivar colisión.
        if (col != null)
            col.enabled = false;

        // Ocultar la imagen de la piedra.
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        // Mostrar efecto de recolección.
        if (efectoRecogida != null)
            efectoRecogida.SetActive(true);

        // Activar el portal.
        if (portalSiguienteMapa != null)
        {
            portalSiguienteMapa.SetActive(true);

            Debug.Log("¡Piedra Mágica obtenida! Portal desbloqueado.");
        }
        else
        {
            Debug.LogWarning(
                "No se asignó el portal a PiedraMagica."
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
