using UnityEngine;

public class JugadorMovimiento : MonoBehaviour
{
    public float velocidad = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movimiento;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Leer la entrada de las teclas A/D (X) y W/S (Y)
        movimiento.x = Input.GetAxisRaw("Horizontal");
        movimiento.y = Input.GetAxisRaw("Vertical");

        // Mandar los valores al Blend Tree
        animator.SetFloat("Horizontal", movimiento.x);
        animator.SetFloat("Vertical", movimiento.y);
    }

    void FixedUpdate()
    {
        // Mover al personaje usando físicas
        rb.MovePosition(rb.position + movimiento.normalized * velocidad * Time.fixedDeltaTime);
    }
}