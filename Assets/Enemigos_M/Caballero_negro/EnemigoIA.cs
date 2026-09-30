using UnityEngine;

public class EnemigoIA : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 3f;
    public float rangoDeteccion = 6f;
    public float rangoAtaque = 2f; // Sube esto desde el Inspector para que no se pegue tanto

    [Header("Ataque")]
    public int danoAtaque = 10;
    public float tiempoEntreAtaques = 1.5f;
    private float siguienteAtaque = 0f;

    [Header("Componentes")]
    private Transform jugador;
    private Rigidbody2D rb;
    private Animator animator;

    // Guardar la última dirección válida para no perder la animación
    private Vector2 ultimaDireccion = Vector2.down;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        BuscarJugador();
    }

    void Update()
    {
        if (jugador == null)
        {
            BuscarJugador();
            DetenerEnemigo();
            return;
        }

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= rangoDeteccion && distancia > rangoAtaque)
        {
            MoverHaciaJugador();
        }
        else if (distancia <= rangoAtaque)
        {
            DetenerEnemigo();

            if (Time.time >= siguienteAtaque)
            {
                Atacar();
                siguienteAtaque = Time.time + tiempoEntreAtaques;
            }
        }
        else
        {
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
        Vector2 direccion = (jugador.position - transform.position).normalized;
        rb.linearVelocity = direccion * velocidad;

        // Convertir la dirección a los ejes principales (-1, 1)
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
        if (jugador != null)
        {
            Vector2 direccionAtaque = (jugador.position - transform.position).normalized;
            ActualizarDireccionDominante(direccionAtaque);
        }

        if (animator != null)
        {
            // Forzar las coordenadas exactas de la última dirección antes de tirar el Trigger
            animator.SetFloat("LastHorizontal", ultimaDireccion.x);
            animator.SetFloat("LastVertical", ultimaDireccion.y);

            animator.SetTrigger("Atacar");
        }

        Debug.Log("¡El Caballero Oscuro atacó al jugador!");
    }

    // Método para redondear la dirección al eje con mayor fuerza (Evita que pase a 0,0)
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