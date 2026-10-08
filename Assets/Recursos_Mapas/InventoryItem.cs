using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    public ItemData itemActual;
    public int cantidad;

    [Header("Referencias")]
    public Image iconoImagen;

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

    private void ActualizarUI()
    {
        if (itemActual == null)
        {
            iconoImagen.enabled = false;
            iconoImagen.sprite = null;
        }
        else
        {
            iconoImagen.enabled = true;
            iconoImagen.sprite = itemActual.icono;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemActual != null)
            Debug.Log($"Clic en: {itemActual.nombreItem} x{cantidad}");
    }
}