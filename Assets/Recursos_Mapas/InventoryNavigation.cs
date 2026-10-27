using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InventoryNavigation : MonoBehaviour
{
    [Header("Referencias de la Interfaz")]
    public GameObject panelDescripcion;       
    public Image imagenDescripcionIcono;      
    public TextMeshProUGUI textoDescripcion;  

    [Header("Configuración de Cuadrícula")]
    public int columnas = 5;                  
    private int indiceSeleccionado = 0;       

    void OnEnable()
    {
        indiceSeleccionado = 0;
        if (panelDescripcion != null) panelDescripcion.SetActive(true);
        StartCoroutine(ActualizarDemorado());
    }

    IEnumerator ActualizarDemorado()
    {
        yield return null; // Espera un frame para asegurar que todo esté listo
        ActualizarPanelVisual();
    }

    void Update()
    {
        // Verificamos si existe la instancia de InventoryManager
        if (InventoryManager.instancia == null) return;

        bool seMovio = false;

        // Controles de navegación (Flechas y WASD)
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            indiceSeleccionado = (indiceSeleccionado + 1) % InventoryManager.instancia.numeroDeSlots;
            seMovio = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            indiceSeleccionado = (indiceSeleccionado - 1 + InventoryManager.instancia.numeroDeSlots) % InventoryManager.instancia.numeroDeSlots;
            seMovio = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            indiceSeleccionado += columnas;
            if (indiceSeleccionado >= InventoryManager.instancia.numeroDeSlots)
                indiceSeleccionado %= InventoryManager.instancia.numeroDeSlots; 
            seMovio = true;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            indiceSeleccionado -= columnas;
            if (indiceSeleccionado < 0)
                indiceSeleccionado = InventoryManager.instancia.numeroDeSlots - 1; 
            seMovio = true;
        }

        if (seMovio)
        {
            ActualizarPanelVisual();
        }
    }

    public void ActualizarPanelVisual()
    {
        if (panelDescripcion != null && !panelDescripcion.activeSelf)
        {
            panelDescripcion.SetActive(true);
        }

        if (InventoryManager.instancia == null) return;

        // Accedemos de forma limpia a los hijos transform del slotContainer que maneja el InventoryManager
        Transform slotContainer = InventoryManager.instancia.slotContainer;
        if (slotContainer == null || slotContainer.childCount == 0) return;

        if (indiceSeleccionado >= slotContainer.childCount) indiceSeleccionado = 0;

        Transform slotTransform = slotContainer.GetChild(indiceSeleccionado);
        InventoryItem itemSlot = slotTransform.GetComponent<InventoryItem>();

        if (itemSlot != null && itemSlot.itemActual != null)
        {
            // Hay un ítem en esta casilla: mostramos su icono y descripción
            if (imagenDescripcionIcono != null)
            {
                imagenDescripcionIcono.gameObject.SetActive(true);
                imagenDescripcionIcono.enabled = true;
                imagenDescripcionIcono.sprite = itemSlot.itemActual.icono;
                imagenDescripcionIcono.preserveAspect = true;
            }

            if (textoDescripcion != null)
            {
                textoDescripcion.gameObject.SetActive(true);
                textoDescripcion.text = "<b>" + itemSlot.itemActual.nombreItem + "</b>\n\n" + itemSlot.itemActual.descripcion;
            }
        }
        else
        {
            // Casilla vacía
            if (imagenDescripcionIcono != null)
            {
                imagenDescripcionIcono.sprite = null;
                imagenDescripcionIcono.enabled = false;
            }

            if (textoDescripcion != null)
            {
                textoDescripcion.text = "<b>Casilla vacía</b>\n\nNo hay ningún objeto seleccionado.";
            }
        }
    }

    // Método para mostrar mensajes de alerta directamente en el panel de texto (Fuera del Update y del Else)
    public void MostrarMensajeAlerta(string mensaje)
    {
        if (panelDescripcion != null) panelDescripcion.SetActive(true);

        if (textoDescripcion != null)
        {
            textoDescripcion.gameObject.SetActive(true);
            // Pintamos el texto en un tono llamativo (rojo/naranja) para que destaque
            textoDescripcion.text = "<color=#FF5555><b>¡Atención!</b></color>\n\n" + mensaje;
        }
    }
}