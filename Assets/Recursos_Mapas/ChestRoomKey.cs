using UnityEngine;

public class ChestRoomKey : MonoBehaviour
{
    [Header("Referencias")]
    public Animator anim;
    public AudioSource audioSource;

    [Header("Configuración")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Sonido")]
    public AudioClip openSound;
    [Range(0f, 3f)] public float soundVolume = 1.5f;

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

        // Reproduce animación de apertura si existe
        if (anim != null)
        {
            anim.SetTrigger("Open");
        }

        // Reproduce el efecto de sonido
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound, soundVolume);
        }

        // Suma la llave al contador del inventario/UI
        GiveKey();

        // Desactiva los colliders de tipo Trigger para evitar interacciones dobles
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            if (col.isTrigger)
            {
                col.enabled = false;
            }
        }

        // Destruye el cofre tras terminar el sonido
        Destroy(gameObject, 2f);
    }

    void GiveKey()
    {
        if (InventarioJugador.Instancia != null)
        {
            InventarioJugador.Instancia.AgregarLlave();
            Debug.Log("¡Llave especial de sala obtenida!");
        }
        else
        {
            Debug.LogWarning("No se encontró la 'Instancia' de InventarioJugador en la escena.");
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