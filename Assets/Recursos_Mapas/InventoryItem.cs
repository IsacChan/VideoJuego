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
            if (iconoImagen != null)
            {
                iconoImagen.enabled = false;
                iconoImagen.sprite = null;
                iconoImagen.gameObject.SetActive(false);
            }
            
            if (textoCantidad != null)
            {
                textoCantidad.text = "";
                textoCantidad.gameObject.SetActive(false);
            }
        }
        else
        {
            if (iconoImagen != null)
            {
                iconoImagen.gameObject.SetActive(true);
                iconoImagen.enabled = true;
                iconoImagen.sprite = itemActual.icono;
            }
            
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
        if (itemActual == null) return;

        Debug.Log($"Has usado el objeto: {itemActual.nombreItem}");

        string nombreLower = itemActual.nombreItem != null ? itemActual.nombreItem.ToLower() : "";
        string descLower = itemActual.descripcion != null ? itemActual.descripcion.ToLower() : "";
        bool consumido = false;

        // 1. Poción de vida
        if (nombreLower.Contains("pocion") || nombreLower.Contains("vida") || nombreLower.Contains("pocón"))
        {
            caminar jugador = FindFirstObjectByType<caminar>();

            if (jugador != null)
            {
                if (jugador.vidaActual >= jugador.vidaMaxima)
                {
                    Debug.Log("¡Vida llena! No puedes usar la poción ahora.");

                    InventoryNavigation navegacionUI = FindFirstObjectByType<InventoryNavigation>();
                    if (navegacionUI != null)
                    {
                        navegacionUI.MostrarMensajeAlerta("¡Vida llena!\nCada cosa a su tiempo.");
                    }

                    return;
                }

                jugador.Curar(2);
                consumido = true;
            }
        }
        // 2. Monedas -> 5 segundos de invulnerabilidad
        else if (nombreLower.Contains("moneda") || descLower.Contains("invulnerable"))
        {
            caminar jugador = FindFirstObjectByType<caminar>();

            if (jugador != null)
            {
                if (jugador.esInvulnerable)
                {
                    Debug.Log("¡Ya eres invulnerable! Espera a que termine el efecto.");

                    InventoryNavigation navegacionUI = FindFirstObjectByType<InventoryNavigation>();
                    if (navegacionUI != null)
                    {
                        navegacionUI.MostrarMensajeAlerta("¡Ya eres invulnerable!\nEspera a que termine.");
                    }

                    return;
                }

                jugador.ActivarInvulnerabilidadMoneda(5f);
                consumido = true;
            }
            else
            {
                Debug.LogWarning("No se encontró el componente 'caminar' en la escena.");
            }
        }
        else if (itemActual.esConsumible)
        {
            consumido = true;
        }

        if (consumido)
        {
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
}