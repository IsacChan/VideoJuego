using System;
using System.Collections.Generic;

[Serializable]
public class DatosPartida
{
    public string nombreEscena;

    public float posicionX;
    public float posicionY;
    public float posicionZ;

    public int vidaActual;
    public int llavesActuales;

    public string fechaGuardado;
    public List<string> objetosCompletados =
        new List<string>();

    // Objetos obtenidos en InventoryManager
    public List<SlotGuardado> slotsInventario =
        new List<SlotGuardado>();

    // Estado/valor de correr (descomenta y ajusta cuando conectes tu variable)
    // public float energiaCorrer;
}