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
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject); // Hace que el inventario no se destruya al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Evita duplicados si por error hay otro manager en el siguiente nivel
        }
    }

    private void Start()
    {
        // 1. Limpiamos cualquier slot viejo o duplicado que pudiera haber quedado
        foreach (Transform hijo in slotContainer)
        {
            Destroy(hijo.gameObject);
        }

        // 2. Vaciamos la lista interna
        slots.Clear();

        // 3. Creamos los slots nuevos obligatoriamente al arrancar
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
                if (slot.cantidad < item.maxStack)
                {
                    slot.cantidad += cantidad;
                    slot.ActualizarUI();
                    return true;
                }
                else
                {
                    Debug.Log("Este slot ya alcanzó su límite máximo de stack.");
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