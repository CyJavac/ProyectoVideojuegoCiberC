using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[Serializable]
public class FileEntry
{
    public int id;
    public string path;
    public string name;
    public bool locked;
    public string password;
    public bool hidden;
    public string type;
    public string shortContent;
    public string consequence;
    public int points;
}

public class FileDatabaseImporter : MonoBehaviour
{
    [Header("CSV")]
    public string csvRelativePath = "CSV_Niveles/Contraseñas/Archivos_Contraseñas.csv";
    public bool useStreamingAssets = true;
    
    [Tooltip("Loaded file entries")]
    public List<FileEntry> entries = new List<FileEntry>();
    
    void Start()
    {
        LoadCSV();
        Debug.Log($"Loaded {entries.Count} file entries.");
        
        // Debug: Imprimir los primeros 3 IDs para verificar
        for (int i = 0; i < Mathf.Min(3, entries.Count); i++)
        {
            Debug.Log($"Entry {i}: ID={entries[i].id}, Name={entries[i].name}");
        }
    }
    
    public void LoadCSV()
    {
        entries.Clear();
        string fullPath;
        
        if (useStreamingAssets)
            fullPath = Path.Combine(Application.streamingAssetsPath, csvRelativePath);
        else
            fullPath = Path.Combine(Application.dataPath, csvRelativePath);
        
        Debug.Log($"Intentando cargar CSV desde: {fullPath}");
        
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"CSV not found at {fullPath}");
            return;
        }
        
        string[] lines = File.ReadAllLines(fullPath);
        Debug.Log($"Total líneas leídas: {lines.Length}");
        
        bool first = true;
        int lineNumber = 0;
        
        foreach (string raw in lines)
        {
            lineNumber++;
            
            if (first) 
            { 
                Debug.Log($"Header saltado: {raw}");
                first = false; 
                continue; 
            }
            
            if (string.IsNullOrWhiteSpace(raw))
            {
                Debug.Log($"Línea {lineNumber} vacía, saltando...");
                continue;
            }
            
            Debug.Log($"Procesando línea {lineNumber}: {raw.Substring(0, Mathf.Min(100, raw.Length))}...");
            
            // Parsear CSV respetando comillas
            string[] cols = ParseCSVLine(raw);
            
            Debug.Log($"Línea {lineNumber} parseada en {cols.Length} columnas");
            
            if (cols.Length < 10)
            {
                Debug.LogWarning($"Línea {lineNumber} con menos de 10 columnas (tiene {cols.Length}): {raw}");
                for (int i = 0; i < cols.Length; i++)
                {
                    Debug.Log($"  Col[{i}]: '{cols[i]}'");
                }
                continue;
            }
            
            FileEntry e = new FileEntry();
            
            // Limpiar y parsear el ID
            string idStr = cols[0].Trim();
            Debug.Log($"Línea {lineNumber} - Intentando parsear ID: '{idStr}'");
            
            if (!int.TryParse(idStr, out e.id))
            {
                Debug.LogError($"ERROR en línea {lineNumber}: No se pudo parsear ID: '{idStr}'");
                Debug.LogError($"Todas las columnas de esta línea:");
                for (int i = 0; i < cols.Length; i++)
                {
                    Debug.LogError($"  Col[{i}]: '{cols[i]}'");
                }
                continue;
            }
            
            Debug.Log($"Línea {lineNumber} - ID parseado correctamente: {e.id}");
            
            e.path = cols[1].Trim();
            e.name = cols[2].Trim();
            e.locked = cols[3].Trim().ToLower() == "true";
            e.password = cols[4].Trim();
            e.hidden = cols[5].Trim().ToLower() == "true";
            e.type = cols[6].Trim();
            
            // Limpiar shortContent (remover comillas si existen)
            e.shortContent = cols[7].Trim().Trim('"');
            // Reemplazar \n literales por saltos de línea reales
            e.shortContent = e.shortContent.Replace("\\n", "\n");
            
            e.consequence = cols[8].Trim();
            
            // Parsear points
            string pointsStr = cols[9].Trim();
            if (!int.TryParse(pointsStr, out e.points))
            {
                Debug.LogWarning($"No se pudo parsear points: '{pointsStr}' para ID {e.id}");
                e.points = 0;
            }
            
            entries.Add(e);
        }
        
        Debug.Log($"CSV cargado exitosamente: {entries.Count} entradas");
    }
    
    /// <summary>
    /// Parsea una línea CSV respetando campos entre comillas y comillas escapadas ("")
    /// </summary>
    private string[] ParseCSVLine(string line)
    {
        // Si la línea completa está entre comillas, quitarlas
        line = line.Trim();
        if (line.StartsWith("\"") && line.EndsWith("\""))
        {
            line = line.Substring(1, line.Length - 2);
        }
        
        List<string> fields = new List<string>();
        StringBuilder currentField = new StringBuilder();
        bool inQuotes = false;
        
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            
            if (c == '"')
            {
                // Verificar si es una comilla escapada ("")
                if (i + 1 < line.Length && line[i + 1] == '"')
                {
                    // Es una comilla escapada, agregar una sola comilla
                    currentField.Append('"');
                    i++; // Saltar la segunda comilla
                }
                else
                {
                    // Toggle estado de comillas
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                // Fin del campo
                fields.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }
        
        // Agregar el último campo
        fields.Add(currentField.ToString());
        
        return fields.ToArray();
    }
    
    // Helper: get files in a path
    public List<FileEntry> GetFilesInPath(string folderPath)
    {
        return entries.FindAll(x => x.path.Equals(folderPath, StringComparison.OrdinalIgnoreCase));
    }
    
    // Helper: get nested files (in subfolders)
    public List<FileEntry> GetFilesRecursive(string folderPath)
    {
        return entries.FindAll(x => x.path.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase));
    }
    
    // Lookup by id
    public FileEntry GetById(int id) => entries.Find(e => e.id == id);
}