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
        // Asegura que el inventario comience cerrado al iniciar el juego
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Si presionas la tecla, cambia el estado (si está abierto se cierra, y viceversa)
        if (Input.GetKeyDown(toggleKey))
        {
            isOpen = !isOpen;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(isOpen);
            }
        }
    }
}