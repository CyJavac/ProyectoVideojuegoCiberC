using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRotation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _mouseSensitivity = 250f; // Variable serializable
    [SerializeField] private bool _invertYAxis = false;      // Variable serializable

    private static float mouseSensitivity; // Variable estática real
    private static bool invertYAxis;
    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        mouseSensitivity = _mouseSensitivity; // Sincroniza valores iniciales
        invertYAxis = _invertYAxis;
        CursorManager.Initialize();
    }

    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked && !PauseMenuManager.IsGamePaused())
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime * (invertYAxis ? -1 : 1);

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
        //_mouseSensitivity = mouseSensitivity; // Sincroniza valores iniciales
        //_invertYAxis = invertYAxis;
    }

    public static void SetSensitivity(float sensitivity)
    {
        mouseSensitivity = Mathf.Clamp(sensitivity, 1f, 500f);
        Debug.Log($"Sensibilidad actualizada: {mouseSensitivity}");
    }

    public static void SetInvertYAxis(bool invertY)
    {
        invertYAxis = invertY;
    }
}