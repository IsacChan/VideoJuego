
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CargadorPartida : MonoBehaviour
{
    
public void CargarPartida()
{
    // 1. Leer la partida guardada
    DatosPartida datos = SistemaGuardado.Cargar();

    if (datos == null)
    {
        Debug.LogWarning(
            "No existe una partida válida."
        );
        return;
    }

    // 2. Restaurar el progreso del mundo
    // ANTES de cargar la escena
    ProgresoMundo.RestaurarProgreso(
        datos.objetosCompletados
    );

    Debug.Log(
        "Cargando partida: " + datos.nombreEscena
    );

    // 3. Evitar interferencia de los portales
    DatosTransporte.puntoAparicion = "";

    // 4. Crear el restaurador temporal
    GameObject objetoCarga =
        new GameObject("RestauradorPartida");

    RestauradorPartida restaurador =
        objetoCarga.AddComponent<RestauradorPartida>();

    // 5. Cargar escena y restaurar jugador
    restaurador.IniciarCarga(datos);
}

}

public class RestauradorPartida : MonoBehaviour
{
    private DatosPartida datosGuardados;

    public void IniciarCarga(DatosPartida datos)
    {
        datosGuardados = datos;

        DontDestroyOnLoad(gameObject);

        StartCoroutine(CargarYRestaurar());
    }

    private IEnumerator CargarYRestaurar()
    {
        Time.timeScale = 1f;

        // Cargar mapa guardado
        AsyncOperation carga =
            SceneManager.LoadSceneAsync(
                datosGuardados.nombreEscena
            );

        if (carga == null)
        {
            Debug.LogError(
                "No se pudo cargar la escena guardada."
            );
            Destroy(gameObject);
            yield break;
        }

        while (!carga.isDone)
            yield return null;

        // Esperar a que terminen Start()
        yield return null;

        // Buscar jugador
        caminar jugador =
            FindFirstObjectByType<caminar>();

        if (jugador == null)
        {
            Debug.LogError(
                "No se encontró el jugador."
            );
            Destroy(gameObject);
            yield break;
        }

        // Restaurar posición
        Vector3 posicion = new Vector3(
            datosGuardados.posicionX,
            datosGuardados.posicionY,
            datosGuardados.posicionZ
        );

        Rigidbody2D rb =
            jugador.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = new Vector2(
                posicion.x,
                posicion.y
            );
        }

        jugador.transform.position = posicion;

        Physics2D.SyncTransforms();

        
        // Evitar abrir la ventana de guardado
        // al reaparecer encima de un checkpoint
        Checkpoint[] checkpoints =
            FindObjectsByType<Checkpoint>(
                FindObjectsSortMode.None
            );

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint == null)
                continue;

            Collider2D zona =
                checkpoint.GetComponent<Collider2D>();

            Collider2D colliderJugador =
                jugador.GetComponent<Collider2D>();

            if (zona != null &&
                colliderJugador != null &&
                zona.bounds.Intersects(colliderJugador.bounds))
            {
                checkpoint.IgnorarEntradaInicial();
            }
        }


        // Restaurar checkpoint
        VidaJugador vida =
            jugador.GetComponent<VidaJugador>();

        if (vida != null)
        {
            vida.ActivarCheckpoint(posicion);
        }

        // Restaurar corazones
        jugador.RestaurarVidaGuardada(
            datosGuardados.vidaActual
        );

        // Restaurar llaves
        InventarioJugador inventario =
            FindFirstObjectByType<InventarioJugador>();

        if (inventario != null)
        {
            inventario.RestaurarLlaves(
                datosGuardados.llavesActuales
            );
        }
        else
        {
            Debug.LogWarning(
                "No se encontró InventarioJugador."
            );
        }

        Debug.Log(
            "PARTIDA RESTAURADA CORRECTAMENTE\n" +
            "Posición: " + posicion + "\n" +
            "Vida: " + jugador.vidaActual + "\n" +
            "Llaves: " +
            (inventario != null
                ? inventario.llavesActuales
                : -1)
        );

        Destroy(gameObject);
    }
}
