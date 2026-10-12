using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControladorBarraJefe : MonoBehaviour
{
    [Header("Referencias de UI")]
    public Image barraRoja;   // Arrastra el objeto BarraRoja aquí
    public Image barraTrail;  // Arrastra el objeto BarraTrail aquí

    [Header("Configuración del Efecto")]
    public float tiempoPausa = 0.4f;      // Tiempo que espera la barra blanca antes de retroceder
    public float velocidadSuavizado = 4f; // Qué tan rápido se desliza la barra blanca

    private float vidaMaxima;
    private float vidaActual;
    private Coroutine corrutinaTrail;

    // Llama a esta función al iniciar la pelea para llenar la barra al 100%
    public void InicializarVida(float vidaMax)
    {
        vidaMaxima = vidaMax;
        vidaActual = vidaMax;

        if (barraRoja != null) barraRoja.fillAmount = 1f;
        if (barraTrail != null) barraTrail.fillAmount = 1f;
    }

    // Llama a esta función cada vez que el jefe reciba daño
    public void RecibirDaño(float nuevaVida)
    {
        vidaActual = Mathf.Clamp(nuevaVida, 0, vidaMaxima);
        float porcentajeVidaActual = vidaActual / vidaMaxima;

        // 1. La barra roja baja DE INMEDIATO
        if (barraRoja != null)
        {
            barraRoja.fillAmount = porcentajeVidaActual;
        }

        // 2. Reiniciamos el efecto de la barra blanca de atraso
        if (corrutinaTrail != null)
        {
            StopCoroutine(corrutinaTrail);
        }

        if (gameObject.activeInHierarchy)
        {
            corrutinaTrail = StartCoroutine(AnimarBarraTrail(porcentajeVidaActual));
        }
    }

    private IEnumerator AnimarBarraTrail(float destinoFill)
    {
        // Espera un momento con el valor anterior
        yield return new WaitForSeconds(tiempoPausa);

        // Desliza suavemente la barra blanca hasta alcanzar la barra roja
        while (barraTrail != null && Mathf.Abs(barraTrail.fillAmount - destinoFill) > 0.001f)
        {
            barraTrail.fillAmount = Mathf.Lerp(barraTrail.fillAmount, destinoFill, Time.deltaTime * velocidadSuavizado);
            yield return null;
        }

        if (barraTrail != null)
        {
            barraTrail.fillAmount = destinoFill;
        }
    }
}