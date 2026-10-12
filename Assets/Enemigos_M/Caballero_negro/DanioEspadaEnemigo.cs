using UnityEngine;

public class DanioEspadaEnemigo : MonoBehaviour
{
    [Header("Configuración de Daño")]
    public int danoAtaque = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si lo que tocó el hitbox es el Jugador
        if (collision.CompareTag("Player"))
        {
            Debug.Log("¡La espada del caballero golpeó al jugador!");

            // Llama directamente al método de daño que ya tiene tu script "caminar"
            // y le pasa la posición del hitbox para calcular la dirección del rebote.
            collision.SendMessage("RecibirDanioRaices", transform, SendMessageOptions.DontRequireReceiver);
        }
    }
}