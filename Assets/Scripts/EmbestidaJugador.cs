
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EmbestidaJugador : MonoBehaviour
{
    // =========================
    // DETECCIÓN DE ENEMIGOS
    // =========================

    [Header("Detección")]
    public float radioDeteccion = 6f;
    public LayerMask capaEnemigos;

    // =========================
    // EMBESTIDA
    // =========================

    [Header("Embestida")]
    public float velocidadEmbestida = 15f;
    public float distanciaImpacto = 0.8f;
    public float duracionMaxima = 0.5f;

    // =========================
    // DAÑO
    // =========================

    [Header("Daño")]
    public int danioEmbestida = 3;

    // =========================
    // COOLDOWN
    // =========================

    [Header("Cooldown")]
    public float tiempoRecarga = 5f;

    private float siguienteUso = 0f;

    private Rigidbody2D rb;
    private caminar jugador;

    private bool embistiendo = false;

    public bool EstaEmbistiendo => embistiendo;

    // =========================
    // INICIALIZACIÓN
    // =========================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jugador = GetComponent<caminar>();
    }

    // =========================
    // ACTIVAR CON ENTER
    // =========================

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            IntentarEmbestida();
        }
    }

    // =========================
    // BUSCAR ENEMIGO MÁS CERCANO
    // =========================

    private void IntentarEmbestida()
    {
        if (embistiendo || Time.time < siguienteUso)
            return;

        if (jugador == null || !jugador.PuedeUsarHabilidades())
            return;

        // Buscar únicamente en la Layer seleccionada
        Collider2D[] enemigos = Physics2D.OverlapCircleAll(
            transform.position,
            radioDeteccion,
            capaEnemigos
        );

        Debug.Log(
            "Colliders encontrados en Enemy: " +
            enemigos.Length
        );

        Collider2D enemigoCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (Collider2D candidato in enemigos)
        {
            if (candidato == null)
                continue;

            // No seleccionar al jugador
            if (candidato.transform.root == transform.root)
                continue;

            // Ignorar colliders deshabilitados
            if (!candidato.enabled ||
                !candidato.gameObject.activeInHierarchy)
                continue;

            float distancia = Vector2.Distance(
                rb.position,
                candidato.ClosestPoint(rb.position)
            );

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                enemigoCercano = candidato;
            }
        }

        if (enemigoCercano == null)
        {
            Debug.LogWarning(
                "No hay enemigos dentro del radio de detección"
            );

            return;
        }

        Debug.Log(
            "Enemigo seleccionado: " +
            enemigoCercano.name
        );

        siguienteUso = Time.time + tiempoRecarga;

        StartCoroutine(
            RealizarEmbestida(enemigoCercano)
        );
    }

    // =========================
    // REALIZAR EMBESTIDA
    // =========================

    private IEnumerator RealizarEmbestida(Collider2D objetivo)
    {
        embistiendo = true;

        rb.linearVelocity = Vector2.zero;

        float tiempo = 0f;

        while (tiempo < duracionMaxima)
        {
            if (objetivo == null ||
                !objetivo.enabled ||
                !objetivo.gameObject.activeInHierarchy ||
                !jugador.PuedeContinuarEmbestida())
            {
                break;
            }

            Vector2 posicionObjetivo =
                objetivo.ClosestPoint(rb.position);

            Vector2 direccion =
                posicionObjetivo - rb.position;

            float distancia = direccion.magnitude;

            // Verificar si alcanzó al enemigo
            if (distancia <= distanciaImpacto)
            {
                GolpearEnemigo(objetivo);
                break;
            }

            // Mover al jugador hacia el enemigo
            rb.linearVelocity =
                direccion.normalized * velocidadEmbestida;

            yield return new WaitForFixedUpdate();

            tiempo += Time.fixedDeltaTime;
        }

        // Detener movimiento de embestida
        rb.linearVelocity = Vector2.zero;

        embistiendo = false;
    }

    // =========================
    // CAUSAR DAÑO
    // =========================

    private void GolpearEnemigo(Collider2D objetivo)
    {
        if (objetivo == null)
            return;

        // Buscar componentes de daño en el enemigo
        MonoBehaviour[] componentes =
            objetivo.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour componente in componentes)
        {
            if (componente is IRecibirDanio receptor)
            {
                receptor.RecibirDanio(danioEmbestida);

                Debug.Log(
                    "¡Embestida exitosa contra " +
                    objetivo.name +
                    "! Daño: " +
                    danioEmbestida
                );

                return;
            }
        }

        Debug.LogWarning(
            "Se detectó " +
            objetivo.name +
            " pero no implementa IRecibirDanio."
        );
    }

    // =========================
    // MOSTRAR RADIO DE DETECCIÓN
    // =========================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            radioDeteccion
        );
    }
}
