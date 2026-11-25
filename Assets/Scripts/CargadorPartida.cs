using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CargadorPartida : MonoBehaviour
{
    [Header("UI Persistente")]
    [Tooltip("Arrastra aquí tu Prefab del HUD si aún no existe en la escena")]
    public GameObject prefabHUD;

    [Tooltip("Arrastra aquí tu Prefab del Inventario (InventoryManager) si aún no existe en la escena")]
    public GameObject prefabInventario;

    public void CargarPartida()
    {
        DatosPartida datos = SistemaGuardado.Cargar();

        if (datos == null)
        {
            Debug.LogWarning(
                "No existe una partida válida."
            );
            return;
        }

        // Crear el HUD si aún no existe para que viaje a la escena guardada
        if (prefabHUD != null && FindFirstObjectByType<ControladorCorazones>(FindObjectsInactive.Include) == null)
        {
            GameObject uiHUD = Instantiate(prefabHUD);
            uiHUD.name = prefabHUD.name;
            DontDestroyOnLoad(uiHUD);
        }

        // Crear el Inventario si aún no existe para que viaje a la escena guardada
        if (prefabInventario != null && FindFirstObjectByType<InventoryManager>(FindObjectsInactive.Include) == null)
        {
            GameObject uiInventario = Instantiate(prefabInventario);
            uiInventario.name = prefabInventario.name;
            DontDestroyOnLoad(uiInventario);
        }

        ProgresoMundo.RestaurarProgreso(
            datos.objetosCompletados
        );

        Debug.Log(
            "Cargando partida: " + datos.nombreEscena
        );

        DatosTransporte.puntoAparicion = "";

        GameObject objetoCarga =
            new GameObject("RestauradorPartida");

        RestauradorPartida restaurador =
            objetoCarga.AddComponent<RestauradorPartida>();

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

        yield return null;

        caminar jugador =
            FindFirstObjectByType<caminar>(FindObjectsInactive.Include);

        if (jugador == null)
        {
            Debug.LogError(
                "No se encontró el jugador."
            );
            Destroy(gameObject);
            yield break;
        }

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

        Checkpoint[] checkpoints =
            FindObjectsByType<Checkpoint>(
                FindObjectsInactive.Include,
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

        VidaJugador vida =
            jugador.GetComponent<VidaJugador>();

        if (vida != null)
        {
            vida.ActivarCheckpoint(posicion);
        }

        // Restaurar vida exacta (aunque esté baja)
        jugador.RestaurarVidaGuardada(
            datosGuardados.vidaActual
        );

        // Restaurar llaves
        InventarioJugador inventario =
            FindFirstObjectByType<InventarioJugador>(FindObjectsInactive.Include);

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

        // Restaurar ítems obtenidos en InventoryManager
        InventoryManager invManager = InventoryManager.instancia != null
            ? InventoryManager.instancia
            : FindFirstObjectByType<InventoryManager>(FindObjectsInactive.Include);

        if (invManager != null && PlayerPrefs.HasKey("SlotsInventarioGuardados"))
        {
            var contenedor = JsonUtility.FromJson<ContenedorSlotsPartida>(
                PlayerPrefs.GetString("SlotsInventarioGuardados")
            );
            if (contenedor != null && contenedor.slots != null)
            {
                invManager.RestaurarSlotsGuardados(contenedor.slots);
            }
        }

        // Restaurar estado/valor de correr tal como quedó guardado (sin rellenarlo)
        // Ejemplo si usas PlayerPrefs:
        // if (PlayerPrefs.HasKey("EnergiaCorrerGuardada"))
        //     jugador.energiaActual = PlayerPrefs.GetFloat("EnergiaCorrerGuardada");

        Canvas[] canvases =
            FindObjectsByType<Canvas>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (Canvas c in canvases)
        {
            if (c != null && !c.gameObject.activeSelf)
            {
                if (c.gameObject.name.Contains("HUD") ||
                    c.gameObject.name.Contains("Canvas"))
                {
                    c.gameObject.SetActive(true);
                }
            }
        }

        ControladorCorazones corazones =
            FindFirstObjectByType<ControladorCorazones>(FindObjectsInactive.Include);

        if (corazones != null)
        {
            corazones.gameObject.SetActive(true);
            corazones.ActualizarVida(jugador.vidaActual, false);
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