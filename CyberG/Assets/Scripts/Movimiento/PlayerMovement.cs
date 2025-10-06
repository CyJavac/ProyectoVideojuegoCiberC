using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CharacterController controller; // Asigna el del CuerpoJugador
    [SerializeField] private Transform camara;                // Asigna Main Camera

    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadMovimiento = 3.0f;
    [SerializeField] private float velocidadAgachado = 1.5f;
    [SerializeField] private float alturaNormal = 2.0f;
    [SerializeField] private float alturaAgachado = 1.0f;
    [SerializeField] private float suavizadoAgacharse = 8.0f;
    [SerializeField] private float gravedad = -9.81f;

    private bool estaAgachado = false;
    private float alturaObjetivo;
    private float velocidadVertical = 0f;
    private bool puedeMoverse = true;


    void Start()
    {
        if (controller == null)
            Debug.LogWarning("⚠️ Falta asignar el CharacterController en el Inspector.");

        alturaObjetivo = alturaNormal;
    }

    void Update()
    {
        // Bloquea movimiento si el juego está pausado o si el jugador está en modo zoom (pantalla activa)
        if (PauseMenuManager.IsGamePaused() || ScreenInteraction.IsZoomed())
            return;

        MoverJugador();
        ControlAgacharse();
    }

    private void MoverJugador()
    {
        // Leer entrada del jugador
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Movimiento relativo a la dirección de la cámara (en plano horizontal)
        Vector3 direccion = camara.forward * vertical + camara.right * horizontal;
        direccion.y = 0f;
        direccion.Normalize(); // Evita deslizamientos

        float velocidadActual = estaAgachado ? velocidadAgachado : velocidadMovimiento;

        // Aplicar gravedad manual
        if (controller.isGrounded)
        {
            if (velocidadVertical < 0f)
                velocidadVertical = -2f; // Mantener pegado al piso
        }
        else
        {
            velocidadVertical += gravedad * Time.deltaTime;
        }

        // Movimiento total
        Vector3 movimiento = direccion * velocidadActual + Vector3.up * velocidadVertical;
        controller.Move(movimiento * Time.deltaTime);
    }

    private void ControlAgacharse()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            estaAgachado = !estaAgachado;
            alturaObjetivo = estaAgachado ? alturaAgachado : alturaNormal;
        }

        // Transición suave entre alturas
        controller.height = Mathf.Lerp(controller.height, alturaObjetivo, Time.deltaTime * suavizadoAgacharse);

        // Ajuste de cámara
        Vector3 camPos = camara.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, estaAgachado ? 0.5f : 1.0f, Time.deltaTime * suavizadoAgacharse);
        camara.localPosition = camPos;
    }
    
    public void SetMovimientoActivo(bool estado)
    {
        puedeMoverse = estado;
    }
}