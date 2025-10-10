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

    public void ElegirSitio(bool esSeguro, int daño, string nombreSitio)
    {
        if (!nivelActivo) return;

        if (esSeguro)
        {
            feedbackTexto.text = $"Descarga segura desde: {nombreSitio}. ¡Buen trabajo!";
            saludActual = Mathf.Min(saludActual + 10f, saludMax);
            ActualizarUI(); // <- refresca barra
            Victoria();
            return;
        }

        // Descarga sospechoso -> aplicamos daño
        saludActual -= daño;
        if (saludActual < 0) saludActual = 0; //Forzar que nunca quede negativo

        feedbackTexto.text = $"⚠ Has descargado desde: {nombreSitio}. Riesgo detectado.";
        GenerarPopup($"Advertencia: {nombreSitio} tiene anuncios y archivos sospechosos.");

        ActualizarUI(); //Refresca barra ANTES de checkear derrota

        if (saludActual <= 0)
        {
            Derrota("Tu PC está completamente infectado.");
        }
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

    void Derrota(string mensaje)
    {
        nivelActivo = false;
        saludActual = 0;
        ActualizarUI();
        feedbackTexto.text = mensaje + " ❌";

        // Activar glitch overlay
        if (glitchOverlay != null)
        {
            glitchOverlay.SetActive(true);
        }

        CambiarMaterialMonitor(); //Cambio de material
    }
    
    

}
