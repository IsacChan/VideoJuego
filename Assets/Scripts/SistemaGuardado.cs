
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public static class SistemaGuardado
{
    private static string RutaGuardado =>
        Path.Combine(
            Application.persistentDataPath,
            "partida.json"
        );

    
public static void Guardar(
    Vector3 posicion,
    int vidaActual,
    int llavesActuales)
{
    DatosPartida datos = new DatosPartida();

    datos.nombreEscena =
        SceneManager.GetActiveScene().name;

    datos.posicionX = posicion.x;
    datos.posicionY = posicion.y;
    datos.posicionZ = posicion.z;

    datos.vidaActual = vidaActual;
    datos.llavesActuales = llavesActuales;

    datos.objetosCompletados =
    ProgresoMundo.ObtenerProgreso();

    datos.fechaGuardado =
        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    string json = JsonUtility.ToJson(datos, true);

    try
    {
        string rutaTemporal = RutaGuardado + ".tmp";

        File.WriteAllText(rutaTemporal, json);

        // Sobrescribir partida anterior
        if (File.Exists(RutaGuardado))
        {
            File.Delete(RutaGuardado);
        }

        // Guardar nueva partida
        File.Move(rutaTemporal, RutaGuardado);

        Debug.Log(
            "Partida guardada correctamente en: "
            + RutaGuardado
        );
    }
    catch (Exception e)
    {
        Debug.LogError(
            "Error al guardar partida: " + e.Message
        );

        throw;
    }
}


    public static bool ExistePartida()
    {
        return File.Exists(RutaGuardado);
    }

    public static DatosPartida Cargar()
    {
        if (!ExistePartida())
        {
            Debug.LogWarning("No existe una partida guardada.");
            return null;
        }

        try
        {
            string json = File.ReadAllText(RutaGuardado);
            DatosPartida datos =
                JsonUtility.FromJson<DatosPartida>(json);

            if (datos == null ||
                string.IsNullOrEmpty(datos.nombreEscena))
            {
                Debug.LogError("Archivo de partida inválido.");
                return null;
            }
Debug.Log("JSON CARGADO:\n" + json);
            return datos;
            
        }
        catch (Exception e)
        {
            Debug.LogError("Error al cargar: " + e.Message);
            return null;
        }
    }
}
