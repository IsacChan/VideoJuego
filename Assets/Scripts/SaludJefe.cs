using UnityEngine;
using UnityEngine.UI; // Necesario para trabajar con el Slider

public class SaludJefe : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public float vidaMaxima = 100f;
    public float vidaActual;

    [Header("Referencias de Interfaz")]
    public Slider barraVidaSlider; // Arrastra aquí la BarraVidaJefe desde la Jerarquía

    void Start()
    {
        // Inicializamos la vida al máximo al comenzar
        vidaActual = vidaMaxima;

        if (barraVidaSlider != null)
        {
            barraVidaSlider.maxValue = vidaMaxima;
            barraVidaSlider.value = vidaActual;
        }
    }

    // Método para restar vida (puedes llamarlo cuando el jugador golpee al jefe)
    public void RecibirDanio(float cantidad)
    {
        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima); // Evita que baje de 0

        // Actualizamos visualmente el Slider
        if (barraVidaSlider != null)
        {
            barraVidaSlider.value = vidaActual;
        }

        // Comprobamos si el jefe se quedó sin vida
        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        Debug.Log("¡El jefe ha sido derrotado!");
        // Aquí puedes agregar efectos, abrir un portal o destruir el objeto del jefe
        Destroy(gameObject);
    }
}