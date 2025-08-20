using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CursorManager
{
    public static void SetCursorState(bool locked, bool hidden)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !hidden;
    }

    public static void Initialize()
    {
        SetCursorState(true, true); // Bloquea y oculta el cursor al inicio
    }

    //Bloquear cursor al area del canvas
    // public static void LockCursorToGameView(bool confine)
    // {
    //     Cursor.lockState = confine ? CursorLockMode.Confined : CursorLockMode.None;
    //     Cursor.visible = !confine;
    // }

    public static void LockCursorToGameView(bool confine)
    {
        Cursor.lockState = confine ? CursorLockMode.Confined : CursorLockMode.None;
        Cursor.visible = true; // Mostrar cursor al confinarlo
    }

    // public static void CenterCursor()
    // {
    //     Cursor.position = new Vector2(Screen.width / 2, Screen.height / 2);
    // }

    // public static void CenterCursor()
    // {
    //     // Alternativa 1: Usar Input.mousePosition (solo para debug)
    //     //Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    //     //Input.mousePosition = screenCenter; // No funciona en builds finales

    //     // Alternativa 2: Forzar el cursor al centro (requiere Unity 2021+)
    //     #if UNITY_EDITOR || UNITY_STANDALONE
    //     UnityEngine.InputSystem.Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2, Screen.height / 2));
    //     #endif
    // }

}