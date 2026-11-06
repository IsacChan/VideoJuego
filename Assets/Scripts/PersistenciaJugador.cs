using UnityEngine;

public class PersistenciaJugador: MonoBehaviour
{
    private static PersistenciaJugador instancia;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject); // Hace que este objeto no se destruya al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Evita duplicados si regresas al nivel 1
        }
    }
}