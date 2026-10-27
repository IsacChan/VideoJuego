using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instancia;

    [Header("Configuración")]
    [Tooltip("Número total de casillas que tendrá el inventario")]
    public int numeroDeSlots = 10;

    [Header("Referencias")]
    public Transform slotContainer;
    public GameObject slotPrefab;

    private List<InventoryItem> slots = new List<InventoryItem>();

    private void Awake()
    {
        if (instancia == null) instancia = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        CrearSlots(numeroDeSlots);
    }

    public void CrearSlots(int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            GameObject nuevo = Instantiate(slotPrefab, slotContainer);
            InventoryItem item = nuevo.GetComponent<InventoryItem>();
            if (item != null) slots.Add(item);
        }
    }

    public bool AñadirItem(ItemData item, int cantidad = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("Intentaste añadir un item null");
            return false;
        }

        // 1. Apilar si ya existe (comparando por nombre del item y validando maxStack)
        foreach (var slot in slots)
        {
            if (slot.itemActual != null && slot.itemActual.nombreItem == item.nombreItem)
            {
                // Comprobamos que no rebase el límite máximo de apilado
                if (slot.cantidad < item.maxStack)
                {
                    slot.cantidad += cantidad;
                    slot.ActualizarUI(); // Llamada directa limpia y segura
                    return true;
                }
                else
                {
                    Debug.Log("Este slot ya alcanzó su límite máximo de stack.");
                    // Si alcanzó el límite, el flujo continuará para buscar otro slot vacío
                }
            }
        }

        // 2. Buscar slot vacío si no se pudo apilar
        foreach (var slot in slots)
        {
            if (slot.itemActual == null)
            {
                slot.AsignarItem(item, cantidad);
                return true;
            }
        }

        Debug.Log("¡Inventario lleno!");
        return false;
    }
}