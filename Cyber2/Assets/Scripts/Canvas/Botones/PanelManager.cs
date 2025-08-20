using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelManager : MonoBehaviour
{
    [System.Serializable]
    public class PanelButtonPair
    {
        public Button button;
        public GameObject panel;
    }

    [SerializeField] private List<PanelButtonPair> panelButtonPairs = new List<PanelButtonPair>();
    [SerializeField] private Button closeAllButton;

    private GameObject currentOpenPanel = null;

    private void Start()
    {
        foreach (var pair in panelButtonPairs)
        {
            if (pair.button != null && pair.panel != null)
            {
                pair.button.onClick.AddListener(() => OpenPanel(pair.panel));
            }
        }

        if (closeAllButton != null)
        {
            closeAllButton.onClick.AddListener(CloseAllPanels);
        }

        CloseAllPanels();
    }

    public void OpenPanel(GameObject panel)
    {
        // Si el panel ya está abierto, no hacemos nada
        if (currentOpenPanel == panel && panel.activeSelf)
            return;

        // Cerramos el panel actual si hay uno abierto
        if (currentOpenPanel != null)
        {
            currentOpenPanel.SetActive(false);
        }

        // Abrimos el nuevo panel
        panel.SetActive(true);
        currentOpenPanel = panel;
    }

    public void ClosePanel(GameObject panel)
    {
        if (panel == currentOpenPanel)
        {
            currentOpenPanel = null;
        }
        panel.SetActive(false);
    }

    private void CloseAllPanels()
    {
        foreach (var pair in panelButtonPairs)
        {
            if (pair.panel != null)
            {
                pair.panel.SetActive(false);
            }
        }
        currentOpenPanel = null;
    }
}