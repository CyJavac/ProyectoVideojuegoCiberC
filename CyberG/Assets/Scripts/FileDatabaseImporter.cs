using System;
using System.Collections.Generic;
using System.IO;
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
    public string csvRelativePath = "file_database.csv"; // inside StreamingAssets or Resources
    public bool useStreamingAssets = true;
    [Tooltip("Loaded file entries")]
    public List<FileEntry> entries = new List<FileEntry>();

    void Start()
    {
        LoadCSV();
        // Example: debug print
        Debug.Log($"Loaded {entries.Count} file entries.");
    }

    public void LoadCSV()
    {
        entries.Clear();
        string fullPath;

        if (useStreamingAssets)
            fullPath = Path.Combine(Application.streamingAssetsPath, csvRelativePath);
        else
            fullPath = Path.Combine(Application.dataPath, csvRelativePath);

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"CSV not found at {fullPath}");
            return;
        }

        string[] lines = File.ReadAllLines(fullPath);
        bool first = true;
        foreach (string raw in lines)
        {
            if (first) { first = false; continue; } // skip header
            if (string.IsNullOrWhiteSpace(raw)) continue;
            // naive CSV split (works if no commas inside fields)
            string[] cols = raw.Split(new char[] {','}, 10);
            if (cols.Length < 10) continue;

            FileEntry e = new FileEntry();
            int.TryParse(cols[0], out e.id);
            e.path = cols[1].Trim();
            e.name = cols[2].Trim();
            e.locked = cols[3].Trim().ToLower() == "true";
            e.password = cols[4].Trim();
            e.hidden = cols[5].Trim().ToLower() == "true";
            e.type = cols[6].Trim();
            e.shortContent = cols[7].Trim().Trim('"');
            e.consequence = cols[8].Trim();
            int.TryParse(cols[9], out e.points);

            entries.Add(e);
        }
    }

    // Helper: get files in a path
    public List<FileEntry> GetFilesInPath(string folderPath)
    {
        List<FileEntry> list = entries.FindAll(x => x.path.Equals(folderPath, StringComparison.OrdinalIgnoreCase));
        return list;
    }

    // Helper: get nested files (in subfolders)
    public List<FileEntry> GetFilesRecursive(string folderPath)
    {
        List<FileEntry> list = entries.FindAll(x => x.path.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase));
        return list;
    }

    // Lookup by id
    public FileEntry GetById(int id) => entries.Find(e => e.id == id);
}
