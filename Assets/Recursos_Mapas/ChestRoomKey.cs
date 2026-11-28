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

    private InventarioJugador inventario;

    public bool LlaveRecogida =>
        ProgresoMundo.EstaCompletado(idLlave);

    private bool CofreDisponible =>
        gameObject.activeInHierarchy && !isOpen && !LlaveRecogida;

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

        if (LlaveRecogida)
        {
            playerIsClose = false;
            OcultarAviso();
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!CofreDisponible)
            return;

        if (playerIsClose && Input.GetKeyDown(interactKey))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        if (!CofreDisponible)
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
        playerIsClose = false;

        ProgresoMundo.Registrar(idLlave);
        inventario.AgregarLlave();

        MostrarMensajeTemporal(mensajeLlaveObtenida, duracionMensajeObtenida);

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

        if (string.IsNullOrEmpty(nombreObjetoAvisoUI))
            return;

        Canvas[] canvases = FindObjectsByType<Canvas>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

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

    private void MostrarAvisoInteractuar()
    {
        if (!CofreDisponible)
        {
            OcultarAviso();
            return;
        }

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

        if (CofreDisponible && playerIsClose)
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
        if (!CofreDisponible)
            return;

        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
            MostrarAvisoInteractuar();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!CofreDisponible)
        {
            playerIsClose = false;
            return;
        }

        if (other.CompareTag("Player") && !playerIsClose)
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
        playerIsClose = false;
        if (rutinaMensaje != null)
        {
            StopCoroutine(rutinaMensaje);
            rutinaMensaje = null;
        }
        OcultarAviso();
    }
}