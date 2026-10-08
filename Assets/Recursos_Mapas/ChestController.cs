using UnityEngine;

public class ChestController : MonoBehaviour
{
    [Header("Referencias")]
    public Animator anim;
    public AudioSource audioSource; // Referencia al componente de audio
    
    [Header("Configuración")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Sistema de Botín (Loot Aleatorio)")]
    public GameObject[] possibleDrops;
    public Transform dropSpawnPoint;

    [Header("Sonido")]
    public AudioClip openSound;
    [Range(0f, 3f)] public float soundVolume = 1.5f;

    [HideInInspector] public int spawnIndex;

    private bool playerIsClose = false;
    private bool isOpen = false;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        
        // Busca automáticamente el AudioSource si no lo arrastraste
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

        // Reproduce el sonido usando el AudioSource local en 2D con todo su volumen
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound, soundVolume);
        }

        DropRandomItem();
        
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            if (col.isTrigger)
            {
                col.enabled = false;
            }
        }

        // Espera 2 segundos antes de destruir el objeto para que el sonido termine de reproducirse bien
        Destroy(gameObject, 2f);
    }

    void DropRandomItem()
    {
        if (possibleDrops != null && possibleDrops.Length > 0)
        {
            int randomIndex = Random.Range(0, possibleDrops.Length);
            GameObject itemToDrop = possibleDrops[randomIndex];

            if (itemToDrop != null)
            {
                Vector3 spawnPos = dropSpawnPoint != null ? dropSpawnPoint.position : transform.position;
                Instantiate(itemToDrop, spawnPos, Quaternion.identity);
            }
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