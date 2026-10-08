
using UnityEngine;
using System.Collections;

public class PuntoAparicion : MonoBehaviour
{
    [Header("Identificador")]
    public string identificador = "EntradaMapa2";

    private IEnumerator Start()
    {
        Debug.Log(
            "Destino guardado: " +
            DatosTransporte.puntoAparicion
        );

        Debug.Log(
            "Identificador del punto: " +
            identificador
        );

        if (DatosTransporte.puntoAparicion != identificador)
        {
            Debug.LogWarning(
                "Este no es el punto de aparición solicitado."
            );

            yield break;
        }

        // Esperar a que termine la inicialización
        yield return null;

        caminar jugador = FindFirstObjectByType<caminar>();

        if (jugador == null)
        {
            Debug.LogError(
                "No se encontró al jugador en Mapa 2."
            );

            yield break;
        }

        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();

        Vector3 posicionDestino = transform.position;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = new Vector2(
                posicionDestino.x,
                posicionDestino.y
            );
        }

        jugador.transform.position = posicionDestino;

        Physics2D.SyncTransforms();

        Debug.Log(
            "Jugador colocado en: " +
            posicionDestino
        );

        DatosTransporte.puntoAparicion = "";
    }
}
