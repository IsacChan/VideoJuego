using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    public ItemData itemActual;
    public int cantidad;

    [Header("Referencias")]
    public Image iconoImagen;
    public TextMeshProUGUI textoCantidad;

    private void Start()
    {
        ActualizarUI();
    }

    public void AsignarItem(ItemData nuevoItem, int cantidadInicial = 1)
    {
        itemActual = nuevoItem;
        cantidad = cantidadInicial;
        ActualizarUI();
    }

    public void LimpiarSlot()
    {
        itemActual = null;
        cantidad = 0;
        ActualizarUI();
    }

    public void ActualizarUI()
    {
        if (itemActual == null)
        {
            iconoImagen.enabled = false;
            iconoImagen.sprite = null;
            
            if (textoCantidad != null)
            {
                textoCantidad.text = "";
                textoCantidad.gameObject.SetActive(false);
            }
        }
        else
        {
            iconoImagen.enabled = true;
            iconoImagen.sprite = itemActual.icono;
            
            if (textoCantidad != null)
            {
                textoCantidad.gameObject.SetActive(true);
                textoCantidad.text = "x" + cantidad;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemActual != null)
        {
            UsarObjeto();
        }
    }

    void UsarObjeto()
    {
        Debug.Log($"Has usado el objeto: {itemActual.nombreItem}");

        // Verificamos si es una poción de vida
        if (itemActual.nombreItem.Contains("Pocion") || itemActual.nombreItem.Contains("vida") || itemActual.nombreItem.Contains("Pocón"))
        {
            // Buscamos al jugador en la escena que tiene el script "caminar"
            caminar jugador = FindFirstObjectByType<caminar>();

            if (jugador != null)
            {
                // Curamos 2 puntos (equivalente a medio corazón, ya que 4 puntos = 1 corazón completo)
                jugador.Curar(2);
            }
        }

        // Reducir la cantidad al consumirlo
        cantidad--;
        
        if (cantidad <= 0)
        {
            LimpiarSlot();
        }
        else
        {
            ActualizarUI();
        }
    }
}