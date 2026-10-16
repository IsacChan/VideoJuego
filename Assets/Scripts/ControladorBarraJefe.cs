using UnityEngine;
using UnityEngine.UI;

public class ControladorBarraJefe : MonoBehaviour
{
    [Header("Componente Slider")]
    public Slider sliderVida;

    // Inicializa el valor máximo y actual de la barra al empezar el juego
    public void InicializarVida(int vidaMaxima)
    {
        if (sliderVida != null)
        {
            sliderVida.maxValue = vidaMaxima;
            sliderVida.value = vidaMaxima;
        }
    }

    // Actualiza la barra cuando el jefe recibe daño
    public void RecibirDaño(int vidaActual)
    {
        if (sliderVida != null)
        {
            sliderVida.value = vidaActual;
        }
    }
}