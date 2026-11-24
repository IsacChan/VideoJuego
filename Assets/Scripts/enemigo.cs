using UnityEngine;
using System.Collections;

public class enemigo : MonoBehaviour
{

    public enum TipoAtaque
{
    Raices,
    Laser
}

[Header("Tipo de ataque")]
public TipoAtaque tipoAtaque;

[Header("Rango de ataque láser")]
public float rangoAtaqueLaser = 8f;


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


    [Header("Ataque Láser")]
public GameObject prefabLaser;
public Transform puntoDisparoLaser;

public float tiempoEntreLaser = 5f;
public float tiempoInicialLaser = 2f;

private float temporizadorLaser;
private bool laserActivo = true;

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

    // Buscar ataque de raíces
    ataqueCircular = GetComponent<AtaqueRaicesCircular>();

    // Inicializar enfriamiento
    attackTimer = attackCooldown;

    // Inicializar barra de vida
    if (barraVidaJefe != null)
    {
        barraVidaJefe.InicializarVida(vida);
        barraVidaJefe.gameObject.SetActive(false);
    }

    // Comprobar configuración del ataque seleccionado
    if (tipoAtaque == TipoAtaque.Raices && ataqueCircular == null)
    {
        Debug.LogWarning("Este enemigo está configurado para raíces, pero no tiene AtaqueRaicesCircular.");
    }

    if (tipoAtaque == TipoAtaque.Laser &&
        (prefabLaser == null || puntoDisparoLaser == null))
    {
        Debug.LogWarning("Falta asignar el prefab o el punto de disparo del láser.");
    }
}

    // =========================
    // UPDATE
    // =========================

    


void Update()
{
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
    // DETECTAR JUGADOR
    // =========================

    float distanceToPlayer = Vector2.Distance(
        transform.position,
        player.position
    );

    if (distanceToPlayer <= detectionRadius)
    {
        if (barraVidaJefe != null)
            barraVidaJefe.gameObject.SetActive(true);

        Vector2 direction = (
            player.position - transform.position
        ).normalized;

        float rangoActual = tipoAtaque == TipoAtaque.Laser
            ? rangoAtaqueLaser
            : attackRange;

        // =========================
        // MOVIMIENTO
        // =========================

        if (distanceToPlayer > rangoActual)
        {
            // Perseguir al jugador
            movement = direction;
        }
        else
        {
            // Detenerse para atacar
            movement = Vector2.zero;
        }

        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        animator.SetFloat("Speed", movement.magnitude);

        // =========================
        // TEMPORIZADOR
        // =========================

        attackTimer -= Time.deltaTime;

        // =========================
        // ATACAR
        // =========================

        if (attackTimer <= 0f &&
            distanceToPlayer <= rangoActual)
        {
            Atacar();
            return;
        }
    }
    else
    {
        movement = Vector2.zero;

        if (barraVidaJefe != null)
            barraVidaJefe.gameObject.SetActive(false);
    }

    // =========================
    // APLICAR MOVIMIENTO
    // =========================

    rb.MovePosition(
        rb.position + movement * speed * Time.deltaTime
    );
}




    // =========================
    // ATACAR
    // =========================


void Atacar()
{
    if (muerto || isAttacking || player == null)
        return;

    isAttacking = true;

    // Detener movimiento
    movement = Vector2.zero;
    rb.linearVelocity = Vector2.zero;

    // Calcular dirección del jugador
    Vector2 direccion = (
        player.position - transform.position
    ).normalized;

    // Actualizar dirección del ataque si corresponde
    if (tipoAtaque == TipoAtaque.Laser)
    {
        animator.SetFloat("Horizontal", direccion.x);
        animator.SetFloat("Vertical", direccion.y);
    }

    // Detener animación de movimiento
    animator.SetFloat("Speed", 0f);

    // Reproducir animación de ataque
    animator.SetTrigger("Attack");

    // Ejecutar ataque según el tipo
    switch (tipoAtaque)
    {
        case TipoAtaque.Raices:

            if (ataqueCircular != null)
            {
                ataqueCircular.EjecutarAtaque();
            }
            else
            {
                Debug.LogWarning(
                    "No se encontró AtaqueRaicesCircular."
                );
            }

            break;

        case TipoAtaque.Laser:

            // El láser se lanzará mediante Animation Event
            break;
    }

    // Tiempo que permanece atacando
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

    // Detener movimiento y ataques
    movement = Vector2.zero;
    isAttacking = false;

    StopAllCoroutines();

    // Detener Rigidbody
    rb.linearVelocity = Vector2.zero;
    rb.simulated = false;

    // Desactivar colisión
    if (col != null)
    {
        col.enabled = false;
    }

    // Cancelar ataque de raíces si corresponde
    if (ataqueCircular != null)
    {
        ataqueCircular.CancelarAtaque();
    }

    // Reproducir animación de muerte
    animator.ResetTrigger("Attack");
    animator.SetFloat("Speed", 0f);
    animator.SetTrigger("Death");

    // Notificar a la sala
    if (salaJefe != null)
    {
        salaJefe.NotificarJefeDerrotado();
    }

    Debug.Log("El enemigo ha sido derrotado");

    StartCoroutine(DestruirDespuesDeMuerte());
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

private IEnumerator DestruirDespuesDeMuerte()
{
    yield return null;

    // Esperar a que comience Death
    while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
    {
        yield return null;
    }

    AnimatorStateInfo estado =
        animator.GetCurrentAnimatorStateInfo(0);

    float duracion = estado.length;

    yield return new WaitForSeconds(duracion);

    Destroy(gameObject);
}

private void LanzarLaser()
{
    if (muerto || player == null)
        return;

    if (prefabLaser == null || puntoDisparoLaser == null)
    {
        Debug.LogWarning("No se puede lanzar el láser: faltan referencias.");
        return;
    }

    GameObject laser = Instantiate(
        prefabLaser,
        puntoDisparoLaser.position,
        Quaternion.identity
    );

    LaserGiratorio script =
        laser.GetComponent<LaserGiratorio>();

    if (script != null)
    {
        script.Inicializar(player, transform);
    }
    else
    {
        Debug.LogError("El prefab no contiene LaserGiratorio.");
        Destroy(laser);
    }
}

public void EventoDispararLaser()
{
    if (muerto || tipoAtaque != TipoAtaque.Laser)
        return;

    LanzarLaser();
}
}