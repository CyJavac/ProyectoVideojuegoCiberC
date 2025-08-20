using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject selectionTooltip;
    [SerializeField] private GameObject escPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject centerDot;

    void Start()
    {
        // Inicialmente, solo muestra el menú de pausa
        selectionTooltip.SetActive(false);
        escPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ShowSelectionTooltip(bool show)
    {
        selectionTooltip.SetActive(show);
    }

    public void SetZoomModeUI(bool isZoomed)
    {
        escPanel.SetActive(isZoomed);
        pausePanel.SetActive(!isZoomed);
        selectionTooltip.SetActive(false);
        centerDot.SetActive(!isZoomed);
    }
}