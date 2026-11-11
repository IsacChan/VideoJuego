
using UnityEngine;
using UnityEngine.UI;

public class ControladorCarrera : MonoBehaviour
{
    [Header("Sistema de Carrera")]
    public caminar jugador;

    [Header("Indicador Circular")]
    public Image rellenoCarrera;

    private void Update()
    {
        if (jugador == null || rellenoCarrera == null)
            return;

        rellenoCarrera.fillAmount = jugador.ProgresoCarrera;
    }
}
