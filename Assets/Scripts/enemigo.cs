
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

    [Header("Daño")]
    public float tiempoParpadeo = 0.1f;

    private bool muerto = false;

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
    }

    // =========================
    // UPDATE
    // =========================

    void Update()
    {
        // Si está muerto, no hacer nada
        if (muerto)
        {
            movement = Vector2.zero;
            return;
        }

        if (player == null)
        {
            movement = Vector2.zero;
            animator.SetFloat("Speed", 0f);
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

        // Detener movimiento y ataque
        movement = Vector2.zero;
        isAttacking = false;

        // Detener coroutines de parpadeo
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

        // Reproducir animación de derrota
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Death");

        Debug.Log("El enemigo ha sido derrotado");
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
