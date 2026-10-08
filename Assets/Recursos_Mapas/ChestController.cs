using UnityEngine;

public class ChestController : MonoBehaviour
{
    [Header("Referencias")]
    public Animator anim;
    
    [Header("Configuración")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Sistema de Botín (Loot Aleatorio)")]
    public GameObject[] possibleDrops;
    public Transform dropSpawnPoint;

    [Header("Sonido")]
    public AudioClip openSound;

    [HideInInspector] public int spawnIndex;

    private bool playerIsClose = false;
    private bool isOpen = false;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
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

        if (openSound != null)
        {
            AudioSource.PlayClipAtPoint(openSound, transform.position);
        }

        DropRandomItem();
        
        // Desactiva el collider para que no se pueda interactuar dos veces
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            if (col.isTrigger)
            {
                col.enabled = false;
            }
        }

        // Se abre, reproduce su animación y se destruye para siempre tras 2 segundos
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