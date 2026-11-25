
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

        // Guardar la posición exacta de la muerte.
        Vector3 posicionMuerte = transform.position;

        Debug.Log(
            "Jefe final derrotado. Posición: " +
            posicionMuerte
        );

        // Notificar a la sala.
        if (salaJefe != null)
        {
            salaJefe.NotificarJefeDerrotado();

            // Desplegar la piedra donde murió el jefe.
            salaJefe.DesplegarPiedra(posicionMuerte);
        }
        else
        {
            Debug.LogError(
                "No se asignó SalaJefe al controlador."
            );
        }

        // La desaparición del jefe sigue dependiendo
        // de CaballeroHealth y su Animation Event.
    }
}
