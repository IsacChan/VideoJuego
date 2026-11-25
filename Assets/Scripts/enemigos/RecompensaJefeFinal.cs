
using UnityEngine;
using System.Collections;

public class RecompensaJefeFinal : MonoBehaviour
{
    [Header("Piedras mágicas")]
    [SerializeField] private ItemData piedra1;
    [SerializeField] private ItemData piedra2;
    [SerializeField] private ItemData piedra3;

    [Header("Portal")]
    [SerializeField] private GameObject portalFinal;

    [Header("Progreso")]
    [SerializeField] private string idJefeDerrotado =
        "jefe_final_derrotado";

    private bool procesandoDerrota = false;
    private bool derrotaProcesada = false;

    private void Start()
    {
        if (piedra1 == null || piedra2 == null || piedra3 == null ||
            portalFinal == null)
        {
            Debug.LogError("Faltan referencias en RecompensaJefeFinal.");
            enabled = false;
            return;
        }

        if (piedra1 == piedra2 || piedra1 == piedra3 ||
            piedra2 == piedra3)
        {
            Debug.LogError("Las tres piedras deben ser diferentes.");
            enabled = false;
            return;
        }

        portalFinal.SetActive(false);

        if (ProgresoMundo.EstaCompletado(idJefeDerrotado))
        {
            derrotaProcesada = true;
            portalFinal.SetActive(true);

            Debug.Log("El jefe ya fue derrotado. Portal activado.");
        }
    }

    public void NotificarDerrota()
    {
        if (derrotaProcesada || procesandoDerrota)
            return;

        StartCoroutine(EntregarRecompensa());
    }

    private IEnumerator EntregarRecompensa()
    {
        procesandoDerrota = true;

        while (InventoryManager.instancia == null)
            yield return null;

        bool recuperada1 = false;
        bool recuperada2 = false;
        bool recuperada3 = false;

        while (!recuperada1 || !recuperada2 || !recuperada3)
        {
            if (!recuperada1)
                recuperada1 =
                    InventoryManager.instancia.AñadirItem(piedra1, 1);

            if (!recuperada2)
                recuperada2 =
                    InventoryManager.instancia.AñadirItem(piedra2, 1);

            if (!recuperada3)
                recuperada3 =
                    InventoryManager.instancia.AñadirItem(piedra3, 1);

            if (!recuperada1 || !recuperada2 || !recuperada3)
            {
                Debug.LogWarning(
                    "Inventario lleno. Libera espacio para recuperar las piedras."
                );

                yield return new WaitForSecondsRealtime(1f);
            }
        }

        // Registrar la derrota después de devolver las tres piedras.
        ProgresoMundo.Registrar(idJefeDerrotado);

        derrotaProcesada = true;
        procesandoDerrota = false;

        portalFinal.SetActive(true);

        Debug.Log("¡Piedras recuperadas! El portal final está disponible.");
    }
}
