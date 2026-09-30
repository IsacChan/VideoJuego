using UnityEngine;
using System.Collections;

public class enemigo : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 10f;
    public float speed = 5f;

    [Header("Ataque")]
    public float attackCooldown = 5f;
    public float attackDuration = 1f;

    private float attackTimer;
    private float attackAnimationTimer;

    private bool isAttacking = false;

    private Animator animator;
    private Rigidbody2D rb;

    private Vector2 movement;

    public int vida = 3;

    [Header("Daño")]
public float tiempoParpadeo = 0.1f;

private SpriteRenderer spriteRenderer;

    void Start()
{
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
    spriteRenderer = GetComponent<SpriteRenderer>();

    attackTimer = attackCooldown;
}

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(
            transform.position,
            player.position
        );

        // ==========================================
        // SI ESTÁ ATACANDO
        // ==========================================
        if (isAttacking)
        {
            movement = Vector2.zero;

            // El enemigo no camina
            animator.SetFloat("Speed", 0f);

            // Contamos cuánto lleva atacando
            attackAnimationTimer -= Time.deltaTime;

            if (attackAnimationTimer <= 0f)
            {
                isAttacking = false;
                attackTimer = attackCooldown;
            }

            return;
        }

        // ==========================================
        // REDUCIR COOLDOWN
        // ==========================================
        attackTimer -= Time.deltaTime;

        // ==========================================
        // JUGADOR DENTRO DEL RANGO
        // ==========================================
        if (distanceToPlayer <= detectionRadius)
        {
            Vector2 direction = (
                player.position - transform.position
            ).normalized;

            movement = direction;

            // Guardamos la dirección
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);

            // ==========================================
            // ATACAR
            // ==========================================
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

        // ==========================================
        // MOVIMIENTO
        // ==========================================
        rb.MovePosition(
            rb.position + movement * speed * Time.deltaTime
        );

        animator.SetFloat("Speed", movement.magnitude);
    }

    void Atacar()
    {
        isAttacking = true;

        // Detener movimiento
        movement = Vector2.zero;

        animator.SetFloat("Speed", 0f);

        // Ejecutar animación
        animator.SetTrigger("Attack");

        // Duración del ataque
        attackAnimationTimer = attackDuration;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRadius
        );
    }

    public void RecibirDanio(int damage)
{
    vida -= damage;

    Debug.Log("El enemigo recibió " + damage + " de daño");

    StartCoroutine(Parpadear());

    if (vida <= 0)
    {
        Morir();
    }
}

private void Morir()
{
    Destroy(gameObject);
}

private IEnumerator Parpadear()
{
    spriteRenderer.enabled = false;

    yield return new WaitForSeconds(tiempoParpadeo);

    spriteRenderer.enabled = true;
}
}
