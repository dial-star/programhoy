
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ControlJugador2D : MonoBehaviour
{
    [Header("Movimiento y Salto")]
    public float velocidad = 5f;
    public float fuerzaSalto = 8f;

    [Header("Sonido de Golpe")]
    public AudioSource audioSource;
    public AudioClip sonidoGolpe;

    [Header("Caida y Mensaje")]
    public GameObject mensajeCaida;

    private Rigidbody2D rb;
    private bool enSuelo = false;
    private bool yaCayo = false;

    void Start()
    {
        // Obtener Rigidbody2D de Lisa
        rb = GetComponent<Rigidbody2D>();

        // Obtener AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Ocultar mensaje al comenzar
        if (mensajeCaida != null)
        {
            mensajeCaida.SetActive(false);
        }
    }

    void Update()
    {
        // No permitir movimiento si Lisa ya cayo
        if (yaCayo || Keyboard.current == null)
            return;

        float movimiento = 0f;

        // FLECHA ARRIBA: ir hacia la izquierda
        if (Keyboard.current.upArrowKey.isPressed)
        {
            movimiento = -1f;
        }

        // ESPACIO: avanzar hacia la derecha
        if (Keyboard.current.spaceKey.isPressed)
        {
            movimiento = 1f;
        }

        // Aplicar movimiento horizontal
        rb.linearVelocity = new Vector2(
            movimiento * velocidad,
            rb.linearVelocity.y
        );

        // LETRA P: saltar
        if (Keyboard.current.pKey.wasPressedThisFrame && enSuelo)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                fuerzaSalto
            );

            enSuelo = false;
        }
    }

    // Detectar cuando Lisa toca suelo u obstaculos
    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool esSuelo =
            collision.gameObject.CompareTag("Suelo");

        bool esObstaculo =
            collision.gameObject.CompareTag("Obstaculo");

        // Comprobar si Lisa aterriza encima
        if (esSuelo || esObstaculo)
        {
            foreach (ContactPoint2D contacto in collision.contacts)
            {
                if (contacto.normal.y > 0.5f)
                {
                    enSuelo = true;
                }
            }
        }

        // Reproducir sonido al chocar con obstaculos
        if (esObstaculo)
        {
            ReproducirGolpe();
        }
    }

    // Detectar cuando Lisa deja de tocar una superficie
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo") ||
            collision.gameObject.CompareTag("Obstaculo"))
        {
            enSuelo = false;
        }
    }

    // Reproducir el sonido de golpe
    private void ReproducirGolpe()
    {
        if (audioSource != null && sonidoGolpe != null)
        {
            audioSource.PlayOneShot(sonidoGolpe);
        }
    }

    // Detectar si Lisa cae al hueco
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ZonaCaida") && !yaCayo)
        {
            yaCayo = true;

            // Detener a Lisa
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;

            // Mostrar mensaje
            if (mensajeCaida != null)
            {
                mensajeCaida.SetActive(true);

                TMP_Text texto =
                    mensajeCaida.GetComponent<TMP_Text>();

                if (texto != null)
                {
                    texto.text =
                        "nooo TE CAISTE HASTA NUNCA";
                }
            }

            Debug.Log("Lisa se cayo al hueco");
        }
    }
}
