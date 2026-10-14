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
    [Tooltip("Tiempo de espera entre cada letra (mayor número = más lento, prueba con 0.05 o 0.06)")]
    public float velocidadLetra = 0.05f;

    [Header("Sonido")]
    public bool reproducirSonido = true;

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

        // Generamos un sonido de clic mecánico por código automáticamente
        clickClip = GenerarSonidoClickProcedural();
    }

    void OnEnable()
    {
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

            // Si es una letra visible (no espacio) y el sonido está activo, reproducimos el clic
            if (reproducirSonido && !char.IsWhiteSpace(letra) && audioSource != null && clickClip != null)
            {
                audioSource.pitch = Random.Range(0.85f, 1.15f); // Varía ligeramente el tono para que suene natural
                audioSource.PlayOneShot(clickClip, 0.3f);
            }

            yield return new WaitForSeconds(velocidadLetra);
        }
    }

    // Método para crear un sonido de tecleo de forma puramente matemática por código
    AudioClip GenerarSonidoClickProcedural()
    {
        int sampleRate = 44100;
        float duration = 0.015f; // Duración ultra corta (15 milisegundos)
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            // Mezcla de ruido blanco y decaimiento rápido para simular el golpe de una tecla
            float noise = (Random.value * 2f - 1f);
            float envelope = 1f - ((float)i / sampleCount); 
            samples[i] = noise * envelope * 0.4f;
        }

        AudioClip clip = AudioClip.Create("TypewriterClick", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}