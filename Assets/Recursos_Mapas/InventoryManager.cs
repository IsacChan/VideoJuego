using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct SlotGuardado
{
    public string nombreItem;
    public int cantidad;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instancia;

    [Header("Configuración")]
    [Tooltip("Número total de casillas que tendrá el inventario")]
    public int numeroDeSlots = 10;

    [Header("Referencias")]
    public Transform slotContainer;
    public GameObject slotPrefab;

    [Header("Catálogo de ítems para cargar partida")]
    [Tooltip("Arrastra aquí todos los ItemData de tu juego para poder recrearlos al cargar")]
    public List<ItemData> catalogoItems = new List<ItemData>();

    private List<InventoryItem> slots = new List<InventoryItem>();

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else if (instancia != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (slots.Count == 0)
        {
            InicializarInventario();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (slotContainer == null)
        {
            GameObject panel = GameObject.Find("SlotContainer");
            if (panel != null) slotContainer = panel.transform;
        }

        // Asegurar que el Canvas hijo (si existe) esté activo al entrar al mapa
        Canvas canvasHijo = GetComponentInChildren<Canvas>(true);
        if (canvasHijo != null)
        {
            canvasHijo.gameObject.SetActive(true);
        }

        if (slots.Count == 0)
        {
            InicializarInventario();
        }
    }

    public void InicializarInventario()
    {
        if (slotContainer == null) return;

        List<ItemData> itemsGuardados = new List<ItemData>();
        List<int> cantidadesGuardadas = new List<int>();

        foreach (var slot in slots)
        {
            if (slot != null && slot.itemActual != null)
            {
                itemsGuardados.Add(slot.itemActual);
                cantidadesGuardadas.Add(slot.cantidad);
            }
            else
            {
                itemsGuardados.Add(null);
                cantidadesGuardadas.Add(0);
            }
        }

        foreach (Transform hijo in slotContainer)
        {
            Destroy(hijo.gameObject);
        }

        slots.Clear();
        CrearSlots(numeroDeSlots);

        for (int i = 0; i < Mathf.Min(itemsGuardados.Count, slots.Count); i++)
        {
            if (itemsGuardados[i] != null)
            {
                slots[i].AsignarItem(itemsGuardados[i], cantidadesGuardadas[i]);
            }
        }
    }

    public void CrearSlots(int cantidad)
    {
        if (slotPrefab == null || slotContainer == null) return;

        for (int i = 0; i < cantidad; i++)
        {
            GameObject nuevo = Instantiate(slotPrefab, slotContainer);
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
            }
        }

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

    public List<SlotGuardado> ObtenerSlotsGuardados()
    {
        List<SlotGuardado> lista = new List<SlotGuardado>();
        foreach (var slot in slots)
        {
            if (slot != null && slot.itemActual != null && slot.cantidad > 0)
            {
                lista.Add(new SlotGuardado
                {
                    nombreItem = slot.itemActual.nombreItem,
                    cantidad = slot.cantidad
                });
            }
            else
            {
                lista.Add(new SlotGuardado
                {
                    nombreItem = "",
                    cantidad = 0
                });
            }
        }
        return lista;
    }

    public void RestaurarSlotsGuardados(List<SlotGuardado> datosSlots)
    {
        if (datosSlots == null || slotContainer == null) return;

        if (slots.Count != numeroDeSlots)
        {
            slots.Clear();
            foreach (Transform hijo in slotContainer)
            {
                Destroy(hijo.gameObject);
            }
            CrearSlots(numeroDeSlots);
        }

        for (int i = 0; i < Mathf.Min(datosSlots.Count, slots.Count); i++)
        {
            slots[i].itemActual = null;
            slots[i].cantidad = 0;
            slots[i].ActualizarUI();

            string nombreBuscado = datosSlots[i].nombreItem;
            int cant = datosSlots[i].cantidad;

            if (!string.IsNullOrEmpty(nombreBuscado) && cant > 0)
            {
                ItemData encontrado = catalogoItems.Find(x => x != null && x.nombreItem == nombreBuscado);
                if (encontrado != null)
                {
                    slots[i].AsignarItem(encontrado, cant);
                }
            }
        }
    }


public int ObtenerCantidadItem(ItemData item)
{
    if (item == null)
        return 0;

    int total = 0;

    foreach (InventoryItem slot in slots)
    {
        if (slot != null && slot.itemActual == item)
        {
            total += slot.cantidad;
        }
    }

    return total;
}

public bool EliminarCantidadItem(ItemData item, int cantidadEliminar)
{
    if (item == null || cantidadEliminar <= 0)
        return false;

    int disponible = ObtenerCantidadItem(item);

    if (disponible < cantidadEliminar)
    {
        Debug.LogWarning("No hay suficientes unidades de " + item.nombreItem);
        return false;
    }

    int restante = cantidadEliminar;

    foreach (InventoryItem slot in slots)
    {
        if (slot == null || slot.itemActual != item)
            continue;

        int cantidadQuitar = Mathf.Min(slot.cantidad, restante);

        slot.cantidad -= cantidadQuitar;
        restante -= cantidadQuitar;

        if (slot.cantidad <= 0)
        {
            slot.LimpiarSlot();
        }
        else
        {
            slot.ActualizarUI();
        }

        if (restante <= 0)
            break;
    }

    InventoryNavigation navegacion =
        FindFirstObjectByType<InventoryNavigation>();

    if (navegacion != null)
        navegacion.ActualizarPanelVisual();

    return true;
}

}