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
                // VALIDACIÓN: Si la vida actual es igual o mayor a la máxima, no dejamos usarla
                if (jugador.vidaActual >= jugador.vidaMaxima)
                {
                    Debug.Log("¡Vida llena! No puedes usar la poción ahora.");

                    // Buscamos el script de navegación para mostrar el mensaje en la pantalla del juego
                    InventoryNavigation navegacionUI = FindFirstObjectByType<InventoryNavigation>();
                    if (navegacionUI != null)
                    {
                        navegacionUI.MostrarMensajeAlerta("¡Vida llena!\nCada cosa a su tiempo.");
                    }

                    return; // IMPORTANTE: Detiene el código aquí y NO gasta el objeto
                }

                // Curamos 2 puntos (equivalente a medio corazón)
                jugador.Curar(2);
            }
        }

        // Reducir la cantidad al consumirlo (solo se ejecuta si SÍ se usó la poción)
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