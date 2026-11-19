using UnityEngine;
using TMPro;
using System.Collections;

public class ChestController : MonoBehaviour
{
    [Header("Referencias")]
    public Animator anim;
    public AudioSource audioSource; 
    
    [Header("Configuración")]
    public KeyCode interactKey = KeyCode.E;

    [Header("UI & Avisos")]
    public GameObject promptUI; // El objeto de texto hijo que está en el cofre
    private TextMeshProUGUI textoPromptTMP; // El componente de texto

    [Header("Sistema de Botín (Items)")]
    public ItemData[] possibleItems; 
    public int cantidadMinima = 1;
    public int cantidadMaxima = 1;

    [Header("Sonido")]
    public AudioClip openSound;
    [Range(0f, 3f)] public float soundVolume = 1.5f;

    [HideInInspector] public int spawnIndex;

    private bool playerIsClose = false;
    private bool isOpen = false;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (promptUI != null)
        {
            textoPromptTMP = promptUI.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        if (playerIsClose && !isOpen && Input.GetKeyDown(interactKey))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpen = true;

        if (anim != null)
        {
            anim.SetTrigger("Open");
        }

        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound, soundVolume);
        }

        GiveLoot();
        
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            if (col.isTrigger)
            {
                col.enabled = false;
            }
        }

        // Destruye el cofre después de 2.5 segundos para dar tiempo a leer el texto
        Destroy(gameObject, 2.5f);
    }

    void GiveLoot()
    {
        if (possibleItems != null && possibleItems.Length > 0)
        {
            int randomIndex = Random.Range(0, possibleItems.Length);
            ItemData itemToGive = possibleItems[randomIndex];

            if (itemToGive != null)
            {
                int cantidad = Random.Range(cantidadMinima, cantidadMaxima + 1);
                
                bool exito = InventoryManager.instancia.AñadirItem(itemToGive, cantidad);

                string mensajeFinal = "";
                if (exito)
                {
                    mensajeFinal = $"¡Obtenido: {cantidad}x {itemToGive.nombreItem}!";
                    Debug.Log(mensajeFinal);
                }
                else
                {
                    mensajeFinal = $"¡Inventario lleno! ({cantidad}x {itemToGive.nombreItem})";
                    Debug.Log(mensajeFinal);
                }

                // Cambia el texto flotante para mostrar la recompensa obtenida
                if (textoPromptTMP != null && promptUI != null)
                {
                    textoPromptTMP.text = mensajeFinal;
                    promptUI.SetActive(true);
                }
            }
        }
        else
        {
            Debug.LogWarning("El cofre no tiene items asignados en 'Possible Items'.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            playerIsClose = true;
            if (promptUI != null)
            {
                if (textoPromptTMP != null) textoPromptTMP.text = "Presiona E";
                promptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            playerIsClose = false;
            if (promptUI != null) promptUI.SetActive(false);
        }
    }
}