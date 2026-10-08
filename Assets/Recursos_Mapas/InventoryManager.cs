using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instancia;

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
        CrearSlots(20);
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

        // 1. Apilar si ya existe
        foreach (var slot in slots)
        {
            if (slot.itemActual == item && slot.cantidad < item.maxStack)
            {
                slot.cantidad += cantidad;
                slot.SendMessage("ActualizarUI", SendMessageOptions.DontRequireReceiver);
                return true;
            }
        }

        // 2. Buscar slot vacío
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