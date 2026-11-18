using UnityEngine;

public class EnemigoIA : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 2f;
    public float rangoDeteccion = 5f;
    public float rangoAtaque = 1.2f;

    [Header("Búsqueda / Vigilancia (Estatio girando)")]
    public float tiempoCambioMirada = 2.5f; // Cada cuánto tiempo cambia de dirección al buscar
    private float cronometroMirada = 0f;
    private Vector2 direccionBusqueda = Vector2.down;

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
        ElegirNuevaDireccionBusqueda();

        // Apagar todos los hitboxes al iniciar por seguridad
        DesactivarTodosLosHitboxes();
    }

    void Update()
    {
        if (muerto) return;

        if (jugador == null)
        {
            BuscarJugador();
            VigilarOrizontalVertical();
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
            // SI EL JUGADOR ESTÁ LEJOS: Se queda vigilando / buscando girando en su sitio
            VigilarOrizontalVertical();
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
        // DOBLE SEGURIDAD: Apagar hitboxes por si se quedaron encendidas
        DesactivarTodosLosHitboxes(); 

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

    void VigilarOrizontalVertical()
    {
        DesactivarTodosLosHitboxes();

        // El enemigo se queda completamente quieto en su posición
        rb.linearVelocity = Vector2.zero;

        cronometroMirada -= Time.deltaTime;
        if (cronometroMirada <= 0f)
        {
            ElegirNuevaDireccionBusqueda();
            cronometroMirada = tiempoCambioMirada;
        }

        // Asignamos la dirección de búsqueda a la última dirección para que gire el sprite
        ultimaDireccion = direccionBusqueda;

        if (animator != null)
        {
            animator.SetFloat("Horizontal", ultimaDireccion.x);
            animator.SetFloat("Vertical", ultimaDireccion.y);
            animator.SetFloat("LastHorizontal", ultimaDireccion.x);
            animator.SetFloat("LastVertical", ultimaDireccion.y);
            animator.SetBool("IsMoving", false); // Estático pero cambiando de orientación
        }
    }

    void ElegirNuevaDireccionBusqueda()
    {
        // Elige aleatoriamente una de las 4 direcciones cardinales para mirar
        int opcion = Random.Range(0, 4);
        switch (opcion)
        {
            case 0: direccionBusqueda = Vector2.up; break;      // Arriba
            case 1: direccionBusqueda = Vector2.down; break;    // Abajo
            case 2: direccionBusqueda = Vector2.left; break;    // Izquierda
            case 3: direccionBusqueda = Vector2.right; break;   // Derecha
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

        Debug.Log("¡El Enemigo atacó al jugador!");
    }

    public void FinalizarAtaque()
    {
        estaAtacando = false;
        DesactivarTodosLosHitboxes(); // Seguro por si la animación falla
    }

    // --- CONTROL DE HITBOXES SEGÚN LA DIRECCIÓN ---
    public void ActivarHitboxEspada()
    {
        DesactivarTodosLosHitboxes();

        // Evalúa hacia dónde está mirando y activa el colisor correspondiente
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

        // SOLUCIÓN DEFINITIVA: Apaga la hitbox en 0.2 segundos automáticamente
        Invoke("DesactivarTodosLosHitboxes", 0.2f);
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