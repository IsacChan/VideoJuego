
using UnityEngine;

public class ChestRoomKey : MonoBehaviour
{
    [Header("Sistema de guardado")]
    [SerializeField] private string idLlave = "mapa1_llave_01";

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

    public bool LlaveRecogida =>
        ProgresoMundo.EstaCompletado(idLlave);

    void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Si la llave ya fue obtenida y guardada,
        // ocultar el cofre.
        if (LlaveRecogida)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerIsClose &&
            !isOpen &&
            Input.GetKeyDown(interactKey))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        if (isOpen || LlaveRecogida)
            return;

        if (InventarioJugador.Instancia == null)
        {
            Debug.LogWarning(
                "No se encontró InventarioJugador."
            );
            return;
        }

        isOpen = true;

        // Registrar llave obtenida en memoria
        ProgresoMundo.Registrar(idLlave);

        // Entregar llave al jugador
        InventarioJugador.Instancia.AgregarLlave();

        if (anim != null)
            anim.SetTrigger("Open");

        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(
                openSound,
                soundVolume
            );
        }

        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            if (col.isTrigger)
                col.enabled = false;
        }

        Debug.Log("Llave especial obtenida: " + idLlave);

        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerIsClose = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerIsClose = false;
    }
}
