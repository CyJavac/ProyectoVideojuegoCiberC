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

    [Header("Drawer Interaction UI")]
    [SerializeField] private Image drawerHoverIcon;
    [SerializeField] private Image drawerActiveIcon;

    private bool isHoveringDrawer = false;
    private bool isGrabbingDrawer = false;

    void Start()
    {
        selectionTooltip?.SetActive(false);
        escPanel?.SetActive(false);
        pausePanel?.SetActive(true);
        drawerHoverIcon?.gameObject.SetActive(false);
        drawerActiveIcon?.gameObject.SetActive(false);
        if (centerDot) centerDot.SetActive(true);
    }

    public void ShowSelectionTooltip(bool show)
    {
        selectionTooltip?.SetActive(show);
    }

    /// <summary>
    /// Muestra u oculta elementos al entrar/salir del modo de zoom.
    /// </summary>
    public void SetZoomModeUI(bool isZoomed)
    {
        escPanel?.SetActive(isZoomed);
        pausePanel?.SetActive(!isZoomed);
        selectionTooltip?.SetActive(false);
        UpdateCenterDotState();
    }

    public void SetPauseMenuUI(bool isPaused)
    {
        // Oculta el tooltip siempre que el juego esté en pausa
        if (selectionTooltip != null)
            selectionTooltip.SetActive(!isPaused);

        // pausePanel (el botón de pausa pequeño) solo se muestra cuando el juego NO está pausado
        if (pausePanel != null)
            pausePanel.SetActive(!isPaused);

        // Actualiza el punto central y demás elementos
        UpdateCenterDotState();

        Debug.Log($"[UIController] Actualizado UI pausa. isPaused = {isPaused}");
    }


    // Mostrar icono cuando apunta a un cajón
    public void ShowDrawerHoverIcon(bool show)
    {
        isHoveringDrawer = show;
        drawerHoverIcon?.gameObject.SetActive(show);
        UpdateCenterDotState();
    }

    //Mostrar icono cuando manipula un cajón
    public void ShowDrawerActiveIcon(bool show)
    {
        isGrabbingDrawer = show;
        drawerActiveIcon?.gameObject.SetActive(show);
        UpdateCenterDotState();
    }

    private void UpdateCenterDotState()
    {
        bool shouldShowCenterDot = true;

        // Si está pausado o en zoom, el centerDot debe desaparecer
        if (PauseMenuManager.IsGamePaused() || escPanel.activeSelf)
            shouldShowCenterDot = false;

        // Si está apuntando o arrastrando un cajón, también desaparece
        if (isHoveringDrawer || isGrabbingDrawer)
            shouldShowCenterDot = false;

        if (centerDot != null)
            centerDot.SetActive(shouldShowCenterDot);
    }
}