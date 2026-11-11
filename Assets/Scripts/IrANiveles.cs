using UnityEngine;
using UnityEngine.SceneManagement;

public class IrANiveles : MonoBehaviour
{
    [Header("Escena de Destino")]
    public string nombreMapaDestino = "Mapa_2"; // Nombre exacto del siguiente nivel

    public void CambiarAMapaDos()
    {
        // Cargamos el siguiente nivel de manera limpia desde nuestra escena de historia dedicada
        SceneManager.LoadScene(nombreMapaDestino);
    }
}