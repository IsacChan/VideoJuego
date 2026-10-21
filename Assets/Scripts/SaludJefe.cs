using UnityEngine;
using UnityEngine.UI;

public class SaludJefe : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public float vidaMaxima = 100f;
    public float vidaActual;

    [Header("Referencias de Interfaz")]
    public Slider barraVidaSlider;

    [Header("Estado del Jefe")]
    public bool estaMuerto = false;

    [Header("Sala del jefe")]
public SalaJefe salaJefe;

    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        vidaActual = vidaMaxima;

        if (barraVidaSlider != null)
        {
            barraVidaSlider.maxValue = vidaMaxima;
            barraVidaSlider.value = vidaActual;
        }
    }

    public void RecibirDanio(float cantidad)
    {
        if (estaMuerto)
            return;

        vidaActual -= cantidad;

        vidaActual = Mathf.Clamp(
            vidaActual,
            0,
            vidaMaxima
        );

        if (barraVidaSlider != null)
        {
            barraVidaSlider.value = vidaActual;
        }

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
{
    if (estaMuerto)
        return;

    estaMuerto = true;
    vidaActual = 0f;

    Debug.Log("¡El jefe ha sido derrotado!");

    if (rb != null)
    {
        rb.linearVelocity = Vector2.zero;
    }

    if (col != null)
    {
        col.enabled = false;
    }

    if (animator != null)
    {
        animator.SetBool("IsDead", true);
    }

    // El jefe permanece muerto en el mapa.

    if (salaJefe != null)
{
    salaJefe.NotificarJefeDerrotado();
}
else
{
    Debug.LogWarning("SaludJefe no tiene una SalaJefe asignada.");
}
}
}