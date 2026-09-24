using UnityEngine;

public class JugadorMovimiento : MonoBehaviour
{
    public float velocidad = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movimiento;
    private Vector2 ultimaDireccion;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Por defecto, mirar hacia abajo al empezar
        ultimaDireccion = new Vector2(0, -1);
    }

    void Update()
    {
        movimiento.x = Input.GetAxisRaw("Horizontal");
        movimiento.y = Input.GetAxisRaw("Vertical");

        if (movimiento != Vector2.zero)
        {
            // Si nos estamos moviendo, actualizamos la última dirección
            ultimaDireccion = movimiento;
            
            animator.SetBool("IsMoving", true);
            animator.SetFloat("Horizontal", movimiento.x);
            animator.SetFloat("Vertical", movimiento.y);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

        // Enviamos la última dirección registrada para el Blend Tree de Idle
        animator.SetFloat("LastHorizontal", ultimaDireccion.x);
        animator.SetFloat("LastVertical", ultimaDireccion.y);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movimiento.normalized * velocidad * Time.fixedDeltaTime);
    }
}