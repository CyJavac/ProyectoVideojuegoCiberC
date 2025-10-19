using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class FileExplorerPanel : MonoBehaviour
{
    [Header("Configuración")]
    public string folderName; // Ej: "Descargas", "Trabajo", etc.
    public Transform contentParent; // Donde instanciar los botones
    public GameObject fileButtonPrefab; // Prefab base con Textos e Imagen
    public Sprite iconCarpeta;
    public Sprite iconArchivoTxt;
    public Sprite iconArchivoZip;
    public Sprite iconArchivoExe;
    public Sprite iconArchivoPdf;
    public Sprite iconArchivoImage;
    public Sprite iconArchivoDocx;
    public Sprite iconArchivoXlsx;
    public Sprite iconArchivoPptx;
    public Sprite iconArchivoDesconocido;

    [Header("Referencias")]
    public FileDatabaseImporter database; // Arrástralo en el Inspector
    public Toggle mostrarOcultosToggle;

    private List<GameObject> instanciados = new List<GameObject>();

    void OnEnable()
    {
        // 🔹 Al activarse el panel, generar el contenido
        GenerarContenido();
    }

    public void GenerarContenido()
    {
        // 🔹 Limpia los botones anteriores
        foreach (var go in instanciados)
            Destroy(go);
        instanciados.Clear();

        if (database == null) return;

        bool mostrarOcultos = mostrarOcultosToggle == null || mostrarOcultosToggle.isOn;

        // 🔹 Obtener los archivos/carpeta del CSV
        List<FileEntry> archivos = database.GetFilesInPath(folderName);

        foreach (var archivo in archivos)
        {
            if (archivo.hidden && !mostrarOcultos) continue;

            GameObject boton = Instantiate(fileButtonPrefab, contentParent);
            instanciados.Add(boton);

            // 🔹 Configurar textos
            // boton.transform.Find("TextoNombre").GetComponent<TextMeshProUGUI>().text = archivo.name;
            // boton.transform.Find("TextoTipoArchivo").GetComponent<TextMeshProUGUI>().text = archivo.type.ToUpper();
            boton.transform.Find("TextoNombre").GetComponent<Text>().text = archivo.name;
            boton.transform.Find("TextoTipoArchivo").GetComponent<Text>().text = archivo.type.ToUpper();


            // 🔹 Configurar ícono
            Image icon = boton.transform.Find("Imagen").GetComponent<Image>();
            icon.sprite = ObtenerIcono(archivo);

            // 🔹 Configurar color si está bloqueado
            if (archivo.locked)
                icon.color = Color.red;

            // 🔹 Configurar acción al hacer clic
            Button btn = boton.GetComponent<Button>();
            btn.onClick.AddListener(() => OnArchivoClic(archivo));
        }
    }

    private Sprite ObtenerIcono(FileEntry archivo)
    {
        if (archivo.type == "folder") return iconCarpeta;
        switch (archivo.type)
        {
            case "text": return iconArchivoTxt;
            case "archive": return iconArchivoZip;
            case "exe": return iconArchivoExe;
            case "pdf": return iconArchivoPdf;
            case "image": return iconArchivoImage;
            case "document": return iconArchivoDocx;
            case "spreadsheet": return iconArchivoXlsx;
            case "presentation": return iconArchivoPptx;
            default: return iconArchivoDesconocido;
        }
    }

    private void OnArchivoClic(FileEntry archivo)
    {
        if (archivo.type == "folder")
        {
            // 🔹 Abrir el panel correspondiente a esa carpeta
            Debug.Log($"Abrir carpeta: {archivo.name}");
            // Aquí puedes activar el panel correcto:
            // ExploradorUIManager.Instance.AbrirPanel(archivo.name);
        }
        else
        {
            // 🔹 Abrir archivo (con contraseña si aplica)
            if (archivo.locked)
            {
                Debug.Log($"Archivo bloqueado: {archivo.name}. Requiere contraseña: {archivo.password}");
                // Mostrar UI para ingresar contraseña
            }
            else
            {
                Debug.Log($"Abrir archivo {archivo.name}: {archivo.shortContent}");
                // Mostrar contenido del archivo (panel nuevo)
            }
        }
    }
}
