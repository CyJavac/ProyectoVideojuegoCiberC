using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NivelPirateria : MonoBehaviour
{
    [Header("UI")]
    public Slider barraSaludPC;
    public Image barraFillImage;
    public Image barraHandleImage;
    public TextMeshProUGUI tiempoTexto;
    public TextMeshProUGUI feedbackTexto;
    public GameObject popupsParent;
    public GameObject popupPrefab;

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

    [Header("Sistema de Objetivos")]
    public TextMeshProUGUI objetivosTexto; // ← Arrastra aquí el Text UI
    public int objetivosTotales = 3;

    [System.Serializable]
    public class Objetivo
    {
        public string nombreMostrado;
        public string mediaUrl; // ← Clave para comparar
        public bool completado;
    }

    private List<Objetivo> objetivos = new List<Objetivo>();
    private List<string> todosMediaUrls = new List<string>(); // Pool global
    

    [Header("Derrota - Teléfono")]
    public GameObject telefonoPanel;
    public GameObject panelResponder;
    public GameObject panelOpcionesDerrota;
    public AudioSource audioRingtone;
    public AudioSource audioVibracion;
    public AudioSource audioLlamada;

    public string escenaMenu = "MenuPrincipal";
    public string escenaNivel = "NivelPirateria";

    void Start()
    {
        ActualizarCreditos();
        //RecopilarTodosMediaUrls();
        //GenerarObjetivosAleatorios();
        ReiniciarNivel();
        //MostrarObjetivos();
    }


    void ActualizarCreditos()
    {
        if (creditosTexto != null)
            creditosTexto.text = $"{creditos}";
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


    public void ElegirSitioPirata(int daño, string nombreSitio)
    {
        if (!nivelActivo) return;

        // === MENSAJE INOFENSIVO (daño = 0) ===
        if (daño <= 0)
        {
            feedbackTexto.text = $"Mensaje leído: {nombreSitio}";
            GenerarPopup($"Sin riesgo: {nombreSitio}");
            Debug.Log($"[PIRATA] Mensaje inofensivo: {nombreSitio}");
            return; // ← No hace nada más
        }

        // === DESCARGA PELIGROSA (daño > 0) ===
        saludActual -= daño;
        if (saludActual < 0) saludActual = 0;

        feedbackTexto.text = $"¡Virus detectado! (-{daño} salud)";
        GenerarPopup($"¡Infectado desde {nombreSitio}!");
        //CambiarMaterialMonitor();
        //if (glitchOverlay != null) glitchOverlay.SetActive(true);

        ActualizarUI();

        if (saludActual <= 0)
            Derrota("Tu PC está completamente infectado.");
    }



    public void ElegirSitioLegal(int costo, string nombreSitio)
    {
        if (!nivelActivo) return;

        if (creditos >= costo)
        {
            creditos -= costo;
            ActualizarCreditos();
            feedbackTexto.text = $"Descarga legal completada (-{costo} créditos)";
            GenerarPopup($"Comprado: {nombreSitio}");

            Debug.Log($"[LEGAL] Compra exitosa: {nombreSitio}");
        }
        else
        {
            feedbackTexto.text = $"Faltan {costo - creditos} créditos";
            GenerarPopup("Créditos insuficientes");
        }
    }



    void AcomodarBarra()
    {
        barraSaludPC.value = saludActual / saludMax;
        float t = 1f - (saludActual / saludMax);
        Color color = Color.Lerp(Color.green, Color.red, t);
        barraFillImage.color = color;
        barraHandleImage.color = color;
    }

    void ActualizarUI()
    {
        tiempoTexto.text = $"Tiempo: \n{Mathf.CeilToInt(tiempoRestante)}s";
        AcomodarBarra();
    }

    void GenerarPopup(string texto)
    {
        if (popupPrefab == null || popupsParent == null) return;
        GameObject p = Instantiate(popupPrefab, popupsParent.transform);
        TextMeshProUGUI t = p.GetComponentInChildren<TextMeshProUGUI>();
        if (t) t.text = texto;
        Destroy(p, 4f);
    }

    void Victoria()
    {
        nivelActivo = false;
        feedbackTexto.text += " ✅";
        // aquí puedes llamar al ScoreManager o desbloquear siguiente nivel
    }

    void Derrota(string mensaje)
    {
        nivelActivo = false;
        saludActual = 0;
        ActualizarUI();
        feedbackTexto.text = mensaje + " ❌";

        if (glitchOverlay != null)
            glitchOverlay.SetActive(true);

        CambiarMaterialMonitor();

        if (telefonoPanel != null)
            telefonoPanel.SetActive(true);

        if (audioRingtone != null) audioRingtone.Play();
        if (audioVibracion != null) audioVibracion.Play();

        if (panelResponder != null)
            panelResponder.SetActive(true);
    }

    public void ResponderLlamada()
    {
        if (audioRingtone != null) audioRingtone.Stop(); 
        if (audioVibracion != null) audioVibracion.Stop();

        if (panelResponder != null) panelResponder.SetActive(false);

        if (audioLlamada != null) audioLlamada.Play();
    }

    public void VolverMenu()
    {
        ScreenInteraction screenInteraction = FindObjectOfType<ScreenInteraction>();
        if (screenInteraction != null)
        {
            screenInteraction.ForzarReinicioSeguro(escenaMenu);
        }
        else
        {
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
            UnityEngine.SceneManagement.SceneManager.LoadScene(escenaNivel);
        }
    }
}