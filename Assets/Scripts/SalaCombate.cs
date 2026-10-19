
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class SalaCombate : MonoBehaviour
{
    [Header("Puertas")]
    public GameObject[] puertas;

    [Header("Configuracion")]
    public string tagJugador = "Player";
    public string tagEnemigo = "Enemy";

    [Header("Deteccion")]
    public LayerMask capasDeteccion = ~0;

    private BoxCollider2D zona;
    private bool combateIniciado = false;
    private bool salaCompletada = false;
    private bool enemigosDetectados = false;
private Transform jugadorDentro;
private float margenEntrada = 0.5f;


    void Awake()
    {
        zona = GetComponent<BoxCollider2D>();
        zona.isTrigger = true;
    }

    void Start()
    {
        CambiarPuertas(false);
    }

    

void OnTriggerEnter2D(Collider2D other)
{
    if (salaCompletada || combateIniciado)
        return;

    if (!other.CompareTag(tagJugador))
        return;

    // Guardar referencia del jugador
    jugadorDentro = other.transform;
}



    
void Update()
{
    if (salaCompletada)
        return;

    // Esperar a que el jugador entre completamente
    if (!combateIniciado && jugadorDentro != null)
    {
        Bounds limites = zona.bounds;

        Vector3 posicion = jugadorDentro.position;

        bool dentro =
            posicion.x > limites.min.x + margenEntrada &&
            posicion.x < limites.max.x - margenEntrada &&
            posicion.y > limites.min.y + margenEntrada &&
            posicion.y < limites.max.y - margenEntrada;

        if (dentro)
        {
            int enemigos = ContarEnemigos();

            if (enemigos > 0)
            {
                combateIniciado = true;
                enemigosDetectados = true;

                CambiarPuertas(true);

                Debug.Log("Jugador dentro. Sala cerrada.");
            }
            else
            {
                salaCompletada = true;
                CambiarPuertas(false);
            }
        }
    }

    // Revisar enemigos vivos
    if (!combateIniciado)
        return;

    int enemigosVivos = ContarEnemigos();

    if (enemigosDetectados && enemigosVivos == 0)
    {
        salaCompletada = true;
        combateIniciado = false;

        CambiarPuertas(false);

        Debug.Log("Sala completada.");
    }
}


    int ContarEnemigos()
    {
        Collider2D[] objetos = Physics2D.OverlapBoxAll(
            zona.bounds.center,
            zona.bounds.size,
            0f,
            capasDeteccion
        );

        HashSet<GameObject> enemigos =
            new HashSet<GameObject>();

        foreach (Collider2D objeto in objetos)
        {
            Transform actual = objeto.transform;

            while (actual != null)
            {
                if (actual.CompareTag(tagEnemigo))
                {
                    enemigos.Add(actual.gameObject);
                    break;
                }

                actual = actual.parent;
            }
        }

        return enemigos.Count;
    }

    void CambiarPuertas(bool cerrar)
    {
        foreach (GameObject puerta in puertas)
        {
            if (puerta != null)
                puerta.SetActive(cerrar);
        }
    }

    void OnDrawGizmosSelected()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();

        if (box == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(box.offset, box.size);
    }

    
void OnTriggerExit2D(Collider2D other)
{
    if (other.CompareTag(tagJugador))
    {
        jugadorDentro = null;
    }
}

}
