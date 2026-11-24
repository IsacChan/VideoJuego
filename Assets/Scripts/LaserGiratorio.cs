
using UnityEngine;
using System.Collections;

public class LaserGiratorio : MonoBehaviour
{
    [Header("Anticipación del ataque")]
public float retrasoAntesDelDisparo = 0.5f;

    [Header("Rotación")]
    public float velocidadRotacion = 60f;

    [Header("Duración")]
    public float duracion = 2f;

    [Header("Daño")]
    public float intervaloDanio = 1f;

    [Header("Efectos")]
    public GameObject efectoCarga;
    public GameObject efectoLaser;
    public float duracionCarga = 1f;

    [Header("Collider automático")]
    public BoxCollider2D colliderLaser;
    public SpriteRenderer spriteLaser;
    [Range(0.1f, 1f)]
    public float grosorCollider = 0.8f;

    private Transform jugador;
    private Transform origenDanio;

    private float siguienteDanio;
    private bool activo = false;

    private Coroutine secuenciaActiva;

    public void Inicializar(Transform objetivo, Transform origen)
    {
        jugador = objetivo;
        origenDanio = origen;

        activo = false;

        if (efectoCarga != null)
            efectoCarga.SetActive(true);

        if (efectoLaser != null)
            efectoLaser.SetActive(false);

        if (colliderLaser != null)
            colliderLaser.enabled = false;

        secuenciaActiva = StartCoroutine(SecuenciaLaser());
    }

    private IEnumerator SecuenciaLaser()
{
    // ETAPA 1: mostrar la carga.
    yield return new WaitForSeconds(duracionCarga);

    // ETAPA 2: pequeña anticipación antes del disparo.
    yield return new WaitForSeconds(retrasoAntesDelDisparo);

    // ETAPA 3: mostrar el rayo.
    if (efectoCarga != null)
        efectoCarga.SetActive(false);

    if (efectoLaser != null)
        efectoLaser.SetActive(true);

    // El Animation Event ActivarDanio()
    // activa el daño cuando aparece visualmente el rayo.

    // ETAPA 4: mantener el disparo.
    yield return new WaitForSeconds(duracion);

    DesactivarDanio();

    Destroy(gameObject);
}

    private void Update()
    {
        if (jugador == null)
            return;

        Vector2 direccion = jugador.position - transform.position;

        float angulo = Mathf.Atan2(
            direccion.y,
            direccion.x
        ) * Mathf.Rad2Deg;

        float nuevoAngulo = Mathf.MoveTowardsAngle(
            transform.eulerAngles.z,
            angulo,
            velocidadRotacion * Time.deltaTime
        );

        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            nuevoAngulo
        );
    }

    private void LateUpdate()
    {
        if (activo)
            ActualizarCollider();
    }

    // Calcula el tamaño del collider según el sprite actual.
    private void ActualizarCollider()
    {
        if (colliderLaser == null || spriteLaser == null)
            return;

        if (spriteLaser.sprite == null)
            return;

        Bounds bounds = spriteLaser.sprite.bounds;

        // Convertir los límites del sprite a espacio local
        // del objeto principal del láser.
        Vector3 escala = spriteLaser.transform.lossyScale;
        Vector3 escalaPadre = transform.lossyScale;

        float escalaX = escala.x / escalaPadre.x;
        float escalaY = escala.y / escalaPadre.y;

        colliderLaser.size = new Vector2(
            bounds.size.x * Mathf.Abs(escalaX),
            bounds.size.y * Mathf.Abs(escalaY) * grosorCollider
        );

        Vector3 centroMundo =
            spriteLaser.transform.TransformPoint(bounds.center);

        Vector3 centroLocal =
            transform.InverseTransformPoint(centroMundo);

        colliderLaser.offset = new Vector2(
            centroLocal.x,
            centroLocal.y
        );
    }

    // Animation Event: primer fotograma visible del rayo.
    public void ActivarDanio()
    {
        if (activo)
            return;

        activo = true;
        siguienteDanio = 0f;

        ActualizarCollider();

        if (colliderLaser != null)
            colliderLaser.enabled = true;

        Debug.Log("DAÑO DEL LÁSER ACTIVADO");
    }

    // Desactiva el daño cuando termina el disparo.
    public void DesactivarDanio()
    {
        activo = false;

        if (colliderLaser != null)
            colliderLaser.enabled = false;

        Debug.Log("DAÑO DEL LÁSER DESACTIVADO");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!activo || Time.time < siguienteDanio)
            return;

        caminar jugadorScript =
            other.GetComponentInParent<caminar>();

        if (jugadorScript == null)
            return;

        jugadorScript.RecibirDanioRaices(
            origenDanio != null ? origenDanio : transform
        );

        siguienteDanio = Time.time + intervaloDanio;

        Debug.Log("JUGADOR ALCANZADO POR EL LÁSER");
    }

    private void OnDisable()
    {
        activo = false;

        if (colliderLaser != null)
            colliderLaser.enabled = false;
    }
}
