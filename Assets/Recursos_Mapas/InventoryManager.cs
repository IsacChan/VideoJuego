using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // <-- Importante para detectar el cambio de escena

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

    private void OnEnable()
    {
        // Nos suscribimos al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        InicializarInventario();
    }

    // Este método se ejecuta automáticamente cada vez que cargas una nueva escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si el contenedor se perdió por el cambio de escena, intentamos buscarlo de nuevo en el Canvas activo
        if (slotContainer == null)
        {
            GameObject panel = GameObject.Find("SlotContainer"); // O el nombre exacto de tu contenedor en la jerarquía
            if (panel != null) slotContainer = panel.transform;
        }

        InicializarInventario();
    }

    public void InicializarInventario()
    {
        if (slotContainer == null) return;

        // Guardamos temporalmente los datos que ya teníamos en los slots antes de limpiar
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

        // Limpiamos los slots visuales anteriores
        foreach (Transform hijo in slotContainer)
        {
            Destroy(hijo.gameObject);
        }

        slots.Clear();

        // Creamos los slots nuevos en la nueva escena
        CrearSlots(numeroDeSlots);

        // Devolvemos los ítems a sus slots correspondientes para que no se pierdan visualmente
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

        // 1. Apilar si ya existe
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