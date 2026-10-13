using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        if (cofreDeSala != null)
        {
            // Obtenemos los componentes visuales y de físicas del cofre
            spriteCofre = cofreDeSala.GetComponent<SpriteRenderer>();
            colisionCofre = cofreDeSala.GetComponent<Collider2D>();

            // Ocultamos el cofre y desactivamos su colisión al iniciar
            if (spriteCofre != null) spriteCofre.enabled = false;
            if (colisionCofre != null) colisionCofre.enabled = false;
        }

        SpawnearEnemigos();
    }

    private void SpawnearEnemigos()
    {
        if (enemigosPrefabs.Count == 0) return;

        foreach (Transform punto in puntosDeSpawn)
        {
            if (punto != null)
            {
                GameObject prefabAleatorio = enemigosPrefabs[Random.Range(0, enemigosPrefabs.Count)];
                GameObject enemigoCreado = Instantiate(prefabAleatorio, punto.position, Quaternion.identity);
                enemigosInstanciados.Add(enemigoCreado);
            }
        }
        salaIniciada = true;
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

        if (cofreDeSala != null)
        {
            // Hacemos visible el cofre y activamos su colisión para poder abrirlo
            if (spriteCofre != null) spriteCofre.enabled = true;
            if (colisionCofre != null) colisionCofre.enabled = true;

            Debug.Log("¡Sala despejada! El cofre ha aparecido.");
        }
    }
}