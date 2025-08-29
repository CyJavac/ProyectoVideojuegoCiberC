using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton para fácil acceso desde otros scripts

    [Header("UI References")]
    public TextMeshProUGUI scoreText;     // Texto principal con el puntaje total
    public TextMeshProUGUI popupText;     // Texto pequeño que muestra el +50 o -30

    private int currentScore = 0;

    private void Awake()
    {
        // Configuración Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateScoreUI();
        popupText.text = ""; // ocultamos el popup al inicio
    }

    // Método para añadir puntos (positivos o negativos)
    public void AddScore(int amount)
    {
        currentScore += amount;
        if (currentScore < 0) currentScore = 0; // nunca baja de 0

        UpdateScoreUI();
        ShowPopup(amount);
    }

    // Actualiza el texto principal con el puntaje total
    private void UpdateScoreUI()
    {
        scoreText.text = "Puntos: " + currentScore.ToString();
    }

    // Muestra el popup de +50 o -30
    private void ShowPopup(int amount)
    {
        if (amount > 0)
            popupText.text = "<color=green>+" + amount.ToString() + "</color>";
        else
            popupText.text = "<color=red>" + amount.ToString() + "</color>";

        // Reiniciamos la animación del popup
        CancelInvoke("HidePopup");
        Invoke("HidePopup", 1.5f); // se oculta en 1.5 segundos
    }

    // Ocultar el popup
    private void HidePopup()
    {
        popupText.text = "";
    }
}
