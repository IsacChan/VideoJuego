using UnityEngine;
using System.Collections;

public class TransicionNivelInicio : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float velocidadFade = 1.5f;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        StartCoroutine(FadeOutNegro());
    }

    IEnumerator FadeOutNegro()
    {
        // Empieza completamente en negro cubriendo la pantalla
        canvasGroup.alpha = 1f;
        float tiempo = 0f;

        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime * velocidadFade;
            canvasGroup.alpha = Mathf.Clamp01(1f - tiempo); // Va de 1 (negro) a 0 (transparente)
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false); // ¡Se apaga por completo para que puedas jugar sin que bloquee la pantalla!
    }
}