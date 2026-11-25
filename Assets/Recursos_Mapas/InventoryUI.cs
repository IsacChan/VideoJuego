using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject inventoryPanel; // Arrastra aquí tu InventoryPanel desde la Jerarquía
    
    [Header("Configuración")]
    public KeyCode toggleKey = KeyCode.I; // Tecla para abrir/cerrar

    private bool isOpen = false;

    void Start()
    {
        isOpen = false;

        // Asegura que el inventario comience cerrado al iniciar el juego
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        BarraVidaFlotante.ConfigurarVisibilidadInventario(false);
    }

    void Update()
    {
        // Si presionas la tecla, cambia el estado (si está abierto se cierra, y viceversa)
        if (Input.GetKeyDown(toggleKey))
        {
            EstablecerEstadoInventario(!isOpen);
        }
    }

    public void EstablecerEstadoInventario(bool abrir)
    {
        isOpen = abrir;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isOpen);
        }

        // Oculta o muestra las barras flotantes de los enemigos según el inventario
        BarraVidaFlotante.ConfigurarVisibilidadInventario(isOpen);

        // Pausar o reanudar el juego según el estado del inventario
        Time.timeScale = isOpen ? 0f : 1f;
    }

    // Útil si tienes un botón "X" para cerrar el inventario
    public void CerrarInventario()
    {
        EstablecerEstadoInventario(false);
    }

    void OnDisable()
    {
        // Seguridad por si se desactiva este objeto mientras el inventario estaba abierto
        if (isOpen)
        {
            isOpen = false;
            BarraVidaFlotante.ConfigurarVisibilidadInventario(false);
            Time.timeScale = 1f;
        }
    }
}