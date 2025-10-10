using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DrawerInteraction : MonoBehaviour
{
    [Header("Configuración general")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float raycastMaxDistance = 2f;
    [SerializeField] private LayerMask drawerLayer;
    [SerializeField] private float dragSensitivity = 0.005f;
    [SerializeField] private UIController uiController;

    private Drawer currentDrawer = null;
    private bool isDragging = false;
    private float currentOffset = 0f;
    private float lastMouseY;

    void Update()
    {
        bool isHovering = false;

        // --- Detectar si el jugador apunta a un cajón ---
        if (!isDragging)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastMaxDistance, drawerLayer))
            {
                Drawer drawer = hit.collider.GetComponent<Drawer>();
                if (drawer != null)
                {
                    isHovering = true;

                    // Si hace clic, comienza la interacción
                    if (Input.GetMouseButtonDown(0))
                    {
                        currentDrawer = drawer;
                        lastMouseY = Input.mousePosition.y;
                        isDragging = true;
                        CursorManager.SetCursorState(false, false);

                        uiController?.ShowDrawerHoverIcon(false);
                        uiController?.ShowDrawerActiveIcon(true);
                    }
                }
            }
        }

        // Mostrar icono de hover solo si apunta a un cajón
        uiController?.ShowDrawerHoverIcon(isHovering && !isDragging);

        // --- Arrastre activo ---
        if (isDragging && currentDrawer != null)
        {
            float mouseDelta = Input.mousePosition.y - lastMouseY;
            lastMouseY = Input.mousePosition.y;

            currentOffset += mouseDelta * dragSensitivity;
            currentOffset = Mathf.Clamp(currentOffset, 0f, currentDrawer.maxExtension);

            currentDrawer.SetDrawerPosition(currentOffset);
        }

        // --- Soltar ---
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            currentDrawer = null;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            uiController?.ShowDrawerActiveIcon(false);
        }
    }
}