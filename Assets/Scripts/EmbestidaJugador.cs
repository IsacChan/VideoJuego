
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

    // =========================
    // DATOS PARA EL ORBE
    // =========================

    public float TiempoRestanteRecarga
    {
        get
        {
            return Mathf.Max(0f, siguienteUso - Time.time);
        }
    }

    public float ProgresoRecarga
    {
        get
        {
            if (tiempoRecarga <= 0f)
                return 1f;

            return 1f - Mathf.Clamp01(
                TiempoRestanteRecarga / tiempoRecarga
            );
        }
    }

    // =========================
    // COMPONENTES
    // =========================

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
    // UTILIZANDO TAG ENEMY
    // =========================

    private void IntentarEmbestida()
    {
        if (embistiendo || Time.time < siguienteUso)
            return;

        if (jugador == null ||
            !jugador.PuedeUsarHabilidades())
            return;

        // Buscar objetos con Tag Enemy
        GameObject[] enemigos =
            GameObject.FindGameObjectsWithTag("Enemy");

        Debug.Log(
            "Enemigos encontrados con Tag Enemy: " +
            enemigos.Length
        );

        Collider2D enemigoCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (GameObject enemigo in enemigos)
        {
            if (enemigo == null ||
                enemigo.transform.root == transform.root)
                continue;

            // Buscar colliders en el enemigo y sus hijos
            Collider2D[] colliders =
                enemigo.GetComponentsInChildren<Collider2D>();

            foreach (Collider2D candidato in colliders)
            {
                if (candidato == null ||
                    !candidato.enabled ||
                    !candidato.gameObject.activeInHierarchy)
                    continue;

                // Ignorar hitboxes de ataque
                if (candidato.isTrigger)
                    continue;

                float distancia = Vector2.Distance(
                    rb.position,
                    candidato.ClosestPoint(rb.position)
                );

                if (distancia > radioDeteccion)
                    continue;

                if (distancia < distanciaMinima)
                {
                    distanciaMinima = distancia;
                    enemigoCercano = candidato;
                }
            }
        }

        if (enemigoCercano == null)
        {
            Debug.LogWarning(
                "No hay enemigos válidos dentro del radio"
            );
            return;
        }

        Debug.Log(
            "Enemigo seleccionado: " +
            enemigoCercano.name
        );

        // Iniciar cooldown
        siguienteUso = Time.time + tiempoRecarga;

        // Ejecutar embestida
        StartCoroutine(
            RealizarEmbestida(enemigoCercano)
        );
    }

    // =========================
    // REALIZAR EMBESTIDA
    // =========================

    private IEnumerator RealizarEmbestida(
        Collider2D objetivo
    )
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

            // Impactar al enemigo
            if (distancia <= distanciaImpacto)
            {
                GolpearEnemigo(objetivo);
                break;
            }

            // Desplazarse hacia el enemigo
            rb.linearVelocity =
                direccion.normalized * velocidadEmbestida;

            yield return new WaitForFixedUpdate();

            tiempo += Time.fixedDeltaTime;
        }

        // Detener embestida
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

    // 1. Buscar enemigos que implementen IRecibirDanio
    MonoBehaviour[] componentes =
        objetivo.GetComponentsInParent<MonoBehaviour>();

    foreach (MonoBehaviour componente in componentes)
    {
        if (componente is IRecibirDanio receptor)
        {
            receptor.RecibirDanio(danioEmbestida);

            Debug.Log(
                "Embestida exitosa mediante IRecibirDanio: " +
                objetivo.name
            );

            return;
        }
    }

    // 2. Buscar enemigos que utilizan enemigo.cs
    enemigo enemigoNormal =
        objetivo.GetComponentInParent<enemigo>();

    if (enemigoNormal != null)
    {
        enemigoNormal.RecibirDanio(danioEmbestida);

        Debug.Log(
            "Embestida exitosa contra " +
            objetivo.name +
            " usando enemigo.cs. Daño: " +
            danioEmbestida
        );

        return;
    }

    // 3. Buscar enemigos que utilizan SaludJefe.cs
    SaludJefe saludJefe =
        objetivo.GetComponentInParent<SaludJefe>();

    if (saludJefe != null)
    {
        saludJefe.RecibirDanio(danioEmbestida);

        Debug.Log(
            "Embestida exitosa contra jefe: " +
            objetivo.name +
            ". Daño: " +
            danioEmbestida
        );

        return;
    }

    Debug.LogWarning(
        "No se encontró un sistema de vida compatible en: " +
        objetivo.name
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
