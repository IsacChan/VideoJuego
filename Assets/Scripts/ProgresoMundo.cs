
using System.Collections.Generic;
using UnityEngine;

public static class ProgresoMundo
{
    // Guarda los identificadores de las salas,
    // cofres y objetos completados.
    private static HashSet<string> objetosCompletados =
        new HashSet<string>();

    // Registrar una sala completada o un objeto recogido
    public static void Registrar(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning(
                "No se puede registrar un ID vacío."
            );
            return;
        }

        if (objetosCompletados.Add(id))
        {
            Debug.Log(
                "Progreso registrado: " + id
            );
        }
    }

    // Comprobar si ya fue completado
    public static bool EstaCompletado(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        return objetosCompletados.Contains(id);
    }

    // Obtener el progreso para guardarlo en JSON
    public static List<string> ObtenerProgreso()
    {
        return new List<string>(objetosCompletados);
    }

    // Restaurar el progreso desde JSON
    public static void RestaurarProgreso(
        List<string> datos)
    {
        objetosCompletados = datos != null
            ? new HashSet<string>(datos)
            : new HashSet<string>();

        Debug.Log(
            "Progreso restaurado. Elementos: " +
            objetosCompletados.Count
        );
    }

    // Limpiar progreso al iniciar una partida nueva
    public static void Reiniciar()
    {
        objetosCompletados.Clear();

        Debug.Log("Progreso del mundo reiniciado.");
    }
}

