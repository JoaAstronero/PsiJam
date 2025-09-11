using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // ====> VARIABLES <====

    // --- Referencias de Componentes ---
    private PlayerInput playerInput;
    private CharacterController controller;
    private Transform cameraTransform;

    // --- Referencias de Input Actions ---
    private InputAction moveAction;
    private InputAction lookAction;

    // --- Parámetros de Movimiento ---
    [Header("Movement Parameters")]
    [Tooltip("Speed at which the player moves.")]
    public float moveSpeed = 5f;
    [Tooltip("Gravity's effect on the player.")]
    public float gravityValue = -9.81f;

    // --- Parámetros de Cámara ---
    [Header("Camera Parameters")]
    [Tooltip("Sensitivity for camera rotation.")]
    public float lookSensitivity = 2f;

    // --- Variables Internas ---
    private Vector3 playerVelocity;
    private float xRotation = 0f;

    // --- Métodos de Ciclo de Vida ---
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;

        // Deshabilitar todos los Action Maps y habilitar el de "Player"
        foreach (var map in playerInput.actions.actionMaps)
        {
            map.Disable();
        }
        playerInput.actions.FindActionMap("Player").Enable();
        playerInput.actions.FindActionMap("Interaction").Enable();

        // Obtener las acciones de entrada
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];

    }

    private void Update()
    {
        HandleMovement();
        HandleCameraLook();
    }

    // --- Lógica del Juego ---
    private void HandleMovement()
    {
        // 🔹 Lógica de Gravedad
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Frenar la caída cuando toca el suelo
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;

        // Aplicar movimiento horizontal
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Aplicar gravedad
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void HandleCameraLook()
    {
        Vector2 mouseDelta = lookAction.ReadValue<Vector2>() * lookSensitivity;

        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseDelta.x);
    }

}