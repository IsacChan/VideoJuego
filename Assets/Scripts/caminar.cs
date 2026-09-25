using UnityEngine;
using UnityEngine.InputSystem;

public class caminar : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb2D;
    private Vector2 movementInput;

    private Animator animator;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movementInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                movementInput.x = -1;

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                movementInput.x = 1;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                movementInput.y = 1;

            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                movementInput.y = -1;
        }

        movementInput = movementInput.normalized;

        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);
        animator.SetFloat("Speed", movementInput.magnitude);
    }

    void FixedUpdate()
    {
        rb2D.linearVelocity = movementInput * speed;
    }
}