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

    [Header("Configuración de Velocidad")]
    [Tooltip("Tiempo de espera entre cada letra")]
    public float velocidadLetra = 0.05f;

    [Header("Sonido")]
    public bool reproducirSonido = true;

    [Header("Interfaz")]
    [Tooltip("Arrastra aquí el botón BtnComenzar")]
    public GameObject botonComenzar;

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
        
        foreach (char letra in textoOriginal.ToCharArray())
        {
            textoTextMeshPro.text += letra;

            if (reproducirSonido && !char.IsWhiteSpace(letra) && audioSource != null && clickClip != null)
            {
                audioSource.pitch = Random.Range(0.85f, 1.15f);
                audioSource.PlayOneShot(clickClip, 0.3f);
            }

            yield return new WaitForSeconds(velocidadLetra);
        }

        // Al terminar de escribir todo el texto, activamos el botón
        if (botonComenzar != null)
        {
            botonComenzar.SetActive(true);
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