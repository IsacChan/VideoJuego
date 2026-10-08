
using UnityEngine;
using System.Collections.Generic;

public class DanioRaices : MonoBehaviour
{
    [Header("Zona de daño")]
    public Collider2D zonaDanio;

    private bool activa = false;

    private HashSet<caminar> jugadoresGolpeados =
        new HashSet<caminar>();

    private void Awake()
    {
        if (zonaDanio == null)
            zonaDanio = GetComponent<Collider2D>();

        if (zonaDanio != null)
        {
            zonaDanio.isTrigger = true;
            zonaDanio.enabled = false;
        }
    }

    public void ActivarDanio()
    {
        activa = true;
        jugadoresGolpeados.Clear();

        if (zonaDanio == null)
        {
            Debug.LogError(
                "La raíz no tiene Collider2D."
            );
            return;
        }

        zonaDanio.enabled = true;

        // Detectar jugadores que ya están dentro
        ContactFilter2D filtro = new ContactFilter2D();
        filtro.NoFilter();

        List<Collider2D> contactos = new List<Collider2D>();

        Physics2D.SyncTransforms();
        zonaDanio.Overlap(filtro, contactos);

        foreach (Collider2D contacto in contactos)
        {
            IntentarDanio(contacto);
        }
    }

    public void DesactivarDanio()
    {
        activa = false;

        if (zonaDanio != null)
            zonaDanio.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IntentarDanio(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        IntentarDanio(other);
    }

    private void IntentarDanio(Collider2D other)
    {
        if (!activa)
            return;

        caminar jugador = other.GetComponentInParent<caminar>();

        if (jugador == null)
            return;

        if (jugadoresGolpeados.Contains(jugador))
            return;

        jugadoresGolpeados.Add(jugador);

        Debug.Log("¡Las raíces golpearon al jugador!");

        jugador.RecibirDanioRaices(transform);
    }
}
