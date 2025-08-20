using UnityEngine;
using UnityEngine.UI;

public class ClosePanelButton : MonoBehaviour
{
    [SerializeField] private GameObject panelToClose;
    private PanelManager panelManager;

    private void Start()
    {
        if (panelToClose == null)
        {
            panelToClose = transform.parent.gameObject;
        }

        // Buscamos el PanelManager en la escena
        panelManager = FindObjectOfType<PanelManager>();
        
        GetComponent<Button>().onClick.AddListener(ClosePanel);
    }

    private void ClosePanel()
    {
        if (panelManager != null)
        {
            panelManager.ClosePanel(panelToClose);
        }
        else
        {
            // Fallback por si no encontramos el PanelManager
            panelToClose.SetActive(false);
        }
    }
}