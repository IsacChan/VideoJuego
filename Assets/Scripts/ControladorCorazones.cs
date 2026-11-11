using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ControladorCorazones : MonoBehaviour
{
    [Header("Sprites de Estados del Corazón")]
    public Sprite spriteLleno;         // 4/4
    public Sprite spriteTresCuartos; // 3/4
    public Sprite spriteMitad;       // 2/4
    public Sprite spriteUnCuarto;    // 1/4
    public Sprite spriteVacio;       // 0/4

    [Header("Configuración de Destello")]
    public float duracionDestello = 0.15f; 
    public int numeroDeParpadeos = 2;    

    [Header("Configuración General")]
    public int puntosPorCorazon = 4;

    private List<Image> listaImagenesCorazon = new List<Image>();
    private List<Image> listaImagenesDestello = new List<Image>();
    private Dictionary<Image, Coroutine> corrutinasActivas = new Dictionary<Image, Coroutine>();

    void Awake()
    {
        ActualizarReferencias();
    }

    void Start()
    {
        ActualizarReferencias();

        if (DatosTransporte.datosInicializados)
        {
            ActualizarVida((int)DatosTransporte.vidaJugador, false);
        }
        else
        {
            caminar jugador = Object.FindFirstObjectByType<caminar>();
            if (jugador != null)
            {
                ActualizarVida(jugador.vidaActual, false);
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ActualizarReferencias();
        
        if (DatosTransporte.datosInicializados)
        {
            ActualizarVida((int)DatosTransporte.vidaJugador, false);
        }
    }

    public void ActualizarReferencias()
    {
        listaImagenesCorazon.Clear();
        listaImagenesDestello.Clear();
        corrutinasActivas.Clear();

        foreach (Transform hijo in transform)
        {
            Image imgCorazon = hijo.GetComponent<Image>();

            Image imgDestello = null;
            Transform destelloTransform = hijo.Find("Destello");
            if (destelloTransform != null)
            {
                imgDestello = destelloTransform.GetComponent<Image>();
                Color col = imgDestello.color;
                col.a = 0;
                imgDestello.color = col;
            }

            if (imgCorazon != null) listaImagenesCorazon.Add(imgCorazon);
            if (imgDestello != null) listaImagenesDestello.Add(imgDestello);
        }
    }

    public void ActualizarVida(int vidaActual, bool recibioDaño)
    {
        if (listaImagenesCorazon.Count == 0) ActualizarReferencias();

        for (int i = 0; i < listaImagenesCorazon.Count; i++)
        {
            int vidaMinimaEsteCorazon = i * puntosPorCorazon;
            int vidaEnEsteCorazon = vidaActual - vidaMinimaEsteCorazon;
            vidaEnEsteCorazon = Mathf.Clamp(vidaEnEsteCorazon, 0, puntosPorCorazon);

            Sprite spriteAnterior = listaImagenesCorazon[i].sprite;
            Sprite spriteNuevo = spriteLleno;

            switch (vidaEnEsteCorazon)
            {
                case 4: spriteNuevo = spriteLleno; break;
                case 3: spriteNuevo = spriteTresCuartos; break;
                case 2: spriteNuevo = spriteMitad; break;
                case 1: spriteNuevo = spriteUnCuarto; break;
                case 0: spriteNuevo = spriteVacio; break;
            }

            // Actualizamos el sprite del corazón base de inmediato
            listaImagenesCorazon[i].sprite = spriteNuevo;

            // Verificamos si existe la imagen de destello para este corazón
            if (i < listaImagenesDestello.Count && listaImagenesDestello[i] != null)
            {
                // Si RECIBIÓ DAÑO y este corazón en específico bajó de nivel, lanzamos el destello de daño
                if (recibioDaño && spriteNuevo != spriteAnterior)
                {
                    if (corrutinasActivas.ContainsKey(listaImagenesDestello[i]) && corrutinasActivas[listaImagenesDestello[i]] != null)
                    {
                        StopCoroutine(corrutinasActivas[listaImagenesDestello[i]]);
                    }

                    corrutinasActivas[listaImagenesDestello[i]] = StartCoroutine(EjecutarDestello(listaImagenesDestello[i]));
                }
                else
                {
                    // Si nos curamos (o cargamos escena), nos aseguramos de apagar por completo el destello para que se vea limpio
                    if (corrutinasActivas.ContainsKey(listaImagenesDestello[i]) && corrutinasActivas[listaImagenesDestello[i]] != null)
                    {
                        StopCoroutine(corrutinasActivas[listaImagenesDestello[i]]);
                    }

                    Color col = listaImagenesDestello[i].color;
                    col.a = 0f;
                    listaImagenesDestello[i].color = col;
                }
            }
        }
    }

    private IEnumerator EjecutarDestello(Image destelloImg)
    {
        Color col = Color.white;

        for (int p = 0; p < numeroDeParpadeos; p++)
        {
            col.a = 1f;
            destelloImg.color = col;
            yield return new WaitForSeconds(duracionDestello);

            col.a = 0f;
            destelloImg.color = col;
            yield return new WaitForSeconds(duracionDestello);
        }
    }
}