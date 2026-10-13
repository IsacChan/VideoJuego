
using UnityEngine;
using UnityEngine.UI;

public class ControladorOrb : MonoBehaviour
{
    [Header("Sistema de Embestida")]
    public EmbestidaJugador embestida;

    [Header("Indicador Circular")]
    public Image rellenoOrb;

    private void Update()
    {
        if (embestida == null || rellenoOrb == null)
            return;

        // Actualizar el relleno circular
        rellenoOrb.fillAmount = embestida.ProgresoRecarga;
    }
}

