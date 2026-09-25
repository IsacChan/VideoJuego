using UnityEngine;

public class EnemigoIA : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 3f;
    public float rangoDeteccion = 6f;
    public float rangoAtaque = 1.2f;

    [Header("Ataque")]
    public int danoAtaque = 10;
    public float tiempoEntreAtaques = 1.5f;
    private float siguienteAtaque = 0f;

    [Header("Componentes")]
    private Transform jugador;
    private Rigidbody2D rb;
    private Animator animator;

    // Para recordar la última dirección a la que miró (útil para el Idle)
    private Vector2 ultimaDireccion = Vector2.down;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Busca automáticamente al jugador por su Tag "Player" en la escena
        BuscarJugador();
    }

    void Update()
    {
        // Si el jugador no existe o se eliminó, intenta buscarlo de nuevo
        if (jugador == null)
        {
            BuscarJugador();
            DetenerEnemigo();
            return;
        }

        // Calcular distancia hacia el jugador
        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= rangoDeteccion && distancia > rangoAtaque)
        {
            // El jugador está en rango de visión: Perseguir
            MoverHaciaJugador();
        }
        else if (distancia <= rangoAtaque)
        {
            // El jugador está en rango de golpe: Detenerse y Atacar
            DetenerEnemigo();

            if (Time.time >= siguienteAtaque)
            {
                Atacar();
                siguienteAtaque = Time.time + tiempoEntreAtaques;
            }
        }
        else
        {
            // Fuera de rango: Quedarse quieto (Idle)
            DetenerEnemigo();
        }
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
        // Dirección normalizada hacia el jugador (X e Y para vista Top-Down 2D)
        Vector2 direccion = (jugador.position - transform.position).normalized;
        rb.linearVelocity = direccion * velocidad;

        // Guardar la dirección actual para mantener la orientación
        ultimaDireccion = direccion;

        // Actualizar parámetros del Blend Tree 2D
        if (animator != null)
        {
            animator.SetFloat("Horizontal", direccion.x);
            animator.SetFloat("Vertical", direccion.y);
            
            // Si tienes variables LastHorizontal / LastVertical en tu Animator, también se actualizan
            animator.SetFloat("LastHorizontal", direccion.x);
            animator.SetFloat("LastVertical", direccion.y);

            animator.SetBool("IsMoving", true);
        }
    }

    void DetenerEnemigo()
    {
        rb.linearVelocity = Vector2.zero;

        // Apagar movimiento en el Animator manteniendo la dirección en que quedó mirando
        if (animator != null)
        {
            animator.SetFloat("Horizontal", ultimaDireccion.x);
            animator.SetFloat("Vertical", ultimaDireccion.y);
            animator.SetBool("IsMoving", false);
        }
    }

    void Atacar()
    {
        // Disparar la animación de ataque
        if (animator != null)
        {
            animator.SetTrigger("Atacar");
        }

        Debug.Log("¡El Caballero Oscuro atacó al jugador!");
    }

    // Dibuja los rangos en la ventana Scene para ayudarte a configurarlos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}