using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Nombres de escenas")]
    [Tooltip("Escribe los nombres exactamente como aparecen en Build Settings.")]
    public string escenaNivelPhishing = "Nivel_Phishing";
    public string escenaNivelPirateria = "Nivel_Pirateria";
    public string escenaNivelContrasenas = "Nivel_Contrasenas";

    [Header("Opciones generales")]
    public bool mostrarCursor = true; // por si se quiere ocultar en otros menús

    void Start()
    {
        // Asegurar que el cursor esté visible y sin bloqueo
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = mostrarCursor;
    }

    // 🔹 Métodos públicos para usar desde los botones del menú

    public void IrANivelPhishing()
    {
        CargarEscena(escenaNivelPhishing);
    }

    public void IrANivelPirateria()
    {
        CargarEscena(escenaNivelPirateria);
    }

    public void IrANivelContrasenas()
    {
        CargarEscena(escenaNivelContrasenas);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

        // Si estás en el editor, detener modo Play
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }

    // 🔹 Método genérico para evitar repetición
    private void CargarEscena(string nombreEscena)
    {
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogWarning("⚠ No se ha asignado el nombre de la escena.");
            return;
        }

        SceneManager.LoadScene(nombreEscena);
    }

}
