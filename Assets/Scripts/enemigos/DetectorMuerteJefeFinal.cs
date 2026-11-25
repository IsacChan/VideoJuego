
using UnityEngine;

public class DetectorMuerteJefeFinal : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animatorJefe;
    [SerializeField] private RecompensaJefeFinal recompensaJefe;

    private bool derrotaNotificada = false;

    private void Awake()
    {
        if (animatorJefe == null)
            animatorJefe = GetComponent<Animator>();

        if (animatorJefe == null)
        {
            Debug.LogError("No se encontró el Animator del jefe final.");
            enabled = false;
            return;
        }

        if (recompensaJefe == null)
        {
            Debug.LogError("No se asignó RecompensaJefeFinal.");
            enabled = false;
        }
    }

    private void Update()
    {
        if (derrotaNotificada)
            return;

        bool estaMuriendo = false;

        // Comprobar el estado actual.
        if (animatorJefe.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            estaMuriendo = true;
        }

        // Comprobar si está entrando al estado Death.
        if (animatorJefe.IsInTransition(0))
        {
            AnimatorStateInfo siguiente =
                animatorJefe.GetNextAnimatorStateInfo(0);

            if (siguiente.IsName("Death"))
            {
                estaMuriendo = true;
            }
        }

        if (estaMuriendo)
        {
            derrotaNotificada = true;

            recompensaJefe.NotificarDerrota();

            Debug.Log("Se detectó la derrota del jefe final.");
        }
    }
}

