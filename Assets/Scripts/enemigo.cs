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

    [Header("Distancia de ataque")]
public float attackRange = 1.5f;

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

    [Header("Ataque de Martillo")]
public AtaqueMartillo ataqueMartillo;

[Header("Dirección del ataque")]
private Vector2 direccionAtaque = Vector2.down;

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

        if (ataqueMartillo == null)
{
    ataqueMartillo = GetComponent<AtaqueMartillo>();
}

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

    rb.linearVelocity = Vector2.zero;

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

if (ataqueMartillo != null)
{
    ActualizarDireccionAtaque(direction);
}

            // =========================
            // INICIAR ATAQUE
            // =========================

            if (attackTimer <= 0f &&
    distanceToPlayer <= attackRange)
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

    if (player == null)
        return;

    isAttacking = true;
    movement = Vector2.zero;

    rb.linearVelocity = Vector2.zero;

    // Calcular nuevamente la dirección del jugador
    Vector2 direccion = (
        player.position - transform.position
    ).normalized;

    // Actualizar dirección del Blend Tree
    if (ataqueMartillo != null)
    {
        ActualizarDireccionAtaque(direccion);
    }

    // Detener animación de movimiento
    animator.SetFloat("Speed", 0f);

    // Ejecutar animación
    animator.SetTrigger("Attack");

    // Ejecutar ataque de raíces del enemigo 1
    if (ataqueCircular != null)
    {
        ataqueCircular.EjecutarAtaque();
    }

    // Ejecutar ataque de martillo del enemigo 2
    if (ataqueMartillo != null)
    {
        ataqueMartillo.EjecutarAtaque(direccionAtaque);

        Debug.Log(
            "Enemigo 2 ejecutando ataque de martillo: "
            + direccionAtaque
        );
    }
    else
    {
        Debug.LogWarning(
            "No se encontró AtaqueMartillo en el enemigo."
        );
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

    if (ataqueMartillo != null)
{
    ataqueMartillo.CancelarAtaque();
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

    void ActualizarDireccionAtaque(Vector2 direccion)
{
    if (Mathf.Abs(direccion.x) > Mathf.Abs(direccion.y))
    {
        direccionAtaque = new Vector2(
            direccion.x > 0 ? 1 : -1,
            0
        );
    }
    else
    {
        direccionAtaque = new Vector2(
            0,
            direccion.y > 0 ? 1 : -1
        );
    }

    animator.SetFloat("AttackX", direccionAtaque.x);
    animator.SetFloat("AttackY", direccionAtaque.y);
}
}