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


    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        attackHitbox.SetActive(false);

        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        // =========================
        // SI RECIBE DAÑO
        // =========================

        if (recibiendoDanio)
        {
            movementInput = Vector2.zero;

            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
            animator.SetFloat("Speed", 0);

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
        // Mientras recibe daño, no controlamos
        // la velocidad porque queremos que el rebote funcione.
        if (recibiendoDanio)
            return;

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
    if (recibiendoDanio)
        return;

    recibiendoDanio = true;

    atacando = false;

    // Detener movimiento
    rb2D.linearVelocity = Vector2.zero;

    // Dirección contraria al enemigo
    Vector2 direccionRebote =
        (transform.position - enemigo.position).normalized;

    // Rebote brusco
    rb2D.AddForce(
        direccionRebote * fuerzaRebote,
        ForceMode2D.Impulse
    );

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
}