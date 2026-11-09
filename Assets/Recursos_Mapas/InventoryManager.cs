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
        // Limpiamos cualquier slot anterior o duplicado al arrancar la escena
        foreach (Transform hijo in slotContainer)
        {
            Destroy(hijo.gameObject);
        }

        slots.Clear();

        // Creamos los 10 slots obligatorios desde código
        CrearSlots(numeroDeSlots);
    }

    public void CrearSlots(int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            GameObject nuevo = Instantiate(slotPrefab, slotContainer);
            
            // Forzamos la escala local a 1 para evitar deformaciones en la UI
            nuevo.transform.localScale = Vector3.one;

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