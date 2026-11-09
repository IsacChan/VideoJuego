
using UnityEngine;
using System.Collections;

public class AtaqueMartillo : MonoBehaviour
{
    [Header("Hitboxes por dirección")]
    public Collider2D hitboxArriba;
    public Collider2D hitboxAbajo;
    public Collider2D hitboxIzquierda;
    public Collider2D hitboxDerecha;

    [Header("Tiempo del golpe")]
    public float retrasoGolpe = 0.3f;
    public float duracionGolpe = 0.2f;

    private Coroutine ataqueActual;
    private Collider2D hitboxActiva;

    private bool golpeActivo;
    private bool jugadorGolpeado;

    void Awake()
    {
        DesactivarHitboxes();
    }

    public void EjecutarAtaque(Vector2 direccion)
{
    if (ataqueActual != null)
        return;

    SeleccionarHitbox(direccion);

    Debug.Log("Dirección recibida: " + direccion);

    if (hitboxActiva == null)
    {
        Debug.LogWarning(
            "No hay hitbox configurada para esta dirección."
        );
        return;
    }

    Debug.Log(
        "Hitbox seleccionada: " + hitboxActiva.name
    );

    ataqueActual = StartCoroutine(Golpear());
}

    void SeleccionarHitbox(Vector2 direccion)
    {
        if (Mathf.Abs(direccion.x) > Mathf.Abs(direccion.y))
        {
            hitboxActiva = direccion.x > 0
                ? hitboxDerecha
                : hitboxIzquierda;
        }
        else
        {
            hitboxActiva = direccion.y > 0
                ? hitboxArriba
                : hitboxAbajo;
        }
    }

    IEnumerator Golpear()
    {
        jugadorGolpeado = false;
        golpeActivo = false;

        yield return new WaitForSeconds(retrasoGolpe);

        golpeActivo = true;
        hitboxActiva.enabled = true;

        // Detectar al jugador aunque ya esté
        // dentro de la zona de impacto.
        float tiempo = 0f;

while (tiempo < duracionGolpe)
{
    DetectarImpacto();

    tiempo += Time.deltaTime;

    yield return null;
}
        DesactivarHitboxes();

        ataqueActual = null;
    }

    void DetectarImpacto()
{
    if (hitboxActiva == null || jugadorGolpeado)
        return;

    ContactFilter2D filtro = new ContactFilter2D();
    filtro.useTriggers = true;

    Collider2D[] resultados = new Collider2D[20];

    int cantidad = hitboxActiva.Overlap(
        filtro,
        resultados
    );

    for (int i = 0; i < cantidad; i++)
    {
        IntentarGolpear(resultados[i]);

        if (jugadorGolpeado)
            break;
    }
}

    void OnTriggerEnter2D(Collider2D other)
    {
        IntentarGolpear(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        IntentarGolpear(other);
    }

    void IntentarGolpear(Collider2D other)
    {
        if (!golpeActivo || jugadorGolpeado)
            return;

        caminar jugador =
            other.GetComponentInParent<caminar>();

        if (jugador == null)
            return;

        jugadorGolpeado = true;

        jugador.RecibirDanioRaices(transform);

        Debug.Log("El martillo golpeó al jugador.");
    }

    void DesactivarHitboxes()
    {
        golpeActivo = false;

        if (hitboxArriba != null)
            hitboxArriba.enabled = false;

        if (hitboxAbajo != null)
            hitboxAbajo.enabled = false;

        if (hitboxIzquierda != null)
            hitboxIzquierda.enabled = false;

        if (hitboxDerecha != null)
            hitboxDerecha.enabled = false;
    }

    public void CancelarAtaque()
    {
        if (ataqueActual != null)
        {
            StopCoroutine(ataqueActual);
            ataqueActual = null;
        }

        DesactivarHitboxes();
    }

    void OnDisable()
    {
        CancelarAtaque();
    }
}
