
using System;
using UnityEngine;
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
}

