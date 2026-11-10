using UnityEngine;
using UnityEngine.UI;
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

        // Empezamos totalmente en oscuro (pantalla negra) y nos desvanecemos
        StartCoroutine(FadeInNivel());
    }

    IEnumerator FadeInNivel()
    {
        canvasGroup.alpha = 1f;
        float tiempo = 0f;

        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime * velocidadFadeEntrada;
            canvasGroup.alpha = Mathf.Clamp01(1f - tiempo);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false); // Apagamos el panel negro cuando termina de mostrarse el nivel
    }
}