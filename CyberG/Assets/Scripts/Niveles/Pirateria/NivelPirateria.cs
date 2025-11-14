using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class NivelPirateria : MonoBehaviour
{
    private int contarDescargasPiratas = 0;
    private int contarComprasLegales = 0;

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
    
    [Header("Panel de Victoria")]
    public GameObject panelVictoria;
    public Text txtTituloVictoria; 
    public Text txtEstadisticas;
    public Button btnReiniciar;
    public Button btnSiguiente;
    public Button btnMenu;


    [Header("Derrota - Teléfono")]
    public GameObject telefonoPanel;
    public GameObject panelResponder;
    public GameObject panelOpcionesDerrota;
    public AudioSource audioRingtone;
    public AudioSource audioVibracion;
    public AudioSource audioLlamada;
    public GameObject imagenDerrota;
    public GameObject objetoConCollider;
    private Coroutine parpadeoCoroutine;

    [Header("Escenas")]
    public string escenaSiguiente = "Contraseñas";
    public string escenaMenu = "MenuPrincipal";
    public string escenaNivel = "Pirateria";

    void Start()
    {
        ActualizarCreditos();
        // RecopilarTodosMediaUrls();
        // GenerarObjetivosAleatorios();
        // ReiniciarNivel();
        // MostrarObjetivos();

        StartCoroutine(InicializarObjetivosConEspera());
    }

    private IEnumerator InicializarObjetivosConEspera()
    {
        // Esperar 1 frame para que todos los ChatUIGenerator terminen Start()
        yield return null;

        // Asegurarse de que todos los generators hayan generado sus URLs
        ChatUIGenerator[] generators = FindObjectsOfType<ChatUIGenerator>();
        foreach (var gen in generators)
        {
            // Forzar generación si no se hizo (por si Start() falló)
            if (gen.selectedMediaUrls.Count == 0)
                gen.GenerateGroupButtons();
        }

        // Esperar otro frame por seguridad
        yield return null;

        RecopilarUrlsSeleccionadas(); // ← NUEVA FUNCIÓN
        GenerarObjetivosAleatorios();
        MostrarObjetivos();
        ReiniciarNivel();
    }


    
    // void RecopilarTodosMediaUrls()
    // {
    //     todosMediaUrls.Clear();

    //     // Buscar TODOS los ChatDataManager en la escena
    //     ChatDataManager[] todosManagers = FindObjectsOfType<ChatDataManager>();
    //     foreach (var manager in todosManagers)
    //     {
    //         todosMediaUrls.AddRange(manager.mensajesPeliculas.Select(m => m.mediaUrl));
    //         todosMediaUrls.AddRange(manager.mensajesVideojuegos.Select(m => m.mediaUrl));
    //     }

    //     Debug.Log($"[OBJETIVOS] Recopilados {todosMediaUrls.Count} URLs únicas");
    //     todosMediaUrls = todosMediaUrls.Distinct().ToList();
    // }
    void RecopilarUrlsSeleccionadas()
    {
        todosMediaUrls.Clear();

        // NUEVO: Usar selectedMediaUrls de TODOS los ChatUIGenerator
        ChatUIGenerator[] generators = FindObjectsOfType<ChatUIGenerator>();
        foreach (var gen in generators)
        {
            todosMediaUrls.AddRange(gen.selectedMediaUrls);
        }

        // Eliminar duplicados
        todosMediaUrls = todosMediaUrls.Distinct().ToList();

        Debug.Log($"[OBJETIVOS] URLs seleccionadas visibles: {todosMediaUrls.Count}");
    }


    void GenerarObjetivosAleatorios()
    {
        objetivos.Clear();
        var shuffled = todosMediaUrls.OrderBy(x => Random.value).Take(objetivosTotales * 2).ToList(); // Extra para variedad

        foreach (string url in shuffled.Take(objetivosTotales))
        {
            string nombreMostrado = ExtraerNombreObjetivo(url); // ← Detecta si es legal/pirata
            objetivos.Add(new Objetivo { mediaUrl = url, nombreMostrado = nombreMostrado, completado = false });
        }

        Debug.Log($"[OBJETIVOS] Generados: {string.Join(", ", objetivos.Select(o => o.nombreMostrado))}");
    }

    string ExtraerNombreObjetivo(string mediaUrl)
    {
        // Buscar en TODOS los managers qué sitio corresponde a esta URL
        ChatDataManager[] managers = FindObjectsOfType<ChatDataManager>();

        foreach (var manager in managers)
        {
            // Revisar películas
            var msgPelic = manager.mensajesPeliculas.FirstOrDefault(m => m.mediaUrl == mediaUrl);
            if (msgPelic != null)
            {
                if (manager.isLegalSite)
                    return msgPelic.senderName; // ← LEGAL: sender_name
                else
                    return ExtraerTextoComillas(msgPelic.messageText); // ← PIRATA: entre "
            }

            // Revisar videojuegos
            var msgJuego = manager.mensajesVideojuegos.FirstOrDefault(m => m.mediaUrl == mediaUrl);
            if (msgJuego != null)
            {
                if (manager.isLegalSite)
                    return msgJuego.senderName; // ← LEGAL: sender_name
                else
                    return ExtraerTextoComillas(msgJuego.messageText); // ← PIRATA: entre "
            }
        }

        return mediaUrl; // Fallback
    }

    string ExtraerTextoComillas(string textoConComillas)
    {
        // Extrae texto entre las primeras comillas dobles
        int inicio = textoConComillas.IndexOf('"');
        if (inicio == -1) return textoConComillas;

        int fin = textoConComillas.IndexOf('"', inicio + 1);
        if (fin == -1) return textoConComillas.Substring(inicio + 1);

        return textoConComillas.Substring(inicio + 1, fin - inicio - 1);
    }

    void MostrarObjetivos()
    {
        if (objetivosTexto == null) return;

        string texto = "OBJETIVOS:\n";
        foreach (var obj in objetivos)
        {
            string check = obj.completado ? "[O]" : "[X]";
            texto += $"{check} {obj.nombreMostrado}\n";
        }
        texto += $"\n({objetivos.Count(o => o.completado)}/{objetivosTotales})";

        objetivosTexto.text = texto;
    }
    
    bool CompletarObjetivo(string mediaUrl)
    {
        var objetivo = objetivos.FirstOrDefault(o => o.mediaUrl == mediaUrl);
        if (objetivo != null && !objetivo.completado)
        {
            objetivo.completado = true;
            MostrarObjetivos();
            Debug.Log($"[OBJETIVO] ¡Completado! {objetivo.nombreMostrado}");
            
            int completados = objetivos.Count(o => o.completado);
            if (completados >= objetivosTotales)
            {
                Victoria();
                return true;
            }
            
            feedbackTexto.text += " ¡Objetivo completado!";
            return true;
        }
        return false;
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

        // === RESETEAR EFECTOS DE DERROTA ===
        if (imagenDerrota != null)
            imagenDerrota.SetActive(false);

        if (objetoConCollider != null)
        {
            var collider = objetoConCollider.GetComponent<BoxCollider>();
            if (collider != null)
                collider.enabled = false;
        }

        if (parpadeoCoroutine != null)
        {
            StopCoroutine(parpadeoCoroutine);
            parpadeoCoroutine = null;
        }

        if (glitchOverlay != null)
            glitchOverlay.SetActive(false);
    }


    public void ElegirSitioLegal(int costo, string nombreSitio, string mediaUrl = "") // ← AÑADIR mediaUrl
    {
        if (!nivelActivo) return;

        if (creditos >= costo)
        {
            creditos -= costo;
            contarComprasLegales++;
            ActualizarCreditos();
            feedbackTexto.text = $"Descarga legal (-{costo} créditos)";

            // ← VERIFICAR OBJETIVO
            if (!string.IsNullOrEmpty(mediaUrl) && CompletarObjetivo(mediaUrl))
            {
                GenerarPopup("Objetivo completado");
            }
            else
            {
                GenerarPopup($"Comprado: {nombreSitio}");
            }
        }
        else
        {
            feedbackTexto.text = $"Faltan {costo - creditos} créditos";
        }
    }

    public void ElegirSitioPirata(int daño, string nombreSitio, string mediaUrl = "") // ← AÑADIR mediaUrl
    {
        if (!nivelActivo) return;

        if (daño > 0)
            contarDescargasPiratas++;

        if (daño <= 0)
        {
            feedbackTexto.text = $"Mensaje leído: {nombreSitio}";
            // ← AÚN VERIFICAR OBJETIVO (puede ser info útil)
            if (!string.IsNullOrEmpty(mediaUrl)) CompletarObjetivo(mediaUrl);
            return;
        }

        saludActual -= daño;
        if (saludActual < 0) saludActual = 0;

        feedbackTexto.text = $"¡Virus! (-{daño} salud)";

        // ← VERIFICAR OBJETIVO ANTES DEL DAÑO (¡puedes conseguirlo aunque sea riesgoso!)
        bool objetivoCompletado = !string.IsNullOrEmpty(mediaUrl) && CompletarObjetivo(mediaUrl);

        if (objetivoCompletado)
            GenerarPopup("¡Objetivo completado... pero con virus!");
        else
            GenerarPopup($"Infectado: {nombreSitio}");

        //CambiarMaterialMonitor();
        //if (glitchOverlay) glitchOverlay.SetActive(true);

        ActualizarUI();
        if (saludActual <= 0) Derrota("PC destruido.");
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

    // void Victoria()
    // {
    //     nivelActivo = false;
    //     feedbackTexto.text += " ✅";
    //     // aquí puedes llamar al ScoreManager o desbloquear siguiente nivel
    // }

    void Victoria()
    {
        nivelActivo = false;
        MostrarPanelVictoria();
        DesbloquearCursor();
    }

    void MostrarPanelVictoria()
    {
        if (panelVictoria != null)
            panelVictoria.SetActive(true);

        // === ESTADÍSTICAS ===
        string stats = $"<b>ESTADÍSTICAS</b>\n\n" +
                       $"Tiempo restante: <color=green>{Mathf.CeilToInt(tiempoRestante)}s</color>\n" +
                       $"Salud final: <color=#00FF00>{Mathf.RoundToInt(saludActual)}/{saludMax}</color>\n" +
                       $"Créditos gastados: <color=yellow>{100 - creditos}</color>\n" +
                       $"Descargas piratas: <color=red>{contarDescargasPiratas}</color>\n" +
                       $"Compras legales: <color=cyan>{contarComprasLegales}</color>";

        if (txtEstadisticas != null)
            txtEstadisticas.text = stats;

        // === BOTONES ===
        if (btnReiniciar != null)
            btnReiniciar.onClick.RemoveAllListeners();

        if (btnSiguiente != null)
            btnSiguiente.onClick.RemoveAllListeners();

        if (btnMenu != null)
            btnMenu.onClick.RemoveAllListeners();

        // Asignar acciones
        if (btnReiniciar != null)
            btnReiniciar.onClick.AddListener(Reintentar);

        if (btnSiguiente != null)
            btnSiguiente.onClick.AddListener(IrSiguienteNivel);

        if (btnMenu != null)
            btnMenu.onClick.AddListener(VolverMenu);
    }

    void DesbloquearCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ==========  Acciones botones victoria  =============
    // public void Reintentar()
    // {
    //     ReiniciarNivelCompleto();
    // }

    public void IrSiguienteNivel()
    {
        SceneManager.LoadScene(escenaSiguiente);
    }

    // public void VolverMenu()
    // {
    //     SceneManager.LoadScene(escenaMenu);
    // }

    // void ReiniciarNivelCompleto()
    // {
    //     // Reiniciar contadores
    //     contarDescargasPiratas = 0;
    //     contarComprasLegales = 0;

    //     // Reiniciar objetivos
    //     RecopilarTodosMediaUrls();
    //     GenerarObjetivosAleatorios();
    //     MostrarObjetivos();

    //     // Reiniciar juego
    //     ReiniciarNivel();
    //     if (panelVictoria != null) panelVictoria.SetActive(false);

    //     // Bloquear cursor de nuevo
    //     Cursor.lockState = CursorLockMode.Locked;
    //     Cursor.visible = false;
    // }


    void Derrota(string mensaje)
    {
        nivelActivo = false;
        saludActual = 0;
        ActualizarUI();
        feedbackTexto.text = mensaje + " [X]";

        if (glitchOverlay != null)
            glitchOverlay.SetActive(true);

        CambiarMaterialMonitor();

        if (telefonoPanel != null)
            telefonoPanel.SetActive(true);

        if (audioRingtone != null) audioRingtone.Play();
        if (audioVibracion != null) audioVibracion.Play();

        if (panelResponder != null)
            panelResponder.SetActive(true);

        // === NUEVO: ACTIVAR IMAGEN Y PARPADEO ===
        if (imagenDerrota != null)
        {
            imagenDerrota.SetActive(true);

            // Forzar que se vea al inicio
            Image img = imagenDerrota.GetComponentInChildren<Image>();
            if (img != null)
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);

            // Reiniciar corutina
            if (parpadeoCoroutine != null)
                StopCoroutine(parpadeoCoroutine);
            parpadeoCoroutine = StartCoroutine(ParpadearImagen(imagenDerrota));
        }


        // === : ACTIVAR BOXCOLLIDER ===
        if (objetoConCollider != null)
        {
            var collider = objetoConCollider.GetComponent<BoxCollider>();
            if (collider != null)
                collider.enabled = true;
        }

    }



    IEnumerator ParpadearImagen(GameObject obj)
    {
        // Asegurarse de que tenga Image
        Image imagen = obj.GetComponentInChildren<Image>();
        if (imagen == null)
        {
            Debug.LogError("[Derrota] No se encontró componente Image en el objeto de derrota.");
            yield break;
        }

        // Guardar color original
        Color colorOriginal = imagen.color;

        while (true)
        {
            // MOSTRAR (1 segundo)
            imagen.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 1f);
            yield return new WaitForSeconds(1f);

            // OCULTAR (1 segundo)
            imagen.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0f);
            yield return new WaitForSeconds(1f);
        }
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