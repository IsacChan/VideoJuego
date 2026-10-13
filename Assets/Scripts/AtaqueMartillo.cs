
using UnityEngine;
using System.Collections;

public class AtaqueMartillo : MonoBehaviour
{
    [Header("Zona de impacto")]
    public Collider2D hitboxMartillo;

    [Header("Tiempo del golpe")]
    public float retrasoGolpe = 0.3f;
    public float duracionGolpe = 0.2f;

    private Coroutine ataqueActual;
    private bool golpeActivo;
    private bool jugadorGolpeado;

    private void Start()
    {
        if (hitboxMartillo != null)
        {
            hitboxMartillo.isTrigger = true;
            hitboxMartillo.enabled = false;
        }
    }

    public void EjecutarAtaque()
    {
        if (ataqueActual != null)
            return;

        ataqueActual = StartCoroutine(Golpear());
    }

    private IEnumerator Golpear()
    {
        jugadorGolpeado = false;

        yield return new WaitForSeconds(retrasoGolpe);

        golpeActivo = true;

        if (hitboxMartillo != null)
            hitboxMartillo.enabled = true;

        yield return new WaitForSeconds(duracionGolpe);

        DesactivarGolpe();
        ataqueActual = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IntentarGolpear(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        IntentarGolpear(other);
    }

    private void IntentarGolpear(Collider2D other)
    {
        if (!golpeActivo || jugadorGolpeado)
            return;

        caminar jugador = other.GetComponentInParent<caminar>();

        if (jugador == null)
            return;

        jugadorGolpeado = true;

        jugador.RecibirDanioRaices(transform);

        Debug.Log("El martillo golpeó al jugador.");
    }

    private void DesactivarGolpe()
    {
        golpeActivo = false;

        if (hitboxMartillo != null)
            hitboxMartillo.enabled = false;
    }

    public void CancelarAtaque()
    {
        if (ataqueActual != null)
        {
            StopCoroutine(ataqueActual);
            ataqueActual = null;
        }

        DesactivarGolpe();
    }
}
