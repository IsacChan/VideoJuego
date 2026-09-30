using UnityEngine;
using System.Collections.Generic;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;

    private BoxCollider2D boxCollider;

    // Enemigos golpeados durante el ataque actual
    private HashSet<enemigo> enemigosGolpeados = new HashSet<enemigo>();

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Se llama cuando comienza un nuevo ataque
    public void NuevoAtaque()
    {
        enemigosGolpeados.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        enemigo enemigo = collision.GetComponent<enemigo>();

        if (enemigo == null)
            return;

        // Si ya recibió daño durante este ataque, no hacemos nada
        if (enemigosGolpeados.Contains(enemigo))
            return;

        // Hacemos daño
        enemigo.RecibirDanio(damage);

        // Guardamos al enemigo para no volver a dañarlo
        enemigosGolpeados.Add(enemigo);
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