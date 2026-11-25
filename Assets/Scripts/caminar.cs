using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class caminar : MonoBehaviour
{
    public float speed = 5f;

    [Header("Sistema de carrera")]
    public float velocidadCarrera = 8f;
    public float duracionCarrera = 3f;
    public float tiempoRecargaCarrera = 5f;

    private float tiempoCarreraRestante;
    private float tiempoRecargaRestante;

    private bool corriendo = false;

    private Rigidbody2D rb2D;
    private Vector2 movementInput;

    // Última dirección en la que estaba mirando el personaje
    private Vector2 ultimaDireccion = Vector2.down;

    private Animator animator;

    private bool atacando;

    // =========================
    // DAÑO Y REBOTE
    // =========================

    public float fuerzaRebote = 8f;

    public float duracionRebote = 0.15f;
    public float tiempoParalizado = 2f;

    private bool recibiendoDanio = false;

    private SpriteRenderer spriteRenderer;

    public float tiempoParpadeo = 0.1f;

    public GameObject attackHitbox;

    public float distanciaAtaque = 0.8f;

    [Header("Invulnerabilidad (Moneda)")]
    public bool esInvulnerable { get; private set; }
    private Coroutine invulnerabilidadMonedaRoutine;

    [Header("UI Invulnerabilidad")]
    public GameObject contenedorInvulnerabilidad; // Opcional: panel o padre del texto
    public TextMeshProUGUI textoInvulnerabilidad; // Arrastra aquí tu TextMeshProUGUI

    [Header("Fuerza (Poción Azul)")]
    public float multiplicadorFuerza { get; private set; } = 1f;
    public bool tieneFuerzaActiva => multiplicadorFuerza > 1f;
    private Coroutine fuerzaPocionRoutine;

    [Header("UI Fuerza")]
    public GameObject contenedorFuerza; // Opcional: panel o padre del texto
    public TextMeshProUGUI textoFuerza; // Arrastra aquí tu TextMeshProUGUI

    [Header("Ataque automático")]
    public float radioDeteccionAtaque = 3f;

    private Vector2 direccionAtaque = Vector2.down;

    public GameObject slashEffect;

    [Header("Vida")]
    public int vidaMaxima = 20; // 5 corazones * 4 puntos fraccionarios cada uno
    public int vidaActual;

    private bool muerto = false;

    private VidaJugador sistemaRespawn;

    [Header("UI de Corazones")]
    public ControladorCorazones controladorCorazones; // Arrastra aquí tu ContenedorCorazones desde Unity

    private EmbestidaJugador sistemaEmbestida;


    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        spriteRenderer = GetComponent<SpriteRenderer>();

        OcultarUIInvulnerabilidad();
        OcultarUIFuerza();

        // Buscamos automáticamente el controlador de corazones de forma global (incluso en DontDestroyOnLoad)
        if (controladorCorazones == null)
        {
            controladorCorazones = Object.FindFirstObjectByType<ControladorCorazones>();
        }

        // =========================================================================
        // CORRECCIÓN: Si venimos de otro nivel, recuperamos la vida real transportada
        // =========================================================================
        if (DatosTransporte.datosInicializados)
        {
            RestaurarVidaGuardada((int)DatosTransporte.vidaJugador);
        }
        else
        {
            vidaActual = vidaMaxima;
            if (controladorCorazones != null)
            {
                controladorCorazones.ActualizarVida(vidaActual, false);
            }
        }

        sistemaRespawn = GetComponent<VidaJugador>();
        sistemaEmbestida = GetComponent<EmbestidaJugador>();

        tiempoCarreraRestante = duracionCarrera;
    }


    void Update()
    {
        if (muerto)
        {
            movementInput = Vector2.zero;
            return;
        }

        if (sistemaEmbestida != null && sistemaEmbestida.EstaEmbistiendo)
        {
            movementInput = Vector2.zero;

            animator.SetFloat("Speed", 0);
            animator.SetBool("atacar", false);

            return;
        }

        if (recibiendoDanio)
        {
            movementInput = Vector2.zero;

            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
            animator.SetFloat("Speed", 0);

            return;
        }

        if (atacando)
        {
            movementInput = Vector2.zero;
            return;
        }

        movementInput = Vector2.zero;

        // =========================
        // MOVIMIENTO
        // =========================

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed)
            {
                movementInput.x = -1;
            }

            if (Keyboard.current.dKey.isPressed ||
                Keyboard.current.rightArrowKey.isPressed)
            {
                movementInput.x = 1;
            }

            if (Keyboard.current.wKey.isPressed ||
                Keyboard.current.upArrowKey.isPressed)
            {
                movementInput.y = 1;
            }

            if (Keyboard.current.sKey.isPressed ||
                Keyboard.current.downArrowKey.isPressed)
            {
                movementInput.y = -1;
            }

            // =========================
            // ATAQUE
            // =========================

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Atacando();
            }
        }

        movementInput = movementInput.normalized;

        ActualizarCarrera();

        // =========================
        // GUARDAR ÚLTIMA DIRECCIÓN
        // =========================

        if (movementInput != Vector2.zero)
        {
            ultimaDireccion = movementInput;
        }

        if (!atacando)
        {
            direccionAtaque = ultimaDireccion;
        }

        // =========================
        // ANIMATOR
        // =========================

        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);
        animator.SetFloat("Speed", movementInput.magnitude);
        animator.SetBool("Corriendo", corriendo);

        // Dirección del ataque
        animator.SetFloat("AttackX", direccionAtaque.x);
        animator.SetFloat("AttackY", direccionAtaque.y);

        animator.SetBool("atacar", atacando);
    }


    void FixedUpdate()
    {
        // Jugador muerto
        if (muerto)
        {
            rb2D.linearVelocity = Vector2.zero;
            return;
        }

        // La embestida controla el Rigidbody2D
        if (sistemaEmbestida != null && sistemaEmbestida.EstaEmbistiendo)
        {
            return;
        }

        // Permitir el rebote cuando recibe daño
        if (recibiendoDanio)
        {
            return;
        }

        // No moverse mientras ataca
        if (atacando)
        {
            rb2D.linearVelocity = Vector2.zero;
            return;
        }

        // Movimiento normal o carrera
        float velocidadActual = corriendo ? velocidadCarrera : speed;

        rb2D.linearVelocity = movementInput * velocidadActual;
    }


    // =========================
    // ATAQUE
    // =========================

    private Vector2 BuscarDireccionEnemigo()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(
            transform.position,
            radioDeteccionAtaque
        );

        Transform enemigoCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (Collider2D objeto in objetos)
        {
            Transform actual = objeto.transform;

            while (actual != null && !actual.CompareTag("Enemy"))
            {
                actual = actual.parent;
            }

            if (actual == null || !actual.gameObject.activeInHierarchy)
                continue;

            float distancia = Vector2.Distance(
                transform.position,
                objeto.ClosestPoint(transform.position)
            );

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                enemigoCercano = actual;
            }
        }

        if (enemigoCercano == null)
            return ultimaDireccion;

        Vector2 direccion = (
            enemigoCercano.position - transform.position
        ).normalized;

        if (Mathf.Abs(direccion.x) > Mathf.Abs(direccion.y))
        {
            return direccion.x > 0
                ? Vector2.right
                : Vector2.left;
        }

        return direccion.y > 0
            ? Vector2.up
            : Vector2.down;
    }


    public void Atacando()
    {
        if (atacando || muerto || recibiendoDanio)
            return;

        if (sistemaEmbestida != null && sistemaEmbestida.EstaEmbistiendo)
        {
            return;
        }

        direccionAtaque = BuscarDireccionEnemigo();

        animator.SetFloat("AttackX", direccionAtaque.x);
        animator.SetFloat("AttackY", direccionAtaque.y);

        movementInput = Vector2.zero;
        rb2D.linearVelocity = Vector2.zero;

        atacando = true;
        animator.SetBool("atacar", true);
    }


    public void EjecutarGolpe()
    {
        if (muerto || recibiendoDanio || (sistemaEmbestida != null && sistemaEmbestida.EstaEmbistiendo))
        {
            return;
        }

        if (attackHitbox == null)
            return;

        attackHitbox.transform.localPosition = direccionAtaque * distanciaAtaque;

        AttackHitbox hitbox = attackHitbox.GetComponent<AttackHitbox>();

        if (hitbox != null)
        {
            hitbox.NuevoAtaque();
        }

        attackHitbox.SetActive(true);

        if (slashEffect != null)
        {
            SlashEffect slash = slashEffect.GetComponent<SlashEffect>();

            if (slash != null)
            {
                slash.Mostrar(direccionAtaque);
            }
        }
    }


    public void TerminarGolpe()
    {
        attackHitbox.SetActive(false);
    }


    public void NoAtacando()
    {
        atacando = false;
        attackHitbox.SetActive(false);
    }


    // =========================
    // INVULNERABILIDAD (MONEDA)
    // =========================

    public void ActivarInvulnerabilidadMoneda(float duracion = 5f)
    {
        if (muerto) return;

        if (invulnerabilidadMonedaRoutine != null)
        {
            StopCoroutine(invulnerabilidadMonedaRoutine);
        }

        invulnerabilidadMonedaRoutine = StartCoroutine(InvulnerabilidadMonedaCoroutine(duracion));
    }

    private IEnumerator InvulnerabilidadMonedaCoroutine(float duracion)
    {
        esInvulnerable = true;
        Color colorOriginal = spriteRenderer != null ? spriteRenderer.color : Color.white;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0.92f, 0.45f, 1f);
        }

        if (contenedorInvulnerabilidad != null)
        {
            contenedorInvulnerabilidad.SetActive(true);
        }

        if (textoInvulnerabilidad != null)
        {
            textoInvulnerabilidad.gameObject.SetActive(true);
        }

        float tiempoRestante = duracion;
        int ultimoSegundoMostrado = -1;

        while (tiempoRestante > 0f)
        {
            int segundoActual = Mathf.CeilToInt(tiempoRestante);
            segundoActual = Mathf.Clamp(segundoActual, 0, Mathf.CeilToInt(duracion));

            if (segundoActual != ultimoSegundoMostrado)
            {
                ultimoSegundoMostrado = segundoActual;
                if (textoInvulnerabilidad != null)
                {
                    textoInvulnerabilidad.text = $"Invulnerabilidad: {segundoActual}";
                }
            }

            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        if (textoInvulnerabilidad != null)
        {
            textoInvulnerabilidad.text = "Invulnerabilidad: 0";
        }

        yield return new WaitForSeconds(0.15f);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = colorOriginal;
        }

        OcultarUIInvulnerabilidad();

        esInvulnerable = false;
        invulnerabilidadMonedaRoutine = null;
        Debug.Log("La invulnerabilidad por moneda ha terminado.");
    }

    private void OcultarUIInvulnerabilidad()
    {
        if (textoInvulnerabilidad != null)
        {
            textoInvulnerabilidad.text = "";
            textoInvulnerabilidad.gameObject.SetActive(false);
        }

        if (contenedorInvulnerabilidad != null)
        {
            contenedorInvulnerabilidad.SetActive(false);
        }
    }


    // =========================
    // FUERZA (POCIÓN AZUL)
    // =========================

    public void ActivarFuerzaPocion(float multiplicador = 1.5f, float duracion = 10f)
    {
        if (muerto) return;

        if (fuerzaPocionRoutine != null)
        {
            StopCoroutine(fuerzaPocionRoutine);
        }

        fuerzaPocionRoutine = StartCoroutine(FuerzaPocionCoroutine(multiplicador, duracion));
    }

    private IEnumerator FuerzaPocionCoroutine(float multiplicador, float duracion)
    {
        multiplicadorFuerza = multiplicador;
        Color colorOriginal = spriteRenderer != null ? spriteRenderer.color : Color.white;

        if (spriteRenderer != null && !esInvulnerable)
        {
            spriteRenderer.color = new Color(0.55f, 0.82f, 1f, 1f);
        }

        if (contenedorFuerza != null)
        {
            contenedorFuerza.SetActive(true);
        }

        if (textoFuerza != null)
        {
            textoFuerza.gameObject.SetActive(true);
        }

        float tiempoRestante = duracion;
        int ultimoSegundoMostrado = -1;

        while (tiempoRestante > 0f)
        {
            if (muerto)
            {
                multiplicadorFuerza = 1f;
                fuerzaPocionRoutine = null;
                OcultarUIFuerza();
                yield break;
            }

            int segundoActual = Mathf.CeilToInt(tiempoRestante);
            segundoActual = Mathf.Clamp(segundoActual, 0, Mathf.CeilToInt(duracion));

            if (segundoActual != ultimoSegundoMostrado)
            {
                ultimoSegundoMostrado = segundoActual;
                if (textoFuerza != null)
                {
                    textoFuerza.text = $"Fuerza: {segundoActual}s";
                }
            }

            if (spriteRenderer != null && !esInvulnerable && !recibiendoDanio)
            {
                spriteRenderer.color = new Color(0.55f, 0.82f, 1f, 1f);
            }

            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        if (textoFuerza != null)
        {
            textoFuerza.text = "Fuerza: 0s";
        }

        yield return new WaitForSeconds(0.15f);

        multiplicadorFuerza = 1f;
        fuerzaPocionRoutine = null;
        OcultarUIFuerza();

        if (spriteRenderer != null && !esInvulnerable && !recibiendoDanio)
        {
            spriteRenderer.color = colorOriginal;
        }

        Debug.Log("El efecto de la poción azul de fuerza ha terminado.");
    }

    private void OcultarUIFuerza()
    {
        if (textoFuerza != null)
        {
            textoFuerza.text = "";
            textoFuerza.gameObject.SetActive(false);
        }

        if (contenedorFuerza != null)
        {
            contenedorFuerza.SetActive(false);
        }
    }

    public int CalcularDanioConFuerza(int danioBase)
    {
        int danioFinal = Mathf.RoundToInt(danioBase * multiplicadorFuerza);
        return Mathf.Max(1, danioFinal);
    }


    // =========================
    // RECIBIR DAÑO Y REBOTE
    // =========================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (sistemaEmbestida != null && sistemaEmbestida.EstaEmbistiendo)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            RecibirDanio(collision.transform);
        }
    }


    private void RecibirDanio(Transform enemigo)
    {
        if (esInvulnerable)
        {
            Debug.Log("¡Kael es invulnerable por la moneda! Daño bloqueado.");
            return;
        }

        if (sistemaEmbestida != null && sistemaEmbestida.EstaEmbistiendo)
        {
            return;
        }

        if (recibiendoDanio || muerto)
            return;

        vidaActual -= 1;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        Debug.Log("Vida del jugador: " + vidaActual);

        if (controladorCorazones == null)
        {
            controladorCorazones = Object.FindFirstObjectByType<ControladorCorazones>();
        }

        if (controladorCorazones != null)
        {
            controladorCorazones.ActualizarVida(vidaActual, true);
        }

        recibiendoDanio = true;
        atacando = false;

        attackHitbox.SetActive(false);
        rb2D.linearVelocity = Vector2.zero;

        Vector2 direccionRebote = (transform.position - enemigo.position).normalized;

        rb2D.AddForce(
            direccionRebote * fuerzaRebote,
            ForceMode2D.Impulse
        );

        if (vidaActual <= 0)
        {
            StartCoroutine(ReboteMortal());
            return;
        }

        StartCoroutine(Paralizar());
        StartCoroutine(Parpadear());
        StartCoroutine(DetenerRebote());
    }


    // =========================
    // CORRUTINAS DE ESTADO
    // =========================

    private IEnumerator Paralizar()
    {
        yield return new WaitForSeconds(tiempoParalizado);
        rb2D.linearVelocity = Vector2.zero;
        recibiendoDanio = false;
    }

    private IEnumerator Parpadear()
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoParalizado)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(tiempoParpadeo);

            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(tiempoParpadeo);

            tiempoTranscurrido += tiempoParpadeo * 2f;
        }

        spriteRenderer.enabled = true;
    }

    private IEnumerator DetenerRebote()
    {
        yield return new WaitForSeconds(duracionRebote);
        rb2D.linearVelocity = Vector2.zero;
    }

    private void Morir()
    {
        if (muerto)
            return;

        muerto = true;
        recibiendoDanio = false;
        atacando = false;

        multiplicadorFuerza = 1f;
        fuerzaPocionRoutine = null;

        movementInput = Vector2.zero;
        rb2D.linearVelocity = Vector2.zero;

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        StopAllCoroutines();
        OcultarUIInvulnerabilidad();
        OcultarUIFuerza();

        spriteRenderer.enabled = true;

        animator.SetTrigger("Death");

        if (ControladorDerrota.Instancia != null)
        {
            ControladorDerrota.Instancia.MostrarDerrota();
        }
        else
        {
            Debug.LogError("No existe ControladorDerrota en la escena.");
        }
    }

    private IEnumerator ReboteMortal()
    {
        yield return new WaitForSeconds(duracionRebote);
        rb2D.linearVelocity = Vector2.zero;
        Morir();
    }

    public void RecibirDanioRaices(Transform origen)
    {
        RecibirDanio(origen);
    }

    public void Revivir()
    {
        vidaActual = vidaMaxima;

        muerto = false;
        recibiendoDanio = false;
        atacando = false;

        multiplicadorFuerza = 1f;
        fuerzaPocionRoutine = null;

        movementInput = Vector2.zero;
        rb2D.linearVelocity = Vector2.zero;

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        spriteRenderer.enabled = true;
        OcultarUIInvulnerabilidad();
        OcultarUIFuerza();

        animator.Rebind();
        animator.Update(0f);

        if (controladorCorazones == null)
        {
            controladorCorazones = Object.FindFirstObjectByType<ControladorCorazones>();
        }

        if (controladorCorazones != null)
        {
            controladorCorazones.ActualizarVida(vidaActual, false);
        }
    }

    public bool PuedeUsarHabilidades()
    {
        return !muerto && !recibiendoDanio && !atacando;
    }

    public bool PuedeContinuarEmbestida()
    {
        return !muerto && !recibiendoDanio;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccionAtaque);
    }

    public void RestaurarVidaGuardada(int cantidad)
    {
        vidaActual = Mathf.Clamp(cantidad, 1, vidaMaxima);

        muerto = false;
        recibiendoDanio = false;
        atacando = false;

        movementInput = Vector2.zero;

        if (rb2D != null)
            rb2D.linearVelocity = Vector2.zero;

        if (controladorCorazones == null)
        {
            controladorCorazones = Object.FindFirstObjectByType<ControladorCorazones>();
        }

        if (controladorCorazones != null)
        {
            controladorCorazones.ActualizarVida(vidaActual, false);
        }

        Debug.Log("Vida restaurada correctamente: " + vidaActual);
    }

    // =========================
    // MÉTODO PARA CURAR DESDE EL INVENTARIO
    // =========================
    public void Curar(int cantidadACurar)
    {
        if (muerto) return;

        vidaActual += cantidadACurar;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        if (controladorCorazones == null)
        {
            controladorCorazones = Object.FindFirstObjectByType<ControladorCorazones>();
        }

        if (controladorCorazones != null)
        {
            controladorCorazones.ActualizarVida(vidaActual, false);
        }

        Debug.Log("¡Salud restaurada! Vida actual: " + vidaActual);
    }

    private void ActualizarCarrera()
    {
        // Si está recargando, esperar
        if (tiempoRecargaRestante > 0)
        {
            tiempoRecargaRestante -= Time.deltaTime;

            corriendo = false;

            if (tiempoRecargaRestante <= 0)
            {
                tiempoRecargaRestante = 0;
                tiempoCarreraRestante = duracionCarrera;
            }

            return;
        }

        bool teclaCarrera =
            Keyboard.current != null &&
            (Keyboard.current.leftShiftKey.isPressed ||
             Keyboard.current.rightShiftKey.isPressed);

        // Solo correr si se está moviendo
        corriendo = teclaCarrera &&
                    movementInput != Vector2.zero &&
                    tiempoCarreraRestante > 0;

        if (corriendo)
        {
            tiempoCarreraRestante -= Time.deltaTime;

            // Se terminó la energía
            if (tiempoCarreraRestante <= 0)
            {
                tiempoCarreraRestante = 0;
                corriendo = false;

                tiempoRecargaRestante = tiempoRecargaCarrera;
            }
        }
    }

    public float ProgresoCarrera
    {
        get
        {
            if (tiempoRecargaRestante > 0)
            {
                return 1f - (tiempoRecargaRestante / tiempoRecargaCarrera);
            }

            return duracionCarrera > 0
                ? tiempoCarreraRestante / duracionCarrera
                : 0f;
        }
    }

    public bool EstaRecargandoCarrera
    {
        get { return tiempoRecargaRestante > 0; }
    }
}