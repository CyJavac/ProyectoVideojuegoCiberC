using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class NivelPirateria : MonoBehaviour
{
    [Header("UI")]
    public Slider barraSaludPC;                 // Slider (0..1)
    public Image barraFillImage;               // Image del Fill (assign in Inspector)
    public Image barraHandleImage;
    public TextMeshProUGUI tiempoTexto;
    public TextMeshProUGUI feedbackTexto;
    public GameObject popupsParent;            // contenedor para pop-ups
    public GameObject popupPrefab;             // prefab simple para popups

    [Header("Parámetros")]
    [Range(0f, 100f)] public float saludMax = 100f;
    public float tiempoMax = 60f;

    public float saludActual;
    private float tiempoRestante;
    private bool nivelActivo = false;

    [SerializeField] private Renderer monitorRenderer;
    [SerializeField] private Material materialNormal;
    [SerializeField] private Material materialInfectado;

    [SerializeField] private GameObject glitchOverlay;

    [SerializeField] private ChatPopupsManager chatManager;

    [Header("Economía")]
    public int creditos = 100;
    public TextMeshProUGUI creditosTexto;
    public int costoDescargaLegal = 40;

    [Header("Derrota - Teléfono")]
    public GameObject telefonoPanel;
    public GameObject panelResponder;
    public GameObject panelOpcionesDerrota;
    public AudioSource audioRingtone;
    public AudioSource audioVibracion;
    public AudioSource audioLlamada;

    public string escenaMenu = "MenuPrincipal";
    public string escenaNivel = "NivelPirateria";



    void ActualizarCreditos()
    {
        if (creditosTexto != null)
            creditosTexto.text = $"Créditos: {creditos}";
    }

    void CambiarMaterialMonitor()
    {
        if (monitorRenderer != null && materialInfectado != null)
        {
            monitorRenderer.material = materialInfectado;
        }
    }

    private void OnEnable()
    {
        ReiniciarNivel();
        nivelActivo = true;
    }

    private void OnDisable()
    {
        nivelActivo = false;
        // opcional: limpiar popups
        foreach (Transform t in popupsParent.transform) Destroy(t.gameObject);
    }

    private void Update()
    {
        if (!nivelActivo) return;

        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            Derrota("Se acabó el tiempo. No encontraste una descarga segura.");
        }

        if (saludActual < 50f && chatManager != null)
            chatManager.ReducirIntervalo(5f);

        ActualizarUI();
    }

    void ReiniciarNivel()
    {
        saludActual = saludMax;
        tiempoRestante = tiempoMax;
        feedbackTexto.text = "Selecciona un sitio seguro para descargar.";
        AcomodarBarra();
    }


    // public void ElegirSitio(bool esSeguro, int daño, string nombreSitio)
    // {
    //     if (!nivelActivo) return;

    //     if (esSeguro)
    //     {
    //         float saludAntes = saludActual;
    //         saludActual = Mathf.Min(saludActual + 10f, saludMax);
    //         float ganado = saludActual - saludAntes;

    //         feedbackTexto.text = $"✅ Descarga segura (+{ganado:F0} de salud)";
    //         ActualizarUI();
    //         Victoria();
    //         return;
    //     }

    //     // Descarga sospechosa → aplicar daño
    //     float saludAntesInsegura = saludActual;
    //     saludActual -= daño;
    //     if (saludActual < 0) saludActual = 0;

    //     float perdido = saludAntesInsegura - saludActual;
    //     feedbackTexto.text = $"⚠ Riesgo detectado (-{perdido:F0} de salud)";
    //     GenerarPopup($"Advertencia: descarga peligrosa detectada.");

    //     ActualizarUI();

    //     if (saludActual <= 0)
    //     {
    //         Derrota("Tu PC está completamente infectado.");
    //     }
    // }

    public void ElegirSitio(bool esSeguro, int daño, string nombreSitio, bool esLegal = false)
    {
        if (!nivelActivo) return;

        if (esLegal)
        {
            if (creditos >= costoDescargaLegal)
            {
                creditos -= costoDescargaLegal;
                ActualizarCreditos();
                feedbackTexto.text = $"💾 Descargaste legalmente pagando {costoDescargaLegal} créditos.";
                Victoria();
            }
            else
            {
                feedbackTexto.text = "⚠ No tienes suficientes créditos para comprar legalmente.";
            }
            return;
        }

        if (esSeguro)
        {
            feedbackTexto.text = "✅ Descarga segura (pero pirata). Sin daño.";
            // No daña, pero no cuenta como victoria.
            return;
        }

        saludActual -= daño;
        if (saludActual < 0) saludActual = 0;
        feedbackTexto.text = $"⚠ Riesgo detectado (-{daño} salud)";
        GenerarPopup("Descarga peligrosa detectada.");
        ActualizarUI();

        if (saludActual <= 0)
            Derrota("Tu PC está completamente infectado.");
    }


    void AcomodarBarra()
    {
        barraSaludPC.value = saludActual / saludMax;
        // Color: verde (full) -> amarillo (mid) -> rojo (low)
        float t = 1f - (saludActual / saludMax); // 0 => full, 1 => empty
        Color color = Color.Lerp(Color.green, Color.red, t);
        // Puedes suavizar pasando por amarillo si quieres:
        // Color color = Color.Lerp(Color.green, Color.yellow, 1 - t); then Lerp to red...
        barraFillImage.color = color;
        barraHandleImage.color = color;
    }

    void ActualizarUI()
    {
        tiempoTexto.text = $"Tiempo: \n{Mathf.CeilToInt(tiempoRestante)}s";
        // actualizar barra ya hecha por AcomodarBarra()
        AcomodarBarra();
    }

    void GenerarPopup(string texto)
    {
        if (popupPrefab == null || popupsParent == null) return;
        GameObject p = Instantiate(popupPrefab, popupsParent.transform);
        TextMeshProUGUI t = p.GetComponentInChildren<TextMeshProUGUI>();
        if (t) t.text = texto;
        Destroy(p, 4f); // se autodestruye en 4s
    }

    void Victoria()
    {
        nivelActivo = false;
        feedbackTexto.text += " ✅";
        // aquí puedes llamar al ScoreManager o desbloquear siguiente nivel
    }

    // void Derrota(string mensaje)
    // {
    //     nivelActivo = false;
    //     saludActual = 0;
    //     ActualizarUI();
    //     feedbackTexto.text = mensaje + " ❌";

    //     // Activar glitch overlay
    //     if (glitchOverlay != null)
    //     {
    //         glitchOverlay.SetActive(true);
    //     }

    //     CambiarMaterialMonitor(); //Cambio de material
    // }

    void Derrota(string mensaje)
    {
        nivelActivo = false;
        saludActual = 0;
        ActualizarUI();
        feedbackTexto.text = mensaje + " ❌";

        if (glitchOverlay != null)
            glitchOverlay.SetActive(true);

        CambiarMaterialMonitor();

        // Activar escena del teléfono
        if (telefonoPanel != null)
            telefonoPanel.SetActive(true);

        // Iniciar sonidos
        if (audioRingtone != null) audioRingtone.Play();
        if (audioVibracion != null) audioVibracion.Play();

        // Mostrar botón de responder
        if (panelResponder != null)
            panelResponder.SetActive(true);
    }


    public void ResponderLlamada()
    {
        // Detener sonidos
        if (audioRingtone != null) audioRingtone.Stop();
        if (audioVibracion != null) audioVibracion.Stop();

        // Ocultar botón de responder
        if (panelResponder != null) panelResponder.SetActive(false);

        // Reproducir voz
        if (audioLlamada != null) audioLlamada.Play();

        // Mostrar opciones después de 3 segundos
        //StartCoroutine(MostrarOpcionesDerrota());
    }

    // IEnumerator MostrarOpcionesDerrota()
    // {
    //     yield return new WaitForSeconds(3f);
    //     if (panelOpcionesDerrota != null)
    //         panelOpcionesDerrota.SetActive(true);
    // }

    //public void VolverMenu() => UnityEngine.SceneManagement.SceneManager.LoadScene(escenaMenu);
    //public void Reintentar() => UnityEngine.SceneManagement.SceneManager.LoadScene(escenaNivel);

    public void VolverMenu()
    {
        ScreenInteraction screenInteraction = FindObjectOfType<ScreenInteraction>();

        if (screenInteraction != null)
        {
            screenInteraction.ForzarReinicioSeguro(escenaMenu);
        }
        else
        {
            // Fallback si no se encuentra (por seguridad)
            UnityEngine.SceneManagement.SceneManager.LoadScene(escenaMenu);
        }
    }

    public void Reintentar()
    {
        string escenaNivel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        ScreenInteraction screenInteraction = FindObjectOfType<ScreenInteraction>();

        if (screenInteraction != null)
        {
            screenInteraction.ForzarReinicioSeguro(escenaNivel);
        }
        else
        {
            // Fallback si no se encuentra (por seguridad)
            UnityEngine.SceneManagement.SceneManager.LoadScene(escenaNivel);
        }
    }

}
