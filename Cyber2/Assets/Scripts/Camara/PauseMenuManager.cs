using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject quitConfirmationPanel;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_InputField sensitivityInput; // Cambiado a TMP_InputField
    [SerializeField] private Toggle invertYToggle;
    [SerializeField] private Button applyButton; // Nuevo botón

    // Textos TMP (si los necesitas directamente)
    [SerializeField] private TextMeshProUGUI sensitivityLabel;
    [SerializeField] private Text invertYLabel; //El texto del checkbox NO es TMP

    [Header("Configuración")]
    //[SerializeField] private float defaultSensitivity = 250f;
    private static bool isPaused = false; // Ahora es estático
    private float pendingSensitivity; // Valor temporal

    // Propiedad estática para acceso externo
    public static bool IsGamePaused() => isPaused;


    public static PauseMenuManager Instance; // Singleton

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Opcional: si el menú persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    void Start()
    {
        
        // Desactiva el menú al inicio
        pauseMenu.SetActive(false);
        optionsPanel.SetActive(false);
        quitConfirmationPanel.SetActive(false);
        
        //Configuración inicial
        sensitivitySlider.minValue = 1;
        sensitivitySlider.maxValue = 500;
        sensitivitySlider.value = 250;
        sensitivityInput.text = "250";
        pendingSensitivity = 250;

        // Configura eventos
        sensitivitySlider.onValueChanged.AddListener(OnSliderChanged);
        sensitivityInput.onEndEdit.AddListener(OnInputChanged);
        applyButton.onClick.AddListener(ApplySettings);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        // Si estamos haciendo zoom, no permitir pausar
        if (ScreenInteraction.IsZoomed()) 
        {
            // Opcional: forzar salida del zoom si es necesario
            return;
        }
        
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }


    public void OnSliderChanged(float value)
    {
        pendingSensitivity = value;
        sensitivityInput.text = Mathf.RoundToInt(value).ToString();
    }

    public void OnInputChanged(string value)
    {
        if (int.TryParse(value, out int num))
        {
            num = Mathf.Clamp(num, 1, 500);
            pendingSensitivity = num;
            sensitivitySlider.SetValueWithoutNotify(num);
            sensitivityInput.text = num.ToString();
        }
        else
        {
            sensitivityInput.text = Mathf.RoundToInt(pendingSensitivity).ToString();
        }
    }

    public void ApplySettings()
    {
        int finalValue = Mathf.RoundToInt(pendingSensitivity);
        CameraRotation.SetSensitivity(finalValue);
        Debug.Log($"Configuración aplicada: {finalValue}");
    }



    // --- Invertir Eje Y ---
    public void OnInvertYToggleChanged()
    {
        if (invertYToggle.isOn)
        {
            CameraRotation.SetInvertYAxis(true);
        } else
        {
            CameraRotation.SetInvertYAxis(false);
        }
        
    }

    // --- Botones ---
    public void OnResumePressed()
    {
        TogglePause();
    }

    public void OnOptionsPressed()
    {
        optionsPanel.SetActive(true);
    }

    public void OnBackPressed()
    {
        optionsPanel.SetActive(false);
    }

    public void OnQuitPressed()
    {
        quitConfirmationPanel.SetActive(true);
    }

    public void OnQuitToMenu()
    {
        Time.timeScale = 1f; // Asegura que el tiempo se reanude
        SceneManager.LoadScene("MainMenu"); // Cambia "MainMenu" por tu escena
    }

    public void OnQuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OnCancelQuit()
    {
        quitConfirmationPanel.SetActive(false);
    }
    

}