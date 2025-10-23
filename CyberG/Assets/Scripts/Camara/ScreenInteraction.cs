using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Canvas[] screenCanvases;
    [SerializeField] private LayerMask screenLayer; // Asignar la capa "Screens" en el Inspector.
    [SerializeField] private float raycastMaxDistance = 2f;
    [SerializeField] private float zoomSpeed = 5f;
    //[SerializeField] private float zoomDistance = 0.5f;
    [SerializeField] private UIController uiController;
    private RectTransform canvasRect; // Referencia al RectTransform del Canvas actual

    //private Vector3 originalCameraPosition;
    //private Quaternion originalCameraRotation;
    private static bool isZoomed = false;
    private int currentScreenIndex = -1;

    private Vector3 preZoomPosition;
    private Quaternion preZoomRotation;



    public static bool IsZoomed() => isZoomed;

    void Start()
    {
        //originalCameraPosition = playerCamera.transform.localPosition;
        //originalCameraRotation = playerCamera.transform.localRotation;

        foreach (Canvas canvas in screenCanvases)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        //if (!isZoomed && Input.GetMouseButtonDown(0))
        if (!isZoomed && Input.GetMouseButtonDown(0) &&
        !PauseMenuManager.IsGamePaused() &&
        !PauseMenuManager.InputBloqueadoTemporalmente)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastMaxDistance, screenLayer))
            {
                for (int i = 0; i < screenCanvases.Length; i++)
                {
                    if (hit.collider.gameObject == screenCanvases[i].GetComponentInParent<Collider>().gameObject)
                    {
                        StartCoroutine(ZoomToScreen(i));
                        break;
                    }
                }
            }
        }

        // // Verifica si se presiona ESC
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     if (isZoomed)
        //     {
        //         StartCoroutine(ReturnFromZoom());
        //     }
        //     else if (!PauseMenuManager.IsGamePaused()) // Solo abre si el juego no está pausado
        //     {
        //         PauseMenuManager.Instance.TogglePause();
        //     }
        // }

        // Verifica si se presiona ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Si está haciendo zoom, permite salir del monitor
            if (isZoomed)
            {
                StartCoroutine(ReturnFromZoom());
            }

            //Si NO está en zoom, ya no debe abrir ni cerrar el menú de pausa
            // Esto ahora lo controla únicamente PauseMenuManager.
        }


        //Temporal:
        if (!isZoomed && !PauseMenuManager.IsGamePaused())
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            // Muestra el tooltip si está mirando un monitor
            bool isLookingAtScreen = Physics.Raycast(ray, out hit, raycastMaxDistance, screenLayer);
            uiController.ShowSelectionTooltip(isLookingAtScreen);

            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * raycastMaxDistance, Color.green);

            if (Input.GetMouseButtonDown(0))
            {
                //Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                //RaycastHit hit;

                if (Physics.Raycast(ray, out hit, raycastMaxDistance, screenLayer))
                {
                    Debug.Log($"Hit: {hit.collider.gameObject.name}"); // Verifica qué monitor se detecta
                }
            }
        }
        else
        {
            // Si está pausado o haciendo zoom, asegúrate de ocultarlo
            uiController.ShowSelectionTooltip(false);
        }
    }

    private System.Collections.IEnumerator ZoomToScreen(int screenIndex)
    {
        // Guarda la posición y rotación ACTUALES antes del zoom
        preZoomPosition = playerCamera.transform.position;
        preZoomRotation = playerCamera.transform.rotation;

        isZoomed = true;
        currentScreenIndex = screenIndex;
        CursorManager.SetCursorState(false, false);

        Transform cameraPivot = screenCanvases[screenIndex].transform.parent.Find("CameraPivot");
        Vector3 targetPosition = cameraPivot.position;
        Quaternion targetRotation = cameraPivot.rotation;


        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            float smoothProgress = Mathf.SmoothStep(0f, 1f, elapsedTime);
            playerCamera.transform.position = Vector3.Lerp(preZoomPosition, targetPosition, smoothProgress);
            playerCamera.transform.rotation = Quaternion.Slerp(preZoomRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * zoomSpeed;
            yield return null;
        }

        screenCanvases[screenIndex].gameObject.SetActive(true);
        CursorManager.LockCursorToGameView(true); // Bloquear cursor dentro de la pantalla
        //Cursor.visible = true; // Fuerza visibilidad (opcional)
        //CursorManager.CenterCursor(); // Bloquear cursor en el centro de la pantalla
        canvasRect = screenCanvases[screenIndex].GetComponent<RectTransform>();
        StartCoroutine(ConfineCursorToCanvas());

        uiController.SetZoomModeUI(true); // Activa el panel ESC

        yield return null;
    }

    // private System.Collections.IEnumerator ReturnFromZoom()
    // {
    //     StopCoroutine(ConfineCursorToCanvas());
    //     screenCanvases[currentScreenIndex].gameObject.SetActive(false);

    //     // Restablecer cursor INMEDIATAMENTE (sin esperar la animación)
    //     Cursor.lockState = CursorLockMode.Locked;
    //     Cursor.visible = false;

    //     float elapsedTime = 0f;
    //     while (elapsedTime < 1f)
    //     {
    //         playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, preZoomPosition, elapsedTime);
    //         playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, preZoomRotation, elapsedTime);
    //         elapsedTime += Time.deltaTime * zoomSpeed;
    //         yield return null;
    //     }


    //     isZoomed = false;
    //     uiController.SetZoomModeUI(false);
    // }

    private IEnumerator ReturnFromZoom()
    {
        // Evitar errores si no hay índice válido
        if (currentScreenIndex < 0 || currentScreenIndex >= screenCanvases.Length)
        {
            Debug.LogWarning("⚠️ ReturnFromZoom llamado sin pantalla activa válida.");
            isZoomed = false;
            yield break;
        }

        StopCoroutine(ConfineCursorToCanvas());
        screenCanvases[currentScreenIndex].gameObject.SetActive(false);

        // Restablecer cursor INMEDIATAMENTE (sin esperar la animación)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, preZoomPosition, elapsedTime);
            playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, preZoomRotation, elapsedTime);
            elapsedTime += Time.deltaTime * zoomSpeed;
            yield return null;
        }

        isZoomed = false;
        currentScreenIndex = -1;
        uiController.SetZoomModeUI(false);
    }

    private System.Collections.IEnumerator ConfineCursorToCanvas()
    {
        while (isZoomed)
        {
            // Convertir los límites del Canvas a coordenadas de pantalla
            Vector3[] canvasCorners = new Vector3[4];
            canvasRect.GetWorldCorners(canvasCorners);

            Vector2 minScreenPoint = playerCamera.WorldToScreenPoint(canvasCorners[0]);
            Vector2 maxScreenPoint = playerCamera.WorldToScreenPoint(canvasCorners[2]);

            // Restringir el cursor a los límites del Canvas
            Vector2 clampedCursorPos = new Vector2(
                Mathf.Clamp(Input.mousePosition.x, minScreenPoint.x, maxScreenPoint.x),
                Mathf.Clamp(Input.mousePosition.y, minScreenPoint.y, maxScreenPoint.y)
            );

            // Mover el cursor si está fuera del Canvas
            if ((Vector2)Input.mousePosition != clampedCursorPos)
            {
                #if UNITY_EDITOR || UNITY_STANDALONE
                                UnityEngine.InputSystem.Mouse.current.WarpCursorPosition(clampedCursorPos);
                #endif
            }

            yield return null;
        }
    }

    public IEnumerator SalirDelZoomYReiniciar(string escenaNivel)
    {
        if (!isZoomed)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(escenaNivel);
            yield break;
        }

        Debug.Log("🔄 Saliendo del zoom antes de reiniciar nivel...");

        // Salir del zoom primero
        yield return StartCoroutine(ReturnFromZoom());

        // Espera un pequeño momento para seguridad
        yield return new WaitForSeconds(0.5f);

        UnityEngine.SceneManagement.SceneManager.LoadScene(escenaNivel);
    }
    
    public void ForzarReinicioSeguro(string escenaNivel)
    {
        StartCoroutine(ReiniciarTrasZoom(escenaNivel));
    }

    private IEnumerator ReiniciarTrasZoom(string escenaNivel)
    {
        if (isZoomed)
        {
            Debug.Log("🔄 Saliendo del zoom antes de reiniciar...");
            yield return StartCoroutine(ReturnFromZoom());
            yield return new WaitForSeconds(0.4f);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(escenaNivel);
    }
}