using UnityEngine;

public class ChestManager : MonoBehaviour
{
    [Header("Configuración de Aparición")]
    public GameObject chestPrefab;          // El Prefab de tu cofre
    public Transform[] spawnPoints;         // Lista de puntos en el mapa

    void Start()
    {
        SpawnAllChests();
    }

    void SpawnAllChests()
    {
        // Recorre todos los puntos y coloca un cofre al iniciar el nivel
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            SpawnChestAt(i);
        }
    }

    public void SpawnChestAt(int index)
    {
        if (spawnPoints.Length == 0) return;

        Transform selectedPoint = spawnPoints[index];
        GameObject newChest = Instantiate(chestPrefab, selectedPoint.position, Quaternion.identity);
        
        ChestController controller = newChest.GetComponent<ChestController>();
        if (controller != null)
        {
            controller.spawnIndex = index;
        }
    }
}