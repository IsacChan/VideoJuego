using UnityEngine;
using System.Collections;

public class TransicionEntrada : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float velocidadFadeEntrada = 1.5f;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Empezamos totalmente transparentes u ocultos y nos desvanecemos hacia la visibilidad
        StartCoroutine(FadeInPanel());
    }

    IEnumerator FadeInPanel()
    {
        canvasGroup.alpha = 0f; // Empieza invisible
        float tiempo = 0f;

        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime * velocidadFadeEntrada;
            canvasGroup.alpha = Mathf.Clamp01(tiempo); // Sube suavemente de 0 a 1
            yield return null;
        }

        canvasGroup.alpha = 1f; // Se queda completamente visible (¡ya no se apaga!)
    }
}