using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoomManager : MonoBehaviour
{
    [Header("Configuración de Spawner (Enemigos y Puntos)")]
    [SerializeField] private List<GameObject> enemigosPrefabs = new List<GameObject>(); 
    [SerializeField] private List<Transform> puntosDeSpawn = new List<Transform>();    

    [Header("Cofre de Recompensa")]
    [SerializeField] private GameObject cofreDeSala; 

    private SpriteRenderer spriteCofre;
    private Collider2D colisionCofre;

    private List<GameObject> enemigosInstanciados = new List<GameObject>();
    private bool salaIniciada = false;
    private bool cofreActivado = false;

    [Header("Sistema de guardado")]
    [SerializeField] private string idSala = ""; // Se autogenera limpio por escena

    private void Awake()
    {
        // ⚠️ QUITAMOS transform.position = Vector3.zero; para que cada sala 
        // se quede exactamente en el lugar del mapa donde la posicionaste.

        // Genera el ID automático (Ej: Mapa_2_Sala_01), sin afectar al Nivel 1
        if (string.IsNullOrEmpty(idSala))
        {
            idSala = SceneManager.GetActiveScene().name + "_" + gameObject.name;
        }
    }

    private void Start()
    {
        if (cofreDeSala != null)
        {
            spriteCofre = cofreDeSala.GetComponent<SpriteRenderer>();
            colisionCofre = cofreDeSala.GetComponent<Collider2D>();

            if (spriteCofre != null)
                spriteCofre.enabled = false;

            if (colisionCofre != null)
                colisionCofre.enabled = false;
        }

        // Usamos la corrutina para esperar a que la escena cargue por completo tras el portal
        StartCoroutine(InicializarSalaDespuesDeCargar());
    }

    private IEnumerator InicializarSalaDespuesDeCargar()
    {
        yield return null; // Espera un frame de respiro

        // Comprobar si esta sala ya fue completada
        if (ProgresoMundo.EstaCompletado(idSala))
        {
            salaIniciada = true;
            cofreActivado = true;

            RestaurarCofre();

            Debug.Log("Sala completada restaurada: " + idSala);
            yield break;
        }

        // Sala pendiente: generar enemigos
        SpawnearEnemigos();
    }

    private void SpawnearEnemigos()
    {
        if (enemigosPrefabs.Count == 0 || puntosDeSpawn.Count == 0) return;

        foreach (Transform punto in puntosDeSpawn)
        {
            if (punto != null && enemigosPrefabs.Count > 0)
            {
                GameObject prefabAleatorio = enemigosPrefabs[Random.Range(0, enemigosPrefabs.Count)];
                GameObject enemigoCreado = Instantiate(prefabAleatorio, punto.position, Quaternion.identity);
                enemigosInstanciados.Add(enemigoCreado);
            }
        }
        salaIniciada = true;
        Debug.Log("⚔️ Enemigos nacidos en sala: " + idSala);
    }

    void Update()
    {
        if (!salaIniciada || cofreActivado) return;

        // Limpia a los enemigos muertos
        enemigosInstanciados.RemoveAll(enemigo => enemigo == null);

        // Si ya no quedan enemigos vivos
        if (enemigosInstanciados.Count == 0)
        {
            LiberarCofre();
        }
    }

    private void LiberarCofre()
    {
        cofreActivado = true;

        ChestRoomKey llave = cofreDeSala != null ? cofreDeSala.GetComponent<ChestRoomKey>() : null;

        if (llave != null && llave.LlaveRecogida)
        {
            cofreDeSala.SetActive(false);
            return;
        }

        if (cofreDeSala != null)
        {
            if (spriteCofre != null) spriteCofre.enabled = true;
            if (colisionCofre != null) colisionCofre.enabled = true;

            Debug.Log("¡Sala despejada! El cofre ha aparecido en " + idSala);
        }
    }
    
    private void RestaurarCofre()
    {
        if (cofreDeSala == null)
            return;

        ChestRoomKey llave = cofreDeSala.GetComponent<ChestRoomKey>();

        if (llave != null && llave.LlaveRecogida)
        {
            cofreDeSala.SetActive(false);
            Debug.Log("Cofre especial ya recogido en " + idSala);
            return;
        }

        if (spriteCofre != null)
            spriteCofre.enabled = true;

        if (colisionCofre != null)
            colisionCofre.enabled = true;
    }
}