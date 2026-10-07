using UnityEngine;
using System.Collections;

public class SlashEffect : MonoBehaviour
{
    private LineRenderer line;

    [Header("Configuración del corte")]
    public float duracion = 0.20f;
    public float distancia = 1f;
    public float radio = 0.75f;
    public float anguloCorte = 120f;

    [Header("Forma")]
    public int puntos = 20;
    public int longitudEstela = 7;

    void Awake()
    {
        line = GetComponent<LineRenderer>();

        line.useWorldSpace = false;
        line.positionCount = 0;
    }

    public void Mostrar(Vector2 direccion)
    {
        StopAllCoroutines();

        gameObject.SetActive(true);

        StartCoroutine(AnimarCorte(direccion));
    }

    private IEnumerator AnimarCorte(Vector2 direccion)
    {
        direccion.Normalize();

        float anguloDireccion =
            Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

        // Norte y Sur mantienen el sentido actual.
        // Este y Oeste invierten el recorrido.
        bool ataqueHorizontal =
            Mathf.Abs(direccion.x) > Mathf.Abs(direccion.y);

        float sentido = ataqueHorizontal ? -1f : 1f;

        float anguloInicial =
            anguloDireccion - (anguloCorte / 2f) * sentido;

        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            float progreso =
                Mathf.Clamp01(tiempo / duracion);

            int puntoActual =
                Mathf.RoundToInt(progreso * (puntos - 1));

            int puntoInicial =
                Mathf.Max(0, puntoActual - longitudEstela);

            int cantidad =
                puntoActual - puntoInicial + 1;

            line.positionCount = cantidad;

            for (int i = 0; i < cantidad; i++)
            {
                int indice = puntoInicial + i;

                float t =
                    indice / (float)(puntos - 1);

                float angulo =
                    anguloInicial + (anguloCorte * t * sentido);

                float radianes =
                    angulo * Mathf.Deg2Rad;

                Vector2 punto = new Vector2(
                    Mathf.Cos(radianes),
                    Mathf.Sin(radianes)
                );

                punto *= radio;

                punto += direccion * distancia;

                line.SetPosition(i, punto);
            }

            yield return null;
        }

        line.positionCount = 0;
        gameObject.SetActive(false);
    }
}