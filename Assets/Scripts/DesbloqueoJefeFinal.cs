
using UnityEngine;
using System.Collections;

public class DesbloqueoJefeFinal : MonoBehaviour
{
    [Header("Piedras mágicas necesarias")]
    [SerializeField] private ItemData piedra1;
    [SerializeField] private ItemData piedra2;
    [SerializeField] private ItemData piedra3;

    [Header("Referencias")]
    [SerializeField] private GameObject jefeFinal;
    [SerializeField] private GameObject portalFinal;

    [Header("Progreso")]
    [SerializeField] private string idDesbloqueo =
        "jefe_final_desbloqueado";

    [SerializeField] private string idJefeDerrotado =
        "jefe_final_derrotado";

    private bool jefeDesbloqueado = false;

    private IEnumerator Start()
    {
        if (piedra1 == null || piedra2 == null || piedra3 == null)
        {
            Debug.LogError("Falta asignar alguna piedra mágica.");
            enabled = false;
            yield break;
        }

        if (piedra1 == piedra2 || piedra1 == piedra3 ||
            piedra2 == piedra3)
        {
            Debug.LogError("Las tres piedras deben ser diferentes.");
            enabled = false;
            yield break;
        }

        if (jefeFinal == null || portalFinal == null)
        {
            Debug.LogError("Falta asignar el jefe final o el portal.");
            enabled = false;
            yield break;
        }

        jefeFinal.SetActive(false);
        portalFinal.SetActive(false);

        while (InventoryManager.instancia == null)
            yield return null;

        // Si el jefe ya fue derrotado, no volver a activarlo.
        if (ProgresoMundo.EstaCompletado(idJefeDerrotado))
        {
            jefeDesbloqueado = true;
            portalFinal.SetActive(true);

            Debug.Log("El jefe ya fue derrotado. Portal disponible.");
            yield break;
        }

        // Si ya se desbloqueó anteriormente, activar al jefe.
        if (ProgresoMundo.EstaCompletado(idDesbloqueo))
        {
            ActivarJefe();
        }
    }

    private void Update()
    {
        if (jefeDesbloqueado)
            return;

        if (InventoryManager.instancia == null)
            return;

        if (TieneLasTresPiedras())
        {
            DesbloquearJefe();
        }
    }

    private bool TieneLasTresPiedras()
    {
        return
            InventoryManager.instancia.ObtenerCantidadItem(piedra1) >= 1 &&
            InventoryManager.instancia.ObtenerCantidadItem(piedra2) >= 1 &&
            InventoryManager.instancia.ObtenerCantidadItem(piedra3) >= 1;
    }

    private void DesbloquearJefe()
    {
        if (jefeDesbloqueado || !TieneLasTresPiedras())
            return;

        // Eliminar las tres piedras.
        bool eliminada1 =
            InventoryManager.instancia.EliminarCantidadItem(piedra1, 1);

        bool eliminada2 =
            InventoryManager.instancia.EliminarCantidadItem(piedra2, 1);

        bool eliminada3 =
            InventoryManager.instancia.EliminarCantidadItem(piedra3, 1);

        if (!eliminada1 || !eliminada2 || !eliminada3)
        {
            Debug.LogError("No se pudieron entregar todas las piedras.");
            return;
        }

        ProgresoMundo.Registrar(idDesbloqueo);

        ActivarJefe();

        Debug.Log("¡Las tres piedras fueron entregadas!");
    }

    private void ActivarJefe()
    {
        jefeDesbloqueado = true;
        jefeFinal.SetActive(true);
    }
}
