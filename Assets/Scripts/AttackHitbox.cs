using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;

    private BoxCollider2D boxCollider;

    // Guardamos las referencias de objetos golpeados para no repetirlos en el mismo golpe
    private HashSet<GameObject> objetosGolpeados = new HashSet<GameObject>();

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Se llama cuando comienza un nuevo ataque desde caminar.cs
    public void NuevoAtaque()
    {
        objetosGolpeados.Clear();
    }

    // Usamos OnTriggerStay2D por si la espada se enciende ya estando dentro del enemigo
    private void OnTriggerStay2D(Collider2D collision)
    {
        DeteccionDanio(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DeteccionDanio(collision);
    }

    private void DeteccionDanio(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        // Si este objeto ya recibió daño en este ataque actual, no hacer nada
        if (objetosGolpeados.Contains(collision.gameObject))
            return;

        bool golpeEfectuado = false;

        // 1. Intentar hacer daño a un enemigo normal (script 'enemigo')
        enemigo enemigoComun = collision.GetComponent<enemigo>();
        if (enemigoComun != null)
        {
            enemigoComun.RecibirDanio(damage);
            golpeEfectuado = true;
        }

        // 2. Intentar hacer daño al Caballero Oscuro (script 'CaballeroHealth')
        CaballeroHealth caballero = collision.GetComponent<CaballeroHealth>();
        if (caballero != null)
        {
            caballero.TakeDamage(damage);
            golpeEfectuado = true;
        }

        // Si logró hacerle daño a cualquiera de los dos, guardamos el objeto
        if (golpeEfectuado)
        {
            objetosGolpeados.Add(collision.gameObject);
            Debug.Log("¡Enemigo golpeado!: " + collision.name);
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();

        if (box == null)
            return;

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(
            box.offset,
            box.size
        );

        Gizmos.matrix = Matrix4x4.identity;
    }
}