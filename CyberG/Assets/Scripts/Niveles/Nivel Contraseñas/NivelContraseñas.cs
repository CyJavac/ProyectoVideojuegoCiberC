using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NivelContraseñas : MonoBehaviour
{
    [Header("Objetivo del Nivel")]
    public int idArchivoObjetivo = 5; // ID del archivo a enviar (ej: Entrega_Proyecto_Final.pptx)
    public string nombreArchivoObjetivo = "Entrega_Proyecto_Final.pptx";

    [Header("Panel de Victoria")]
    public GameObject panelVictoria;
    public Text txtEstadisticas;
    public Button btnReiniciar;
    public Button btnSiguiente;
    public Button btnMenu;

    [Header("Estadísticas")]
    private int archivosAbiertos = 0;
    private int intentosFallidos = 0;

    [Header("Escenas")]
    public string escenaSiguiente = "MenuPrincipal";
    public string escenaMenu = "MenuPrincipal";

    void Start()
    {
        if (panelVictoria != null) panelVictoria.SetActive(false);
    }

    public void AbrirArchivo(FileEntry archivo)
    {
        archivosAbiertos++;
        Debug.Log($"[Nivel] Archivo abierto: {archivo.name} (ID: {archivo.id})");
    }

    public void IntentarContraseña(bool correcta)
    {
        if (!correcta) intentosFallidos++;
    }

    public void EnviarArchivo(FileEntry archivo)
    {
        if (archivo.id == idArchivoObjetivo)
        {
            Victoria();
        }
        else
        {
            Debug.Log("[Nivel] Archivo incorrecto enviado.");
            // Opcional: feedback "Este no es el archivo solicitado"
        }
    }

    private void Victoria()
    {
        Time.timeScale = 1f;
        panelVictoria.SetActive(true);

        string stats = $"<b>ESTADÍSTICAS</b>\n\n" +
                       $"Archivo encontrado: <color=green>{nombreArchivoObjetivo}</color>\n" +
                       $"Archivos abiertos: {archivosAbiertos}\n" +
                       $"Intentos fallidos de contraseña: {intentosFallidos}\n" +
                       $"Tiempo: {Mathf.RoundToInt(Time.timeSinceLevelLoad)} segundos";

        if (txtEstadisticas != null)
            txtEstadisticas.text = stats;

        // Botones
        btnReiniciar.onClick.RemoveAllListeners();
        btnSiguiente.onClick.RemoveAllListeners();
        btnMenu.onClick.RemoveAllListeners();

        btnReiniciar.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        btnSiguiente.onClick.AddListener(() => SceneManager.LoadScene(escenaSiguiente));
        btnMenu.onClick.AddListener(() => SceneManager.LoadScene(escenaMenu));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}