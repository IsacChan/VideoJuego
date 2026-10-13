using UnityEngine;
using UnityEngine.UI;

public class ControladorBarraJefe : MonoBehaviour
{
    [Header("Referencia del Slider")]
    [SerializeField] private Slider sliderPrincipal; // El componente Slider principal (que incluye su Fill Area)

    /// <summary>
    /// Configura la vida inicial del jefe al empezar el combate
    /// </summary>
    public void InicializarVida(float vidaMax)
    {
        if (sliderPrincipal != null)
        {
            sliderPrincipal.maxValue = vidaMax;
            sliderPrincipal.value = vidaMax;
        }
    }

    /// <summary>
    /// Actualiza el valor de la barra cuando el jefe recibe daño
    /// </summary>
    public void RecibirDaño(float nuevaVida)
    {
        if (sliderPrincipal != null)
        {
            float vidaClamped = Mathf.Clamp(nuevaVida, 0, sliderPrincipal.maxValue);
            sliderPrincipal.value = vidaClamped; // El Slider baja de manera fluida y directa en su Fill Area
        }
    }
}