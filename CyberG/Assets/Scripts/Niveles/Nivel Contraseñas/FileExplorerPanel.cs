using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FileExplorerPanel : MonoBehaviour
{
    [Header("Configuración")]
    public string folderName;
    public Transform contentParent;
    public GameObject fileButtonPrefab;
    public Sprite iconCarpeta, iconArchivoTxt, iconArchivoZip, iconArchivoExe, iconArchivoPdf,
                  iconArchivoImage, iconArchivoDocx, iconArchivoXlsx, iconArchivoPptx, iconArchivoDesconocido;

    [Header("Referencias")]
    public FileDatabaseImporter database;
    public Toggle mostrarOcultosToggle;
    public GameObject passwordPanelPrefab;
    public GameObject fileContentPanelPrefab;

    // NUEVO: Paneles preconfigurados para carpetas (Mi PC, Descargas, etc.)
    [Header("Paneles de Carpetas (Preconfigurados)")]
    public List<FileExplorerPanel> panelesCarpetas = new List<FileExplorerPanel>();

    private List<GameObject> instanciados = new List<GameObject>();
    private FileEntry archivoPendiente;

    void OnEnable()
    {
        GenerarContenido();
    }

    public void GenerarContenido()
    {
        foreach (var go in instanciados) Destroy(go);
        instanciados.Clear();

        if (database == null) return;

        bool mostrarOcultos = mostrarOcultosToggle == null || mostrarOcultosToggle.isOn;
        List<FileEntry> archivos = database.GetFilesInPath(folderName);

        foreach (var archivo in archivos)
        {
            if (archivo.hidden && !mostrarOcultos) continue;

            GameObject boton = Instantiate(fileButtonPrefab, contentParent);
            instanciados.Add(boton);

            // Textos
            boton.transform.Find("TextoNombre").GetComponent<Text>().text = archivo.name;
            boton.transform.Find("TextoTipoArchivo").GetComponent<Text>().text = archivo.type.ToUpper();

            // Ícono
            Image icon = boton.transform.Find("Imagen").GetComponent<Image>();
            icon.sprite = ObtenerIcono(archivo);
            if (archivo.locked) icon.color = Color.red;

            // Acción
            Button btn = boton.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnArchivoClic(archivo));
        }
    }

    private Sprite ObtenerIcono(FileEntry archivo)
    {
        if (archivo.type == "folder") return iconCarpeta;
        return archivo.type switch
        {
            "text" => iconArchivoTxt,
            "archive" => iconArchivoZip,
            "exe" => iconArchivoExe,
            "pdf" => iconArchivoPdf,
            "image" => iconArchivoImage,
            "document" => iconArchivoDocx,
            "spreadsheet" => iconArchivoXlsx,
            "presentation" => iconArchivoPptx,
            _ => iconArchivoDesconocido
        };
    }

    private void OnArchivoClic(FileEntry archivo)
    {
        if (archivo.type == "folder")
        {
            // BUSCAR PANEL PRECONFIGURADO POR NOMBRE
            string nombreCarpeta = archivo.name;
            FileExplorerPanel panelDestino = panelesCarpetas.Find(p => 
                p.gameObject.name.Contains(nombreCarpeta) || p.folderName.EndsWith("/" + nombreCarpeta));

            if (panelDestino != null)
            {
                panelDestino.folderName = archivo.path + "/" + archivo.name;
                panelDestino.gameObject.SetActive(true);
                panelDestino.GenerarContenido(); // Refrescar
            }
            else
            {
                Debug.LogWarning($"No se encontró panel para carpeta: {nombreCarpeta}");
            }
        }
        else
        {
            if (archivo.locked)
            {
                MostrarPanelContraseña(archivo);
            }
            else
            {
                MostrarContenidoArchivo(archivo);
            }
        }
    }

    // ================== CONTRASEÑA ==================
    private void MostrarPanelContraseña(FileEntry archivo)
    {
        archivoPendiente = archivo;
        GameObject panel = Instantiate(passwordPanelPrefab, transform.parent);
        panel.name = "PasswordPanel_" + archivo.name;

        // Configurar título
        panel.transform.Find("TextoNombre").GetComponent<Text>().text = $"Contraseña para: {archivo.name}";

        // InputField (Legacy)
        InputField input = panel.transform.Find("InputContraseña").GetComponent<InputField>();
        input.text = "";
        input.textComponent.color = Color.white; // Color normal

        // Botones
        Button btnAceptar = panel.transform.Find("BtnAceptar").GetComponent<Button>();
        Button btnCancelar = panel.transform.Find("BtnCancelar").GetComponent<Button>();

        // CORUTINA PARA BORRAR MENSAJE DE ERROR
        Coroutine errorCoroutine = null;
        void MostrarError()
        {
            if (errorCoroutine != null) StopCoroutine(errorCoroutine);
            input.text = "Contraseña incorrecta";
            input.textComponent.color = Color.red;
            //input.Deactivate(); // Quitar foco
            errorCoroutine = StartCoroutine(BorrarErrorDespuesDe(2f));
        }

        IEnumerator BorrarErrorDespuesDe(float segundos)
        {
            yield return new WaitForSeconds(segundos);
            input.text = "";
            input.textComponent.color = Color.white;
            //input.ActivateInputField(); // Volver a enfocar
            errorCoroutine = null;
        }

        // ACEPTAR
        btnAceptar.onClick.RemoveAllListeners();
        btnAceptar.onClick.AddListener(() =>
        {
            if (input.text == archivo.password)
            {
                if (errorCoroutine != null) StopCoroutine(errorCoroutine);
                Destroy(panel);
                MostrarContenidoArchivo(archivo);
            }
            else
            {
                MostrarError();
            }
        });

        btnAceptar.onClick.AddListener(() =>
        {
            NivelContraseñas nivel = FindObjectOfType<NivelContraseñas>();

            if (input.text == archivo.password)
            {
                if (errorCoroutine != null) StopCoroutine(errorCoroutine);
                Destroy(panel);
                MostrarContenidoArchivo(archivo);
            }
            else
            {
                MostrarError();
                if (nivel != null) nivel.IntentarContraseña(false);
            }
        });

        // CANCELAR
        btnCancelar.onClick.RemoveAllListeners();
        btnCancelar.onClick.AddListener(() =>
        {
            if (errorCoroutine != null) StopCoroutine(errorCoroutine);
            Destroy(panel);
        });

        // ABRIR + FOCO
        panel.SetActive(true);
        input.ActivateInputField();
    }

    // ================== CONTENIDO ==================
    // private void MostrarContenidoArchivo(FileEntry archivo)
    // {
    //     GameObject panel = Instantiate(fileContentPanelPrefab, transform.parent);
    //     panel.name = "ContentPanel_" + archivo.name;

    //     panel.transform.Find("Text_Nombre").GetComponent<Text>().text = archivo.name;
    //     panel.transform.Find("Text_Contenido").GetComponent<Text>().text = archivo.shortContent;

    //     panel.SetActive(true);

    //     AplicarConsecuencia(archivo.consequence, archivo.points);
    // }

    private void MostrarContenidoArchivo(FileEntry archivo)
    {
        GameObject panel = Instantiate(fileContentPanelPrefab, transform.parent);
        panel.name = "ContentPanel_" + archivo.name;

        panel.transform.Find("Text_Nombre").GetComponent<Text>().text = archivo.name;
        panel.transform.Find("Text_Contenido").GetComponent<Text>().text = archivo.shortContent;

        // === NUEVO: BOTÓN ENVIAR ===
        Button btnEnviar = panel.transform.Find("Btn_Enviar").GetComponent<Button>();
        btnEnviar.gameObject.SetActive(false); // Oculto por defecto

        // Mostrar solo si es el archivo objetivo
        NivelContraseñas nivel = FindObjectOfType<NivelContraseñas>();
        if (nivel != null && archivo.id == nivel.idArchivoObjetivo)
        {
            btnEnviar.gameObject.SetActive(true);
            btnEnviar.onClick.RemoveAllListeners();
            btnEnviar.onClick.AddListener(() =>
            {
                nivel.EnviarArchivo(archivo);
                Destroy(panel);
            });
        }

        // Registrar que se abrió
        if (nivel != null)
            nivel.AbrirArchivo(archivo);

        panel.SetActive(true);
    }

    // private void AplicarConsecuencia(string tipo, int valor)
    // {
    //     Debug.Log($"[CONSECUENCIA] {tipo}: {valor}");
    //     // Aquí conectar con tu sistema de puntos
    // }
}