using UnityEngine;
using System.Collections;

public class SlashEffect : MonoBehaviour
{
    private LineRenderer line;

    public float duracion = 0.15f;
    public float distancia = 1f;

    void Awake()
    {
        line = GetComponent<LineRenderer>();

        line.positionCount = 7;
        line.useWorldSpace = false;
    }

    public void Mostrar(Vector2 direccion)
    {
        StopAllCoroutines();

        gameObject.SetActive(true);

        StartCoroutine(CrearCorte(direccion));
    }

    private IEnumerator CrearCorte(Vector2 direccion)
    {
        direccion = direccion.normalized;

        Vector2 perpendicular = new Vector2(
            -direccion.y,
            direccion.x
        );

        Vector2 centro = direccion * distancia;

        line.positionCount = 7;

        for (int i = 0; i < 7; i++)
        {
            float t = i / 6f;

            float curva = Mathf.Sin(t * Mathf.PI);

            Vector2 punto =
                centro
                + perpendicular * ((t - 0.5f) * 0.9f)
                + direccion * (curva * 0.15f);

            line.SetPosition(i, punto);
        }

        yield return new WaitForSeconds(duracion);

        gameObject.SetActive(false);
    }
}
