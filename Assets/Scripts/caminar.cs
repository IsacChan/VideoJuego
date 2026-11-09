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

    [Header("Ataque automático")]
    public float radioDeteccionAtaque = 3f;

    private Vector2 direccionAtaque = Vector2.down;

    public GameObject slashEffect;

    [Header("Vida")]
    public int vidaMaxima = 20; // 5 corazones * 4 puntos fraccionarios cada uno
    public int vidaActual;

    private bool muerto = false;

    private VidaJugador sistemaRespawn;

    [Header("UI de Corazones")]
    public ControladorCorazones controladorCorazones; // Arrastra aquí tu ContenedorCorazones desde Unity

    private EmbestidaJugador sistemaEmbestida;


    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        spriteRenderer = GetComponent<SpriteRenderer>();

        vidaActual = vidaMaxima;

        // Inicializar la UI de corazones al arrancar
        if (controladorCorazones != null)
        {
            controladorCorazones.ActualizarVida(vidaActual, false);
        }

        sistemaRespawn = GetComponent<VidaJugador>();
        sistemaEmbestida = GetComponent<EmbestidaJugador>();
    }


    void Update()
    {
        if (muerto)
        {
            movementInput = Vector2.zero;
            return;
        }

        if (sistemaEmbestida != null && sistemaEmbestida.EstaEmbistiendo)
        {
            movementInput = Vector2.zero;

            animator.SetFloat("Speed", 0);
            animator.SetBool("atacar", false);

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
        
        if (!atacando)
        {
            direccionAtaque = ultimaDireccion;
        }

        // =========================
        // ANIMATOR
        // =========================

        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);
        animator.SetFloat("Speed", movementInput.magnitude);

        // Dirección del ataque
        animator.SetFloat("AttackX", direccionAtaque.x);
        animator.SetFloat("AttackY", direccionAtaque.y);

        animator.SetBool("atacar", atacando);
    }


    void FixedUpdate()
    {
        // Jugador muerto
        if (muerto)
        {
            rb2D.linearVelocity = Vector2.zero;
            return;
        }

        // La embestida controla el Rigidbody2D
        if (sistemaEmbestida != null && sistemaEmbestida.EstaEmbistiendo)
        {
            return;
        }

        // Permitir el rebote cuando recibe daño
        if (recibiendoDanio)
        {
            return;
        }

        // No moverse mientras ataca
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

    private Vector2 BuscarDireccionEnemigo()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(
            transform.position,
            radioDeteccionAtaque
        );

        Transform enemigoCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (Collider2D objeto in objetos)
        {
            Transform actual = objeto.transform;

            while (actual != null && !actual.CompareTag("Enemy"))
            {
                actual = actual.parent;
            }

            if (actual == null || !actual.gameObject.activeInHierarchy)
                continue;

            float distancia = Vector2.Distance(
                transform.position,
                objeto.ClosestPoint(transform.position)
            );

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                enemigoCercano = actual;
            }
        }

        if (enemigoCercano == null)
            return ultimaDireccion;

        Vector2 direccion = (
            enemigoCercano.position - transform.position
        ).normalized;

        if (Mathf.Abs(direccion.x) > Mathf.Abs(direccion.y))
        {
            return direccion.x > 0
                ? Vector2.right
                : Vector2.left;
        }

        return direccion.y > 0
            ? Vector2.up
            : Vector2.down;
    }


    public void Atacando()
    {
        if (atacando || muerto || recibiendoDanio)
            return;

        if (sistemaEmbestida != null && sistemaEmbestida.EstaEmbistiendo)
        {
            return;
        }

        direccionAtaque = BuscarDireccionEnemigo();

        animator.SetFloat("AttackX", direccionAtaque.x);
        animator.SetFloat("AttackY", direccionAtaque.y);

        movementInput = Vector2.zero;
        rb2D.linearVelocity = Vector2.zero;

        atacando = true;
        animator.SetBool("atacar", true);

        Debug.Log("Dirección automática: " + direccionAtaque);
    }


    public void EjecutarGolpe()
    {
        if (muerto || recibiendoDanio || (sistemaEmbestida != null && sistemaEmbestida.EstaEmbistiendo))
        {
            return;
        }

        if (attackHitbox == null)
            return;

        attackHitbox.transform.localPosition = direccionAtaque * distanciaAtaque;

        AttackHitbox hitbox = attackHitbox.GetComponent<AttackHitbox>();

        if (hitbox != null)
        {
            hitbox.NuevoAtaque();
        }

        attackHitbox.SetActive(true);

        if (slashEffect != null)
        {
            SlashEffect slash = slashEffect.GetComponent<SlashEffect>();

            if (slash != null)
            {
                slash.Mostrar(direccionAtaque);
            }
        }
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
    // RECIBIR DAÑO Y REBOTE
    // =========================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (sistemaEmbestida != null && sistemaEmbestida.EstaEmbistiendo)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            RecibirDanio(collision.transform);
        }
    }


    private void RecibirDanio(Transform enemigo)
    {
        if (sistemaEmbestida != null && sistemaEmbestida.EstaEmbistiendo)
        {
            return;
        }

        if (recibiendoDanio || muerto)
            return;

        vidaActual -= 1;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        Debug.Log("Vida del jugador: " + vidaActual);

        if (controladorCorazones != null)
        {
            controladorCorazones.ActualizarVida(vidaActual, true);
        }

        recibiendoDanio = true;
        atacando = false;

        attackHitbox.SetActive(false);
        rb2D.linearVelocity = Vector2.zero;

        Vector2 direccionRebote = (transform.position - enemigo.position).normalized;

        rb2D.AddForce(
            direccionRebote * fuerzaRebote,
            ForceMode2D.Impulse
        );

        if (vidaActual <= 0)
        {
            StartCoroutine(ReboteMortal());
            return;
        }

        StartCoroutine(Paralizar());
        StartCoroutine(Parpadear());
        StartCoroutine(DetenerRebote());
    }


    // =========================
    // CORRUTINAS DE ESTADO
    // =========================

    private IEnumerator Paralizar()
    {
        yield return new WaitForSeconds(tiempoParalizado);
        rb2D.linearVelocity = Vector2.zero;
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

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        StopAllCoroutines();

        spriteRenderer.enabled = true;

        animator.SetTrigger("Death");

        Debug.Log("El jugador ha muerto");

        if (ControladorDerrota.Instancia != null)
        {
            ControladorDerrota.Instancia.MostrarDerrota();
        }
        else
        {
            Debug.LogError("No existe ControladorDerrota en la escena.");
        }
    }

    private IEnumerator ReboteMortal()
    {
        yield return new WaitForSeconds(duracionRebote);
        rb2D.linearVelocity = Vector2.zero;
        Morir();
    }

    public void RecibirDanioRaices(Transform origen)
    {
        RecibirDanio(origen);
    }

    public void Revivir()
    {
        vidaActual = vidaMaxima;

        muerto = false;
        recibiendoDanio = false;
        atacando = false;

        movementInput = Vector2.zero;
        rb2D.linearVelocity = Vector2.zero;

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        spriteRenderer.enabled = true;

        animator.Rebind();
        animator.Update(0f);

        if (controladorCorazones != null)
        {
            controladorCorazones.ActualizarVida(vidaActual, false);
        }

        Debug.Log("Jugador revivido con vida completa");
    }

    public bool PuedeUsarHabilidades()
    {
        return !muerto && !recibiendoDanio && !atacando;
    }

    public bool PuedeContinuarEmbestida()
    {
        return !muerto && !recibiendoDanio;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccionAtaque);
    }

    public void RestaurarVidaGuardada(int cantidad)
    {
        vidaActual = Mathf.Clamp(cantidad, 1, vidaMaxima);

        muerto = false;
        recibiendoDanio = false;
        atacando = false;

        movementInput = Vector2.zero;

        if (rb2D != null)
            rb2D.linearVelocity = Vector2.zero;

        if (controladorCorazones != null)
        {
            controladorCorazones.ActualizarVida(vidaActual, false);
        }

        Debug.Log("Vida restaurada correctamente: " + vidaActual);
    }

    // =========================
    // MÉTODO PARA CURAR DESDE EL INVENTARIO
    // =========================
    public void Curar(int cantidadACurar)
    {
        if (muerto) return;

        vidaActual += cantidadACurar;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        if (controladorCorazones != null)
        {
            controladorCorazones.ActualizarVida(vidaActual, false);
        }

        Debug.Log("¡Salud restaurada! Vida actual: " + vidaActual);
    }
}