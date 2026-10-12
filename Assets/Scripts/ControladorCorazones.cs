using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorCorazones : MonoBehaviour
{
    [Header("Sprites de Estados del Corazón")]
    public Sprite spriteLleno;        // 4/4
    public Sprite spriteTresCuartos; // 3/4
    public Sprite spriteMitad;       // 2/4
    public Sprite spriteUnCuarto;    // 1/4
    public Sprite spriteVacio;       // 0/4

    [Header("Configuración de Destello")]
    public float duracionDestello = 0.15f; // Qué tan rápido parpadea cada vez
    public int numeroDeParpadeos = 2;    // 2 para doble parpadeo, 3 para triple, etc.

    [Header("Configuración General")]
    public int puntosPorCorazon = 4;

    private List<Image> listaImagenesCorazon = new List<Image>();
    private List<Image> listaImagenesDestello = new List<Image>();
    private Dictionary<Image, Coroutine> corrutinasActivas = new Dictionary<Image, Coroutine>();

    void Awake()
    {
        ActualizarReferencias();
    }

    public void ActualizarReferencias()
    {
        listaImagenesCorazon.Clear();
        listaImagenesDestello.Clear();
        corrutinasActivas.Clear();

        foreach (Transform hijo in transform)
        {
            // 1. Imagen del corazón principal (abajo)
            Image imgCorazon = hijo.GetComponent<Image>();

            // 2. Imagen del objeto hijo "Destello" (arriba)
            Image imgDestello = null;
            Transform destelloTransform = hijo.Find("Destello");
            if (destelloTransform != null)
            {
                imgDestello = destelloTransform.GetComponent<Image>();
                // Aseguramos que el destello comience transparente
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

            // Actualizamos la imagen fraccionaria del corazón de abajo
            listaImagenesCorazon[i].sprite = spriteNuevo;

            // Si recibió daño y este corazón específico bajó de nivel, lanzamos el destello múltiple
            if (recibioDaño && spriteNuevo != spriteAnterior && i < listaImagenesDestello.Count && listaImagenesDestello[i] != null)
            {
                if (corrutinasActivas.ContainsKey(listaImagenesDestello[i]) && corrutinasActivas[listaImagenesDestello[i]] != null)
                {
                    StopCoroutine(corrutinasActivas[listaImagenesDestello[i]]);
                }

                corrutinasActivas[listaImagenesDestello[i]] = StartCoroutine(EjecutarDestello(listaImagenesDestello[i]));
            }
        }
    }

    private IEnumerator EjecutarDestello(Image destelloImg)
    {
        Color col = Color.white;

        // Repetimos el parpadeo las veces que indique "numeroDeParpadeos"
        for (int p = 0; p < numeroDeParpadeos; p++)
        {
            // Encender destello (Alpha a 1)
            col.a = 1f;
            destelloImg.color = col;
            yield return new WaitForSeconds(duracionDestello);

            // Apagar destello (Alpha a 0)
            col.a = 0f;
            destelloImg.color = col;
            yield return new WaitForSeconds(duracionDestello);
        }
    }
}