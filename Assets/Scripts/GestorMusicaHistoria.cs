using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestorMusicaHistoria : MonoBehaviour
{
    [Header("Lista de Canciones")]
    [SerializeField] private List<AudioClip> playlistCanciones = new List<AudioClip>();

    [Header("Configuración de Transición")]
    public float duracionFade = 2f; // Tiempo que tarda en entrar y salir suavemente
    public float volumenMaximo = 1f;

    private AudioSource audioSource;
    private int indiceActual = 0;

    void Start()
    {
        // Configuramos o buscamos el AudioSource en el mismo objeto
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = false; 
        audioSource.volume = 0f;

        if (playlistCanciones != null && playlistCanciones.Count > 0)
        {
            StartCoroutine(ReproducirPlaylist());
        }
        else
        {
            Debug.LogWarning("La playlist de canciones está vacía o no se ha asignado.");
        }
    }

    IEnumerator ReproducirPlaylist()
    {
        while (true) // Bucle para que la música continúe en la escena de historia
        {
            AudioClip cancionActual = playlistCanciones[indiceActual];
            audioSource.clip = cancionActual;
            audioSource.Play();

            // 1. FADE IN: Sube el volumen suavemente
            yield return StartCoroutine(FadeIn());

            // Espera a que termine la canción (restando el tiempo del Fade Out)
            float tiempoDeEspera = cancionActual.length - duracionFade;
            if (tiempoDeEspera < 0) tiempoDeEspera = 0;

            yield return new WaitForSeconds(tiempoDeEspera);

            // 2. FADE OUT: Baja el volumen suavemente antes de cambiar
            yield return StartCoroutine(FadeOut());

            // Pasa a la siguiente canción y vuelve al inicio si llega al final
            indiceActual = (indiceActual + 1) % playlistCanciones.Count;
        }
    }

    IEnumerator FadeIn()
    {
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionFade)
        {
            tiempoTranscurrido += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, volumenMaximo, tiempoTranscurrido / duracionFade);
            yield return null;
        }
        audioSource.volume = volumenMaximo;
    }

    IEnumerator FadeOut()
    {
        float tiempoTranscurrido = 0f;
        float volumenInicial = audioSource.volume;
        while (tiempoTranscurrido < duracionFade)
        {
            tiempoTranscurrido += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(volumenInicial, 0f, tiempoTranscurrido / duracionFade);
            yield return null;
        }
        audioSource.volume = 0f;
    }
}