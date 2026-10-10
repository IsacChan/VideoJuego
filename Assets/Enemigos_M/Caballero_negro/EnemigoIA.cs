using UnityEngine;

public class EnemigoIA : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 2f;
    public float rangoDeteccion = 5f;
    public float rangoAtaque = 1.2f;

    [Header("Ataque")]
    public int danoAtaque = 10;
    public float tiempoEntreAtaques = 1.5f;
    private float siguienteAtaque = 0f;

    [Header("Hitboxes de la Espada (4 Direcciones)")]
    public Collider2D hitboxAbajo;
    public Collider2D hitboxArriba;
    public Collider2D hitboxIzquierda;
    public Collider2D hitboxDerecha;

    [Header("Componentes")]
    private Transform jugador;
    private Rigidbody2D rb;
    private Animator animator;

    // Control de dirección y estado
    private Vector2 ultimaDireccion = Vector2.down;
    private bool estaAtacando = false;
    private bool muerto = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        BuscarJugador();

        // Apagar todos los hitboxes al iniciar por seguridad
        DesactivarTodosLosHitboxes();
    }

    void Update()
    {
        if (muerto) return;

        if (jugador == null)
        {
            BuscarJugador();
            DetenerEnemigo();
            return;
        }

        if (estaAtacando)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= rangoAtaque)
        {
            rb.linearVelocity = Vector2.zero;

            if (Time.time >= siguienteAtaque)
            {
                Atacar();
                siguienteAtaque = Time.time + tiempoEntreAtaques;
            }
            else
            {
                DetenerEnemigo();
            }
        }
        else if (distancia <= rangoDeteccion)
        {
            MoverHaciaJugador();
        }
        else
        {
            DetenerEnemigo();
        }
    }

    public void MorirAI()
    {
        muerto = true;
        rb.linearVelocity = Vector2.zero;
        DesactivarTodosLosHitboxes();
    }

    void BuscarJugador()
    {
        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }
    }

    void MoverHaciaJugador()
    {
        Vector2 direccion = (jugador.position - transform.position).normalized;
        rb.linearVelocity = direccion * velocidad;

        ActualizarDireccionDominante(direccion);

        if (animator != null)
        {
            animator.SetFloat("Horizontal", ultimaDireccion.x);
            animator.SetFloat("Vertical", ultimaDireccion.y);
            animator.SetFloat("LastHorizontal", ultimaDireccion.x);
            animator.SetFloat("LastVertical", ultimaDireccion.y);
            animator.SetBool("IsMoving", true);
        }
    }

    void DetenerEnemigo()
    {
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetFloat("Horizontal", ultimaDireccion.x);
            animator.SetFloat("Vertical", ultimaDireccion.y);
            animator.SetFloat("LastHorizontal", ultimaDireccion.x);
            animator.SetFloat("LastVertical", ultimaDireccion.y);
            animator.SetBool("IsMoving", false);
        }
    }

    void Atacar()
    {
        estaAtacando = true;
        rb.linearVelocity = Vector2.zero;

        if (jugador != null)
        {
            Vector2 direccionAtaque = (jugador.position - transform.position).normalized;
            ActualizarDireccionDominante(direccionAtaque);
        }

        if (animator != null)
        {
            animator.SetFloat("LastHorizontal", ultimaDireccion.x);
            animator.SetFloat("LastVertical", ultimaDireccion.y);
            animator.SetBool("IsMoving", false);
            animator.SetTrigger("Atacar");
        }

        Debug.Log("¡El Caballero Oscuro atacó al jugador!");
    }

    public void FinalizarAtaque()
    {
        estaAtacando = false;
    }

    // --- CONTROL DE HITBOXES SEGÚN LA DIRECCIÓN ---
    public void ActivarHitboxEspada()
    {
        DesactivarTodosLosHitboxes();

        // Evalúa hacia dónde está mirando el caballero y activa el colisor correspondiente
        if (Mathf.Abs(ultimaDireccion.x) > Mathf.Abs(ultimaDireccion.y))
        {
            if (ultimaDireccion.x > 0 && hitboxDerecha != null)
            {
                hitboxDerecha.enabled = true;
            }
            else if (ultimaDireccion.x < 0 && hitboxIzquierda != null)
            {
                hitboxIzquierda.enabled = true;
            }
        }
        else
        {
            if (ultimaDireccion.y > 0 && hitboxArriba != null)
            {
                hitboxArriba.enabled = true;
            }
            else if (ultimaDireccion.y < 0 && hitboxAbajo != null)
            {
                hitboxAbajo.enabled = true;
            }
        }
    }

    public void DesactivarHitboxEspada()
    {
        DesactivarTodosLosHitboxes();
    }

    void DesactivarTodosLosHitboxes()
    {
        if (hitboxAbajo != null) hitboxAbajo.enabled = false;
        if (hitboxArriba != null) hitboxArriba.enabled = false;
        if (hitboxIzquierda != null) hitboxIzquierda.enabled = false;
        if (hitboxDerecha != null) hitboxDerecha.enabled = false;
    }
    // ---------------------------------------------

    void ActualizarDireccionDominante(Vector2 dir)
    {
        if (dir == Vector2.zero) return;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            ultimaDireccion = new Vector2(dir.x > 0 ? 1 : -1, 0);
        }
        else
        {
            ultimaDireccion = new Vector2(0, dir.y > 0 ? 1 : -1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}