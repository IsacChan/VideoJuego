using UnityEngine;
using System.Collections;

public class CaballeroHealth : MonoBehaviour
{
    [Header("Vida del Caballero Oscuro")]
    public int vidaMaxima = 3;
    private int vidaActual;

    private Animator animator;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    private bool muerto = false;

    [Header("Efecto de Daño")]
    public float tiempoParpadeo = 0.1f;
    public int numeroParpadeos = 3;

    void Start()
    {
        vidaActual = vidaMaxima;
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Aseguramos que el Animator empiece con IsDead en falso al iniciar el juego
        if (animator != null)
        {
            animator.SetBool("IsDead", false);
        }
    }

    // Este método es llamado por la espada del jugador
    public void TakeDamage(int danio)
    {
        if (muerto) return;

        vidaActual -= danio;
        Debug.Log("¡Golpe al Caballero Oscuro! Vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
        else
        {
            // Activar parpadeo si sigue vivo
            StartCoroutine(ParpadearDano());
        }
    }

    IEnumerator ParpadearDano()
    {
        if (spriteRenderer == null) yield break;

        for (int i = 0; i < numeroParpadeos; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(tiempoParpadeo);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(tiempoParpadeo);
        }
        
        spriteRenderer.enabled = true;
    }

    void Morir()
    {
        if (muerto) return;
        muerto = true;

        // Desactivar el colisor para que no siga interactuando
        if (col != null)
        {
            col.enabled = false;
        }

        // Avisar a la IA que ha muerto para que detenga su movimiento y físicas
        EnemigoIA ia = GetComponent<EnemigoIA>();
        if (ia != null)
        {
            ia.MorirAI();
        }

        // Activa la animación de muerte
        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }

        Debug.Log("El Caballero Oscuro ha muerto.");
    }

    // Método llamado por el Animation Event en el último fotograma de la animación de Muerte
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}