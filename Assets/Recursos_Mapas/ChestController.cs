using UnityEngine;

public class ChestController : MonoBehaviour
{
    [Header("Referencias")]
    public Animator anim;
    public AudioSource audioSource; 
    
    [Header("Configuración")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Sistema de Botín (Items)")]
    public ItemData[] possibleItems; // Arrastra aquí tus ScriptableObjects (Llaves, Pociones, etc.)
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

        // Reproduce el sonido
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound, soundVolume);
        }

        // Entregamos el objeto directamente al inventario
        GiveLoot();
        
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            if (col.isTrigger)
            {
                col.enabled = false;
            }
        }

        // Se destruye el cofre después de que termine el sonido
        Destroy(gameObject, 2f);
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
                
                // Llamamos a tu InventoryManager existente para añadir el objeto
                bool exito = InventoryManager.instancia.AñadirItem(itemToGive, cantidad);

                if (exito)
                {
                    Debug.Log($"¡Objeto obtenido: {cantidad}x {itemToGive.nombreItem}!");
                }
                else
                {
                    Debug.Log("¡Inventario lleno! No se pudo guardar el objeto.");
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
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
        }
    }
}