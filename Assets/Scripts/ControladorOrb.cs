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

        rellenoOrb.fillAmount = embestida.ProgresoRecarga;
    }
}