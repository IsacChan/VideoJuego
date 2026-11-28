using System.Collections;
using TMPro;
using UnityEngine;

public class ChestRoomKey : MonoBehaviour
{
    [Header("Sistema de guardado")]
    [SerializeField] private string idLlave = "mapa1_llave_01";

    [Header("Referencias")]
    public Animator anim;
    public AudioSource audioSource;

    [Header("UI - Avisos en pantalla")]
    [SerializeField] private TextMeshProUGUI textoAviso;
    [SerializeField] private string nombreObjetoAvisoUI = "TextoAvisoCofre";
    [SerializeField] private string mensajeInteractuar = "Pulsa E para abrir";
    [SerializeField] private string mensajeLlaveObtenida = "Llave obtenida";
    [SerializeField] private float duracionMensajeObtenida = 1.8f;

    [Header("Configuración")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Sonido")]
    public AudioClip openSound;
    [Range(0f, 3f)] public float soundVolume = 1.5f;

    private bool playerIsClose = false;
    private bool isOpen = false;
    private Coroutine rutinaMensaje;

    // Inventario global / del mapa actual.
    private InventarioJugador inventario;

    public bool LlaveRecogida =>
        ProgresoMundo.EstaCompletado(idLlave);

    void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        ReenlazarTextoAviso();
        OcultarAviso();
    }

    void Start()
    {
        inventario = InventarioJugador.ObtenerOCrear();

        // Si la llave ya fue obtenida y guardada, ocultar el cofre.
        if (LlaveRecogida)
        {
            OcultarAviso();
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

        if (inventario == null)
            inventario = InventarioJugador.ObtenerOCrear();

        if (inventario == null)
        {
            Debug.LogWarning(
                "No se encontró el inventario del mapa actual."
            );
            return;
        }

        isOpen = true;
        MostrarMensajeTemporal(mensajeLlaveObtenida, duracionMensajeObtenida);

        // Registrar llave obtenida.
        ProgresoMundo.Registrar(idLlave);

        // Entregar llave al inventario.
        inventario.AgregarLlave();

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

        Debug.Log(
            "Llave especial obtenida: " + idLlave
        );

        Destroy(gameObject, 2f);
    }

    private void ReenlazarTextoAviso()
    {
        if (textoAviso != null)
            return;

        Canvas[] canvases = FindObjectsByType<Canvas>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        if (!string.IsNullOrEmpty(nombreObjetoAvisoUI))
        {
            foreach (Canvas canvas in canvases)
            {
                if (canvas == null) continue;

                Transform[] hijos = canvas.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in hijos)
                {
                    if (t != null && t.name == nombreObjetoAvisoUI)
                    {
                        textoAviso = t.GetComponent<TextMeshProUGUI>();
                        if (textoAviso != null)
                            return;
                    }
                }
            }
        }
    }

    private void MostrarAvisoInteractuar()
    {
        ReenlazarTextoAviso();
        if (textoAviso == null) return;

        if (rutinaMensaje != null)
        {
            StopCoroutine(rutinaMensaje);
            rutinaMensaje = null;
        }

        textoAviso.text = mensajeInteractuar;
        textoAviso.gameObject.SetActive(true);
    }

    private void MostrarMensajeTemporal(string mensaje, float duracion)
    {
        ReenlazarTextoAviso();
        if (textoAviso == null) return;

        if (rutinaMensaje != null)
            StopCoroutine(rutinaMensaje);

        rutinaMensaje = StartCoroutine(RutinaMensajeTemporal(mensaje, duracion));
    }

    private IEnumerator RutinaMensajeTemporal(string mensaje, float duracion)
    {
        textoAviso.text = mensaje;
        textoAviso.gameObject.SetActive(true);

        yield return new WaitForSeconds(duracion);

        if (!isOpen && playerIsClose)
        {
            textoAviso.text = mensajeInteractuar;
        }
        else
        {
            OcultarAviso();
        }

        rutinaMensaje = null;
    }

    private void OcultarAviso()
    {
        if (textoAviso != null && textoAviso.gameObject.activeSelf)
        {
            textoAviso.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpen && !LlaveRecogida)
        {
            playerIsClose = true;
            MostrarAvisoInteractuar();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            if (!isOpen && rutinaMensaje == null)
            {
                OcultarAviso();
            }
        }
    }

    void OnDisable()
    {
        if (rutinaMensaje != null)
        {
            StopCoroutine(rutinaMensaje);
            rutinaMensaje = null;
        }
        OcultarAviso();
    }
}