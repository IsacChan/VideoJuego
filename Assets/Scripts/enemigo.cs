using UnityEngine;
using System.Collections;

public class enemigo : MonoBehaviour
{
    // =========================
    // MOVIMIENTO
    // =========================

    [Header("Movimiento")]
    public Transform player;
    public float detectionRadius = 10f;
    public float speed = 5f;

    // =========================
    // ATAQUE
    // =========================

    [Header("Ataque")]
    public float attackCooldown = 5f;
    public float attackDuration = 1f;

    private float attackTimer;
    private float attackAnimationTimer;

    private bool isAttacking = false;

    // =========================
    // ATAQUE DE RAÍCES
    // =========================

    private AtaqueRaicesCircular ataqueCircular;

    // =========================
    // VIDA Y DAÑO
    // =========================

    [Header("Vida")]
    public int vida = 3;

    [Header("UI Barra de Vida")]
    [SerializeField] private ControladorBarraJefe barraVidaJefe; // <--- Referencia a la barra de vida

    [Header("Daño")]
    public float tiempoParpadeo = 0.1f;

    private bool muerto = false;

    [Header("Sala del jefe")]
public SalaJefe salaJefe;

    // =========================
    // COMPONENTES
    // =========================

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private Vector2 movement;

    // =========================
    // INICIO
    // =========================

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        ataqueCircular = GetComponent<AtaqueRaicesCircular>();

        attackTimer = attackCooldown;

        // Inicializar la barra de vida del jefe y ocultarla al empezar
        if (barraVidaJefe != null)
        {
            barraVidaJefe.InicializarVida(vida);
            barraVidaJefe.gameObject.SetActive(false); // La barra comienza apagada
        }
    }

    // =========================
    // UPDATE
    // =========================

    void Update()
    {
        // Si está muerto, asegurarnos de apagar la barra y no hacer nada más
        if (muerto)
        {
            movement = Vector2.zero;
            if (barraVidaJefe != null)
            {
                barraVidaJefe.gameObject.SetActive(false);
            }
            return;
        }

        if (player == null)
        {
            movement = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            if (barraVidaJefe != null)
            {
                barraVidaJefe.gameObject.SetActive(false);
            }
            return;
        }

        // =========================
        // SI ESTÁ ATACANDO
        // =========================

        if (isAttacking)
        {
            movement = Vector2.zero;

            animator.SetFloat("Speed", 0f);

            attackAnimationTimer -= Time.deltaTime;

            if (attackAnimationTimer <= 0f)
            {
                isAttacking = false;
                attackTimer = attackCooldown;
            }

            return;
        }

        // =========================
        // COOLDOWN
        // =========================

        attackTimer -= Time.deltaTime;

        // =========================
        // DETECTAR JUGADOR
        // =========================

        float distanceToPlayer = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distanceToPlayer <= detectionRadius)
        {
            // El jugador está cerca: activamos la barra de vida
            if (barraVidaJefe != null)
            {
                barraVidaJefe.gameObject.SetActive(true);
            }

            Vector2 direction = (
                player.position - transform.position
            ).normalized;

            movement = direction;

            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);

            // =========================
            // INICIAR ATAQUE
            // =========================

            if (attackTimer <= 0f)
            {
                Atacar();
                return;
            }
        }
        else
        {
            // El jugador se aleja: ocultamos la barra de vida
            if (barraVidaJefe != null)
            {
                barraVidaJefe.gameObject.SetActive(false);
            }

            movement = Vector2.zero;
        }

        // =========================
        // MOVIMIENTO
        // =========================

        rb.MovePosition(
            rb.position + movement * speed * Time.deltaTime
        );

        animator.SetFloat("Speed", movement.magnitude);
    }

    // =========================
    // ATACAR
    // =========================

    void Atacar()
    {
        if (muerto || isAttacking)
            return;

        isAttacking = true;
        movement = Vector2.zero;

        animator.SetFloat("Speed", 0f);

        // Animación de ataque del enemigo
        animator.SetTrigger("Attack");

        // Generar raíces alrededor
        if (ataqueCircular != null)
        {
            ataqueCircular.EjecutarAtaque();
        }

        attackAnimationTimer = attackDuration;
    }

    // =========================
    // RECIBIR DAÑO
    // =========================

    public void RecibirDanio(int damage)
    {
        if (muerto)
            return;

        vida -= damage;

        Debug.Log(
            "El enemigo recibió " + damage + " de daño"
        );

        // Actualizar la barra de vida en pantalla
        if (barraVidaJefe != null)
        {
            barraVidaJefe.RecibirDaño(vida);
        }

        // Comprobar muerte
        if (vida <= 0)
        {
            vida = 0;
            Morir();
            return;
        }

        // Parpadear al recibir daño
        StartCoroutine(Parpadear());
    }

    // =========================
    // PARPADEO
    // =========================

    private IEnumerator Parpadear()
    {
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(tiempoParpadeo);

        spriteRenderer.enabled = true;
    }

    // =========================
    // MUERTE
    // =========================

    private void Morir()
{
    if (muerto)
        return;

    muerto = true;

    // Ocultar barra de vida
    if (barraVidaJefe != null)
    {
        barraVidaJefe.gameObject.SetActive(false);
    }

    // Detener movimiento y ataque
    movement = Vector2.zero;
    isAttacking = false;

    // Detener corrutinas
    StopAllCoroutines();

    // Mantener sprite visible
    spriteRenderer.enabled = true;

    // Detener Rigidbody
    rb.linearVelocity = Vector2.zero;

    // Detener animación de caminar
    animator.SetFloat("Speed", 0f);

    // Desactivar colisión
    if (col != null)
    {
        col.enabled = false;
    }

    // Cancelar raíces activas
    if (ataqueCircular != null)
    {
        ataqueCircular.CancelarAtaque();
    }

    // Reproducir animación de muerte
    animator.ResetTrigger("Attack");
    animator.SetTrigger("Death");

    Debug.Log("El enemigo ha sido derrotado");

    // Notificar a la sala del jefe
    if (salaJefe != null)
    {
        Debug.Log("Notificando muerte a SalaJefe...");

        salaJefe.NotificarJefeDerrotado();
    }
    else
    {
        Debug.LogWarning(
            "El enemigo murió, pero NO tiene asignada una SalaJefe."
        );
    }

    // No destruir el enemigo.
    // Su cuerpo permanece en el mapa.
}

    // =========================
    // VISUALIZAR RANGO
    // =========================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRadius
        );
    }
}