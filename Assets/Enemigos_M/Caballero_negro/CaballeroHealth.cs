
using UnityEngine;
using System.Collections;

public class CaballeroHealth : MonoBehaviour, IRecibirDanio
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

        if (animator != null)
        {
            animator.SetBool("IsDead", false);
        }
    }

    // =========================
    // RECIBIR DAÑO DE EMBESTIDA
    // =========================

    public void RecibirDanio(int cantidad)
    {
        TakeDamage(cantidad);
    }

    // =========================
    // RECIBIR DAÑO DE LA ESPADA
    // =========================

    public void TakeDamage(int danio)
    {
        if (muerto)
            return;

        vidaActual = Mathf.Max(vidaActual - danio, 0);

        Debug.Log(
            "¡Golpe al Caballero Oscuro! Vida restante: " +
            vidaActual
        );

        if (vidaActual <= 0)
        {
            Morir();
        }
        else
        {
            StartCoroutine(ParpadearDano());
        }
    }

    // =========================
    // PARPADEAR AL RECIBIR DAÑO
    // =========================

    IEnumerator ParpadearDano()
    {
        if (spriteRenderer == null)
            yield break;

        for (int i = 0; i < numeroParpadeos; i++)
        {
            spriteRenderer.enabled = false;

            yield return new WaitForSeconds(tiempoParpadeo);

            spriteRenderer.enabled = true;

            yield return new WaitForSeconds(tiempoParpadeo);
        }

        spriteRenderer.enabled = true;
    }

    // =========================
    // MUERTE
    // =========================

    void Morir()
    {
        if (muerto)
            return;

        muerto = true;

        StopAllCoroutines();

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (col != null)
            col.enabled = false;

        EnemigoIA ia = GetComponent<EnemigoIA>();

        if (ia != null)
        {
            ia.MorirAI();
        }

        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }

        Debug.Log("El Caballero Oscuro ha muerto.");
    }

    // =========================
    // DESTRUIR AL FINAL DE LA ANIMACIÓN
    // =========================

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
