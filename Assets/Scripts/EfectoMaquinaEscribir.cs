using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class EfectoMaquinaEscribir : MonoBehaviour
{
    private TextMeshProUGUI textoTextMeshPro;
    private string textoOriginal;
    private AudioSource audioSource;
    private AudioClip clickClip;

    [Header("Configuración de Tiempos y Velocidad")]
    [Tooltip("Segundos de espera en negro antes de empezar a escribir la historia")]
    public float retrasoInicial = 1.5f;          // NUEVO: Espera inicial
    [Tooltip("Tiempo de espera entre cada letra")]
    public float velocidadLetra = 0.05f;

    [Header("Sonido")]
    public bool reproducirSonido = true;

    [Header("Interfaz")]
    [Tooltip("Arrastra aquí el botón BtnComenzar")]
    public GameObject botonComenzar;

    private bool escribiendo = false;
    private bool mostrarCompleto = false;

    void Awake()
    {
        textoTextMeshPro = GetComponent<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (textoTextMeshPro != null)
        {
            textoOriginal = textoTextMeshPro.text;
            textoTextMeshPro.text = "";
        }

        clickClip = GenerarSonidoClickProcedural();
    }

    void OnEnable()
    {
        // Ocultamos el botón al abrir el panel antes de iniciar la animación
        if (botonComenzar != null)
        {
            botonComenzar.SetActive(false);
        }

        if (textoTextMeshPro != null && !string.IsNullOrEmpty(textoOriginal))
        {
            StopAllCoroutines();
            StartCoroutine(MostrarTextoLetraPorLetra());
        }
    }

    IEnumerator MostrarTextoLetraPorLetra()
    {
        textoTextMeshPro.text = "";
        escribiendo = true;
        mostrarCompleto = false;

        // 1. NUEVO: Espera inicial antes de teclear la primera letra
        yield return new WaitForSeconds(retrasoInicial);

        string textoActual = "";
        foreach (char letra in textoOriginal.ToCharArray())
        {
            // 2. NUEVO: Si el jugador hace clic, muestra todo de golpe y sale del bucle
            if (mostrarCompleto)
            {
                textoTextMeshPro.text = textoOriginal;
                break;
            }

            textoActual += letra;
            textoTextMeshPro.text = textoActual;

            if (reproducirSonido && !char.IsWhiteSpace(letra) && audioSource != null && clickClip != null)
            {
                audioSource.pitch = Random.Range(0.85f, 1.15f);
                audioSource.PlayOneShot(clickClip, 0.3f);
            }

            yield return new WaitForSeconds(velocidadLetra);
        }

        escribiendo = false;

        // Al terminar de escribir todo el texto, activamos el botón
        if (botonComenzar != null)
        {
            botonComenzar.SetActive(true);
        }
    }

    void Update()
    {
        // 3. NUEVO: Detecta si el usuario hace clic izquierdo en cualquier parte mientras se escribe el texto
        if (escribiendo && Input.GetMouseButtonDown(0))
        {
            mostrarCompleto = true;
        }
    }

    AudioClip GenerarSonidoClickProcedural()
    {
        int sampleRate = 44100;
        float duration = 0.015f;
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float noise = (Random.value * 2f - 1f);
            float envelope = 1f - ((float)i / sampleCount); 
            samples[i] = noise * envelope * 0.4f;
        }

        AudioClip clip = AudioClip.Create("TypewriterClick", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}