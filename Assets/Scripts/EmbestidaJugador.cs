using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class EmbestidaJugador : MonoBehaviour
{
    [Header("Detección")]
    public float radioDeteccion = 6f;
    public LayerMask capaEnemigos;

    [Header("Embestida")]
    public float velocidadEmbestida = 15f;
    public float distanciaImpacto = 0.8f;
    public float duracionMaxima = 0.5f;

    [Header("Daño")]
    public int danioEmbestida = 3;

    [Header("Cooldown")]
    public float tiempoRecarga = 5f;

    private float siguienteUso = 0f;

    private Rigidbody2D rb;
    private caminar jugador;

    private bool embistiendo = false;

    public bool EstaEmbistiendo => embistiendo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jugador = GetComponent<caminar>();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            IntentarEmbestida();
        }
    }

    private void IntentarEmbestida()
    {
        if (embistiendo || Time.time < siguienteUso)
            return;

        if (jugador == null || !jugador.PuedeUsarHabilidades())
            return;

        Collider2D[] enemigos = Physics2D.OverlapCircleAll(
            transform.position,
            radioDeteccion,
            capaEnemigos
        );

        Collider2D enemigoCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (Collider2D enemigo in enemigos)
        {
            if (enemigo.transform.root == transform.root)
                continue;

            float distancia = Vector2.Distance(
                transform.position,
                enemigo.transform.position
            );

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                enemigoCercano = enemigo;
            }
        }

        if (enemigoCercano == null)
        {
            Debug.Log("No hay enemigos cerca");
            return;
        }

        siguienteUso = Time.time + tiempoRecarga;

        StartCoroutine(RealizarEmbestida(enemigoCercano));
    }

    private IEnumerator RealizarEmbestida(Collider2D objetivo)
    {
        embistiendo = true;

        rb.linearVelocity = Vector2.zero;

        float tiempo = 0f;

        while (tiempo < duracionMaxima)
        {
            if (objetivo == null || !jugador.PuedeContinuarEmbestida())
                break;

            Vector2 direccion = (
                (Vector2)objetivo.transform.position -
                rb.position
            ).normalized;

            float distancia = Vector2.Distance(
                rb.position,
                objetivo.transform.position
            );

            if (distancia <= distanciaImpacto)
            {
                GolpearEnemigo(objetivo);
                break;
            }

            rb.linearVelocity = direccion * velocidadEmbestida;

            yield return new WaitForFixedUpdate();

            tiempo += Time.fixedDeltaTime;
        }

        rb.linearVelocity = Vector2.zero;

        embistiendo = false;
    }

    private void GolpearEnemigo(Collider2D objetivo)
    {
        // Se obtiene el componente de vida del enemigo
        enemigo enemigoScript =
            objetivo.GetComponentInParent<enemigo>();

        if (enemigoScript != null)
        {
            enemigoScript.RecibirDanio(danioEmbestida);

            Debug.Log("¡Embestida exitosa!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            radioDeteccion
        );
    }
}