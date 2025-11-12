using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChatUIGenerator : MonoBehaviour
{
    [Header("Contenedor global de paneles")]
    public Transform panelEspacioChats; // arrastra aquí el Panel_EspacioChats desde el editor

    [Header("Data")]
    [Tooltip("Referencia al ChatDataManager que carga los CSV")]
    public ChatDataManager dataManager;

    [Header("Iconos por categoría")]
    public Sprite iconoPeliculas;
    public Sprite iconoVideojuegos;

    [Header("UI containers / prefabs")]
    public Transform groupButtonsContainer;
    public GameObject groupButtonPrefab;
    public GameObject groupPanelPrefab;
    public Transform groupPanelsParent;
    public GameObject messagePrefab;

    [Header("Settings")]
    public int groupsPerRun = 7;
    public int messagesPerGroup = 10;

    [Header("Referencias")]
    public NivelPirateria nivelPirateria; // referencia al script principal

    // runtime
    private List<ChatGroup> selectedGroups = new List<ChatGroup>();
    private List<GameObject> spawnedPanels = new List<GameObject>();

    private IEnumerator Start()
    {
        if (dataManager == null)
            dataManager = FindObjectOfType<ChatDataManager>();

        if (dataManager == null)
        {
            Debug.LogError("ChatDataManager no encontrado. Asignalo en el inspector.");
            yield break;
        }

        // Esperar a que ChatDataManager cargue
        int waitFrames = 0;
        while ((dataManager.grupos == null || dataManager.grupos.Count == 0) && waitFrames < 10)
        {
            waitFrames++;
            yield return null;
        }

        if (dataManager.grupos == null || dataManager.grupos.Count == 0)
        {
            Debug.LogError("❌ ChatDataManager aún no tiene grupos después de esperar.");
            yield break;
        }

        Debug.Log($"✅ [ChatUIGenerator] Detectados {dataManager.grupos.Count} grupos listos.");
        GenerateGroupButtons();
    }

    public void GenerateGroupButtons()
    {
        foreach (Transform t in groupButtonsContainer) Destroy(t.gameObject);
        foreach (var p in spawnedPanels) Destroy(p);
        spawnedPanels.Clear();
        selectedGroups.Clear();

        var allGroups = dataManager.grupos;
        if (allGroups == null || allGroups.Count == 0)
        {
            Debug.LogWarning("No hay grupos en dataManager.");
            return;
        }

        // Selecciona grupos aleatorios
        var shuffled = allGroups.OrderBy(x => Random.value).ToList();
        int take = Mathf.Min(groupsPerRun, shuffled.Count);
        selectedGroups = shuffled.Take(take).ToList();

        foreach (var grp in selectedGroups)
        {
            GameObject btnGO = Instantiate(groupButtonPrefab, groupButtonsContainer);
            Text txtName = btnGO.transform.Find("Txt_NombreGrupo").GetComponent<Text>();
            txtName.text = grp.groupName;

            Transform tDesc = btnGO.transform.Find("Txt_Description");
            if (tDesc != null) tDesc.GetComponent<Text>().text = grp.description;

            GameObject panelGO = Instantiate(groupPanelPrefab, groupPanelsParent);
            panelGO.name = $"Panel_{grp.groupName}";
            panelGO.SetActive(false);
            spawnedPanels.Add(panelGO);

            Transform descPanel = panelGO.transform.Find("Panel_DescGrupo");
            if (descPanel != null)
            {
                Transform txtNameObj = descPanel.Find("Txt_NombreGrupo");
                if (txtNameObj != null)
                    txtNameObj.GetComponent<Text>().text = grp.groupName;

                Transform imgTransform = descPanel.Find("FotoPerfil/Imagen");
                if (imgTransform != null)
                {
                    Image img = imgTransform.GetComponent<Image>();
                    if (img != null)
                    {
                        if (grp.category.ToLower().Contains("pelic") || grp.category.ToLower().Contains("movie"))
                            img.sprite = iconoPeliculas;
                        else
                            img.sprite = iconoVideojuegos;
                    }
                }
            }

            Button b = btnGO.GetComponent<Button>();
            ChatGroup captured = grp;
            GameObject capturedPanel = panelGO;
            b.onClick.AddListener(() => OpenGroupPanel(captured, capturedPanel));
        }

        Debug.Log($"[ChatUIGenerator] Generados {selectedGroups.Count} botones de grupo.");
    }

    void OpenGroupPanel(ChatGroup group, GameObject panelGO)
    {
        foreach (var p in spawnedPanels) p.SetActive(false);
        Transform content = panelGO.transform.Find("ScrollViewMensajes/Viewport/Content");
        if (!content) return;
        foreach (Transform c in content) Destroy(c.gameObject);

        var messages = dataManager.GetMessagesForGroup(group.groupId, group.category);
        if (messages.Count == 0)
        {
            messages = (group.category.Contains("pelic") ? dataManager.mensajesPeliculas : dataManager.mensajesVideojuegos)
                       .OrderBy(x => Random.value).Take(5).ToList();
        }

        var shuffled = messages.OrderBy(x => Random.value).Take(10).ToList();

        foreach (var msg in shuffled)
        {
            GameObject mGO = Instantiate(messagePrefab, content);
            mGO.transform.Find("Txt_NombreMiembro").GetComponent<Text>().text = msg.senderName;
            mGO.transform.Find("Txt_Mensaje").GetComponent<Text>().text = msg.messageText;

            Button btn = mGO.transform.Find("Btn_Link").GetComponent<Button>();
            btn.onClick.RemoveAllListeners();

            // Capturar valores correctos
            int value = msg.value;
            string siteName = msg.messageText;
            bool isLegal = dataManager.isLegalSite;

            // btn.onClick.AddListener(() =>
            // {
            //     if (isLegal)
            //         nivelPirateria.ElegirSitioLegal(value, siteName);
            //     else
            //         nivelPirateria.ElegirSitioPirata(value, siteName);
            // });
            
            btn.onClick.AddListener(() =>
            {
                int costo = msg.value;
                string nombre = msg.messageText;
                bool esLegal = dataManager.isLegalSite;

                if (esLegal)
                    nivelPirateria.ElegirSitioLegal(costo, nombre); // ← SIN Victoria()
                else
                    nivelPirateria.ElegirSitioPirata(costo, nombre);
            });

        }

        panelGO.SetActive(true);
    }





}
