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
    [SerializeField] private TMP_InputField sensitivityInput;
    [SerializeField] private Toggle invertYToggle;
    [SerializeField] private Button applyButton;

    [SerializeField] private TextMeshProUGUI sensitivityLabel;
    [SerializeField] private Text invertYLabel;

    private static bool isPaused = false;
    private float pendingSensitivity;
    public static bool InputBloqueadoTemporalmente = false; //NUEVO

    public static bool IsGamePaused() => isPaused;
    public static PauseMenuManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        pauseMenu.SetActive(false);
        optionsPanel.SetActive(false);
        quitConfirmationPanel.SetActive(false);

        sensitivitySlider.minValue = 1;
        sensitivitySlider.maxValue = 500;
        sensitivitySlider.value = 250;
        sensitivityInput.text = "250";
        pendingSensitivity = 250;

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
            return;

        isPaused = !isPaused;

        //Asegura que la UI se actualice ANTES de pausar el tiempo
        var ui = FindObjectOfType<UIController>();
        if (ui != null)
            ui.SetPauseMenuUI(isPaused);

        //Activa o desactiva el menú principal
        pauseMenu.SetActive(isPaused);

        //Cambia el estado del cursor
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        //Pausa o reanuda el tiempo (al final)
        Time.timeScale = isPaused ? 0f : 1f;

        Debug.Log($"[PauseMenuManager] Estado de pausa: {isPaused}");
    }

    //NUEVO MÉTODO
    private IEnumerator BloquearInputTemporal()
    {
        InputBloqueadoTemporalmente = true;
        yield return new WaitForSecondsRealtime(0.25f);
        InputBloqueadoTemporalmente = false;
    }

    //ACTUALIZADO
    public void OnResumePressed()
    {
        TogglePause();
        StartCoroutine(BloquearInputTemporal()); // ← Evita clics fantasmas tras cerrar el menú
    }

    public void OnOptionsPressed() => optionsPanel.SetActive(true);
    public void OnBackPressed() => optionsPanel.SetActive(false);
    public void OnQuitPressed() => quitConfirmationPanel.SetActive(true);

    public void OnQuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnCancelQuit() => quitConfirmationPanel.SetActive(false);

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

    public void OnInvertYToggleChanged()
    {
        CameraRotation.SetInvertYAxis(invertYToggle.isOn);
    }
}