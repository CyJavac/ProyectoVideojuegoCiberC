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

    public void SetZoomModeUI(bool isZoomed)
    {
        escPanel?.SetActive(isZoomed);
        pausePanel?.SetActive(!isZoomed);
        selectionTooltip?.SetActive(false);
        centerDot?.SetActive(!isZoomed);
    }

    // 🔹 Mostrar icono cuando apunta a un cajón
    public void ShowDrawerHoverIcon(bool show)
    {
        isHoveringDrawer = show;
        drawerHoverIcon?.gameObject.SetActive(show);
        UpdateCenterDotState();
    }

    // 🔹 Mostrar icono cuando manipula un cajón
    public void ShowDrawerActiveIcon(bool show)
    {
        isGrabbingDrawer = show;
        drawerActiveIcon?.gameObject.SetActive(show);
        UpdateCenterDotState();
    }

    private void UpdateCenterDotState()
    {
        if (centerDot != null)
            centerDot.SetActive(!isHoveringDrawer && !isGrabbingDrawer);
    }
}