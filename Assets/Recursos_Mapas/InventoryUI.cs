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

            // Pausar o reanudar el juego según el estado del inventario
            if (isOpen)
            {
                Time.timeScale = 0f; // Congela el movimiento del juego y del personaje
            }
            else
            {
                Time.timeScale = 1f; // Reanuda el juego a la normalidad
            }
        }
    }
}