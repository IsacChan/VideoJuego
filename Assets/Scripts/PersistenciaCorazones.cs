using UnityEngine;

public class PersistenciaCorazones : MonoBehaviour
{
    private static PersistenciaCorazones instance;

    void Awake()
    {
        // Esto hace exactamente lo mismo que el inventario: evita que se destruya al cambiar de escena
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // El canvas de los corazones se vuelve inmortal entre escenas
        }
        else
        {
            Destroy(gameObject); // Si ya existe uno en el nuevo nivel, destruye el duplicado para no chocar
        }
    }
}