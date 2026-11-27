
using UnityEngine;

public class ControladorJefeSala : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SalaJefe salaJefe;
    [SerializeField] private GameObject piedraMagica;

    private Animator animator;

    private bool derrotaNotificada = false;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError(
                "El jefe no tiene Animator."
            );
        }
    }

    void Update()
    {
        if (derrotaNotificada || animator == null)
            return;

        // Detectar la muerte mediante el Animator.
        if (animator.GetBool("IsDead"))
        {
            ProcesarDerrota();
        }
    }

    
void ProcesarDerrota()
{
    if (derrotaNotificada)
        return;

    derrotaNotificada = true;

    Debug.Log(
        "Jefe derrotado. Posición: " + transform.position
    );

    if (salaJefe != null)
    {
        salaJefe.NotificarJefeDerrotado();
    }
    else
    {
        Debug.LogError(
            "No se asignó SalaJefe al controlador."
        );
    }
}

}
