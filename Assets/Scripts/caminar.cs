using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class caminar : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb2D;
    private Vector2 movementInput;

    // Última dirección en la que estaba mirando el personaje
    private Vector2 ultimaDireccion = Vector2.down;

    private Animator animator;

    private bool atacando;

    // =========================
    // DAÑO Y REBOTE
    // =========================

    public float fuerzaRebote = 8f;

    public float duracionRebote = 0.15f;
    public float tiempoParalizado = 2f;

    private bool recibiendoDanio = false;

    private SpriteRenderer spriteRenderer;

public float tiempoParpadeo = 0.1f;

public GameObject attackHitbox;

public float distanciaAtaque = 0.8f;

public GameObject slashEffect;

[Header("Vida")]
public int vidaMaxima = 3;
public int vidaActual;

private bool muerto = false;


    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        attackHitbox.SetActive(false);

        spriteRenderer = GetComponent<SpriteRenderer>();

        vidaActual = vidaMaxima;
    }


    void Update()
    {
        // =========================
        // SI RECIBE DAÑO
        // =========================

        if (muerto)
{
    movementInput = Vector2.zero;
    return;

}

        if (recibiendoDanio)
        {
            movementInput = Vector2.zero;

            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
            animator.SetFloat("Speed", 0);

            return;
        }

        if (atacando)
{
    movementInput = Vector2.zero;
    return;
}


        movementInput = Vector2.zero;


        // =========================
        // MOVIMIENTO
        // =========================

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed)
            {
                movementInput.x = -1;
            }

            if (Keyboard.current.dKey.isPressed ||
                Keyboard.current.rightArrowKey.isPressed)
            {
                movementInput.x = 1;
            }

            if (Keyboard.current.wKey.isPressed ||
                Keyboard.current.upArrowKey.isPressed)
            {
                movementInput.y = 1;
            }

            if (Keyboard.current.sKey.isPressed ||
                Keyboard.current.downArrowKey.isPressed)
            {
                movementInput.y = -1;
            }


            // =========================
            // ATAQUE
            // =========================

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Atacando();
            }
        }


        movementInput = movementInput.normalized;


        // =========================
        // GUARDAR ÚLTIMA DIRECCIÓN
        // =========================

        if (movementInput != Vector2.zero)
        {
            ultimaDireccion = movementInput;
        }


        // =========================
        // ANIMATOR
        // =========================

        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);
        animator.SetFloat("Speed", movementInput.magnitude);

        // Dirección del ataque
        animator.SetFloat("AttackX", ultimaDireccion.x);
        animator.SetFloat("AttackY", ultimaDireccion.y);

        animator.SetBool("atacar", atacando);
    }


    void FixedUpdate()
{
    // Si está muerto, detener completamente
    if (muerto)
    {
        rb2D.linearVelocity = Vector2.zero;
        return;
    }

    // Si recibe daño, dejamos que funcione el rebote
    if (recibiendoDanio)
    {
        return;
    }

    // Mientras está atacando NO puede moverse
    if (atacando)
    {
        rb2D.linearVelocity = Vector2.zero;
        return;
    }

    // Movimiento normal
    rb2D.linearVelocity = movementInput * speed;
}


    // =========================
    // ATAQUE
    // =========================

public void Atacando()
{
    if (atacando)
        return;

    atacando = true;
}

public void EjecutarGolpe()
{
    // Colocar hitbox en la dirección del ataque
    attackHitbox.transform.localPosition =
        ultimaDireccion * distanciaAtaque;

    // Reiniciar enemigos golpeados
    attackHitbox.GetComponent<AttackHitbox>().NuevoAtaque();

    // Activar daño
    attackHitbox.SetActive(true);

    // Mostrar efecto visual
    slashEffect.GetComponent<SlashEffect>().Mostrar(ultimaDireccion);
}

public void TerminarGolpe()
{
    attackHitbox.SetActive(false);
}


    public void NoAtacando()
{
    atacando = false;
    attackHitbox.SetActive(false);
}


    // =========================
    // RECIBIR DAÑO
    // =========================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            RecibirDanio(collision.transform);
        }
    }


    // =========================
    // REBOTE
    // =========================

   private void RecibirDanio(Transform enemigo)
{
    if (recibiendoDanio || muerto)
        return;

    // Quitar vida
    vidaActual--;

    Debug.Log("Vida del jugador: " + vidaActual);

    recibiendoDanio = true;
    atacando = false;

    attackHitbox.SetActive(false);

    rb2D.linearVelocity = Vector2.zero;

    // Dirección contraria al enemigo
    Vector2 direccionRebote =
        (transform.position - enemigo.position).normalized;

    // Aplicar rebote
    rb2D.AddForce(
        direccionRebote * fuerzaRebote,
        ForceMode2D.Impulse
    );

    // =========================
    // GOLPE MORTAL
    // =========================

    if (vidaActual <= 0)
    {
        StartCoroutine(ReboteMortal());
        return;
    }

    // =========================
    // GOLPE NORMAL
    // =========================

    StartCoroutine(Paralizar());
    StartCoroutine(Parpadear());
    StartCoroutine(DetenerRebote());
}


    // =========================
    // PARALIZAR
    // =========================

    private IEnumerator Paralizar()
    {
        yield return new WaitForSeconds(tiempoParalizado);

        // Detener el rebote
        rb2D.linearVelocity = Vector2.zero;

        // Volver a permitir movimiento
        recibiendoDanio = false;
    }

    private IEnumerator Parpadear()
{
    float tiempoTranscurrido = 0f;

    while (tiempoTranscurrido < tiempoParalizado)
    {
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(tiempoParpadeo);

        spriteRenderer.enabled = true;

        yield return new WaitForSeconds(tiempoParpadeo);

        tiempoTranscurrido += tiempoParpadeo * 2f;
    }

    spriteRenderer.enabled = true;
}

private IEnumerator DetenerRebote()
{
    yield return new WaitForSeconds(duracionRebote);

    rb2D.linearVelocity = Vector2.zero;
}

private void Morir()
{
    if (muerto)
        return;

    muerto = true;
    recibiendoDanio = false;
    atacando = false;

    movementInput = Vector2.zero;
    rb2D.linearVelocity = Vector2.zero;

    attackHitbox.SetActive(false);

    // Detener parpadeo si estaba recibiendo daño
    StopAllCoroutines();

    spriteRenderer.enabled = true;

    // Reproducir animación de muerte
    animator.SetTrigger("Death");

    Debug.Log("El jugador ha muerto");
}

private IEnumerator ReboteMortal()
{
    // Esperar mientras ocurre el pequeño rebote
    yield return new WaitForSeconds(duracionRebote);

    // Detener al jugador
    rb2D.linearVelocity = Vector2.zero;

    // Ejecutar muerte
    Morir();
}
}