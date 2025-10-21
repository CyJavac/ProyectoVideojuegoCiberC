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
    [Tooltip("Reference to ChatDataManager that loads CSVs")]
    public ChatDataManager dataManager;

    [Header("Icons por categoría")]
    public Sprite iconoPeliculas;
    public Sprite iconoVideojuegos;

    [Header("UI containers / prefabs")]
    public Transform groupButtonsContainer;     // fixed scrollview content for group buttons
    public GameObject groupButtonPrefab;        // prefab for a group button (has Txt_NombreGrupo Text)
    public GameObject groupPanelPrefab;         // prefab for the group's panel (header + ScrollView content)
    public Transform groupPanelsParent;         // where to instantiate group panels (inactive by default)
    public GameObject messagePrefab;            // single prefab: Txt_NombreMiembro (Text), Txt_Mensaje (Text), Btn_Link (Button)

    [Header("Settings")]
    public int groupsPerRun = 7;   // number of groups to show each run
    public int messagesPerGroup = 10; // messages to pick per group (can be less if not enough)

    [Header("References")]
    public NivelPirateria nivelPirateria; // to call ElegirSitio(damage,...)

    // runtime
    private List<ChatGroup> selectedGroups = new List<ChatGroup>();
    private List<GameObject> spawnedPanels = new List<GameObject>();

    // void Start()
    // {
    //     if (dataManager == null) dataManager = FindObjectOfType<ChatDataManager>();
    //     if (dataManager == null)
    //     {
    //         Debug.LogError("ChatDataManager not found. Assign it in inspector.");
    //         return;
    //     }

    //     GenerateGroupButtons();
    // }

    private IEnumerator Start()
    {
        if (dataManager == null)
            dataManager = FindObjectOfType<ChatDataManager>();

        if (dataManager == null)
        {
            Debug.LogError("ChatDataManager not found. Assign it in inspector.");
            yield break;
        }

        // Espera hasta que ChatDataManager haya cargado los grupos
        int waitFrames = 0;
        while ((dataManager.grupos == null || dataManager.grupos.Count == 0) && waitFrames < 10)
        {
            waitFrames++;
            yield return null; // espera un frame
        }

        if (dataManager.grupos == null || dataManager.grupos.Count == 0)
        {
            Debug.LogError("❌ ChatDataManager aún no tiene grupos después de esperar.");
            yield break;
        }

        Debug.Log($"✅ [ChatUIGenerator] Detected {dataManager.grupos.Count} grupos listos.");
        GenerateGroupButtons();
    }

    public void GenerateGroupButtons()
    {
        // cleanup previous UI
        foreach (Transform t in groupButtonsContainer) Destroy(t.gameObject);
        foreach (var p in spawnedPanels) Destroy(p);
        spawnedPanels.Clear();
        selectedGroups.Clear();

        // pick groups randomly but unique
        var allGroups = dataManager.grupos;
        if (allGroups == null || allGroups.Count == 0)
        {
            Debug.LogWarning("No groups found in dataManager.");
            return;
        }

        // Shuffle and take distinct
        var shuffled = allGroups.OrderBy(x => Random.value).ToList();
        int take = Mathf.Min(groupsPerRun, shuffled.Count);
        selectedGroups = shuffled.Take(take).ToList();

        // create a button for each selected group
        foreach (var grp in selectedGroups)
        {
            GameObject btnGO = Instantiate(groupButtonPrefab, groupButtonsContainer);
            Text txtName = btnGO.transform.Find("Txt_NombreGrupo").GetComponent<Text>();
            txtName.text = grp.groupName;

            // (optional) set small description field if prefab has one
            Transform tDesc = btnGO.transform.Find("Txt_Description");
            if (tDesc != null) tDesc.GetComponent<Text>().text = grp.description;

            // instantiate the group's panel prefab (inactive), store it
            //GameObject panelGO = Instantiate(groupPanelPrefab, groupPanelsParent);
            // GameObject panelGO = Instantiate(groupPanelPrefab, panelEspacioChats);

            // panelGO.name = $"Panel_{grp.groupName}";
            // panelGO.SetActive(false);

            // // --- Asignar nombre e imagen en el Panel_DescGrupo ---
            // Transform descPanel = panelGO.transform.Find("Panel_VistaChats/Panel_DescGrupo");
            // if (descPanel != null)
            // {
            //     // Cambiar nombre del grupo
            //     Transform txt = descPanel.Find("Txt_NombreGrupo");
            //     if (txt != null)
            //         txt.GetComponent<Text>().text = grp.groupName;

            //     // Cambiar imagen del grupo (según categoría)
            //     Transform imgParent = descPanel.Find("FotoPerfil/Imagen");
            //     if (imgParent != null)
            //     {
            //         Image img = imgParent.GetComponent<Image>();
            //         if (img != null)
            //         {
            //             // 🔹 Si tienes sprites distintos para películas y videojuegos:
            //             if (grp.category.ToLower().Contains("pel"))
            //                 img.sprite = iconoPeliculas; // Asigna desde el inspector
            //             else
            //                 img.sprite = iconoVideojuegos; // Asigna desde el inspector
            //         }
            //     }
            // }

            // instantiate the group's panel prefab (inactive), store it
            GameObject panelGO = Instantiate(groupPanelPrefab, groupPanelsParent);
            panelGO.name = $"Panel_{grp.groupName}";
            panelGO.SetActive(false);
            spawnedPanels.Add(panelGO);

            // Buscar el panel de descripción
            Transform descPanel = panelGO.transform.Find("Panel_DescGrupo");
            if (descPanel != null)
            {
                // Actualizar nombre del grupo
                Transform txtNameObj = descPanel.Find("Txt_NombreGrupo");
                if (txtNameObj != null)
                    txtNameObj.GetComponent<Text>().text = grp.groupName;

                // Asignar imagen según tipo (películas o videojuegos)
                Transform imgTransform = descPanel.Find("FotoPerfil/Imagen");
                if (imgTransform != null)
                {
                    Image img = imgTransform.GetComponent<Image>();
                    if (img != null)
                    {
                        if (grp.category.ToLower().Contains("pelic") || grp.category.ToLower().Contains("movie"))
                            img.sprite = iconoPeliculas; // sprite asignado por Inspector
                        else
                            img.sprite = iconoVideojuegos;  // sprite asignado por Inspector
                    }
                }
            }
            else
            {
                Debug.LogWarning($"⚠ No se encontró Panel_DescGrupo dentro de {panelGO.name}");
            }




            // set header name & icon if present
            Transform headerName = panelGO.transform.Find("Header/Txt_NombreGrupo");
            if (headerName != null) headerName.GetComponent<Text>().text = grp.groupName;
            Transform icon = panelGO.transform.Find("Header/Img_TipoGrupo");
            if (icon != null)
            {
                // set icon based on grp.category if you want (assign sprites in inspector instead)
            }

            // add button listener to open panel and populate messages
            Button b = btnGO.GetComponent<Button>();
            ChatGroup captured = grp;
            GameObject capturedPanel = panelGO;
            b.onClick.AddListener(() => OpenGroupPanel(captured, capturedPanel));
        }

        Debug.Log($"[ChatUIGenerator] Generated {selectedGroups.Count} group buttons.");
    }

    void OpenGroupPanel(ChatGroup group, GameObject panelGO)
    {
        // hide all other panels
        foreach (var p in spawnedPanels) p.SetActive(false);

        Transform scroll = panelGO.transform.Find("ScrollViewMensajes");
        if (scroll == null)
        {
            Debug.LogError($"❌ No se encontró 'ScrollViewMensajes' dentro de {panelGO.name}");
            return;
        }



        Transform content = scroll.Find("Viewport/Content");
        if (content == null)
        {
            Debug.LogError($"❌ No se encontró 'Viewport/Content' dentro de {panelGO.name}");
            return;
        }




        if (content == null)
        {
            Debug.LogError("❌ No se encontró 'Panel_VistaChats/ScrollViewMensajes/Viewport/Content' en " + panelGO.name);
            return;
        }
        foreach (Transform c in content) Destroy(c.gameObject);

        // choose message source based on group.category
        //bool isMovie = group.category.ToLower().Contains("pelic") || group.category.ToLower().Contains("movie") || group.category.ToLower().Contains("peliculas");
        //bool isMovie = group.category.Trim().ToLower() == "peliculas";
        //List<ChatMessage> pool = isMovie ? dataManager.mensajesPeliculas : dataManager.mensajesVideojuegos;

        // get all messages for this group id (pool may be large)
        // var messagesForGroup = pool.Where(m => m.groupId == group.groupId).ToList();

        // if (messagesForGroup.Count == 0)
        // {
        //     // if none found, consider pulling random messages from pool (optional)
        //     Debug.LogWarning($"No messages found for group {group.groupName} (id {group.groupId}).");
        // }
        // Asociar fuente según tipo de grupo
        bool isMovie = group.category.Trim().ToLower() == "peliculas";
        List<ChatMessage> pool = isMovie ? dataManager.mensajesPeliculas : dataManager.mensajesVideojuegos;

        // Buscar mensajes asignados a este grupo
        var messagesForGroup = pool.Where(m => m.groupId == group.groupId).ToList();

        // Si el grupo no tiene mensajes, usar aleatorios del mismo tipo
        if (messagesForGroup.Count == 0)
        {
            Debug.LogWarning($"⚠ No messages found for group {group.groupName} (id {group.groupId}). Using random messages instead.");
            messagesForGroup = pool.OrderBy(x => Random.value).Take(messagesPerGroup).ToList();
        }

        // pick up to messagesPerGroup randomly (messages may repeat across different groups, but here we avoid duplicates within this group)
        var shuffled = messagesForGroup.OrderBy(x => Random.value).ToList();
        int take = Mathf.Min(messagesPerGroup, shuffled.Count);
        var chosen = shuffled.Take(take).ToList();

        // instantiate message prefabs
        foreach (var msg in chosen)
        {
            GameObject mGO = Instantiate(messagePrefab, content);
            Text tName = mGO.transform.Find("Txt_NombreMiembro").GetComponent<Text>();
            Text tMsg  = mGO.transform.Find("Txt_Mensaje").GetComponent<Text>();
            Button btn  = mGO.transform.Find("Btn_Link").GetComponent<Button>();

            tName.text = msg.senderName;
            tMsg.text  = msg.messageText;

            // remove previous listeners to avoid duplication
            btn.onClick.RemoveAllListeners();

            // link action: call NivelPirateria.ElegirSitio
            int damage = msg.damage;
            bool isSafe = damage == 0;
            string siteName = msg.messageText; // or msg.mediaUrl

            btn.onClick.AddListener(() =>
            {
                if (nivelPirateria != null)
                    nivelPirateria.ElegirSitio(isSafe, damage, siteName);
                else
                    Debug.Log($"Download clicked: {siteName} damage {damage}");
            });
        }

        // show panel
        panelGO.SetActive(true);
    }
}
