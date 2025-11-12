// Assets/Scripts/Niveles/Pirateria/CsvLoader.cs
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// public static class CsvLoader
// {
//     // Lee un archivo CSV y devuelve las líneas (ignora cabecera y vacías)
//     private static string[] ReadLinesFromStreamingAssets(string relativePath)
//     {
//         string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
//         if (!File.Exists(fullPath))
//         {
//             Debug.LogError($"CSV no encontrado en: {fullPath}");
//             return new string[0];
//         }

//         string raw = File.ReadAllText(fullPath);
//         // Separar por CRLF o LF robustamente y eliminar líneas vacías
//         string[] lines = raw.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
//         return lines;
//     }

//     // Cargar grupos: espera cabecera: group_id,group_name,category,description
//     public static List<ChatGroup> LoadGroups(string relativePath)
//     {
//         var result = new List<ChatGroup>();
//         string[] lines = ReadLinesFromStreamingAssets(relativePath);
//         if (lines.Length <= 1) return result; // sin datos

//         for (int i = 1; i < lines.Length; i++)
//         {
//             string line = lines[i].Trim();
//             if (string.IsNullOrEmpty(line)) continue;

//             // Split simple (asume que no hay comas dentro de campos). Si las hay, ver nota abajo.
//             string[] cols = line.Split(',');
//             if (cols.Length < 4) continue;

//             ChatGroup g = new ChatGroup();
//             int.TryParse(cols[0].Trim(), out g.groupId);
//             g.groupName = cols[1].Trim().Trim('"');
//             g.category = cols[2].Trim().Trim('"').ToLower();
//             g.description = cols[3].Trim().Trim('"');
//             result.Add(g);
//         }

//         return result;
//     }

//     // Cargar mensajes: espera cabecera: group_id,sender_name,message_type,message_text,media_url,damage
//     public static List<ChatMessage> LoadMessages(string relativePath)
//     {
//         var result = new List<ChatMessage>();
//         string[] lines = ReadLinesFromStreamingAssets(relativePath);
//         if (lines.Length <= 1) return result;

//         for (int i = 1; i < lines.Length; i++)
//         {
//             string line = lines[i].Trim();
//             if (string.IsNullOrEmpty(line)) continue;

//             // Split robusto: limitamos a 6 columnas como máximo (evitar partir el texto con comas internas)
//             string[] cols = line.Split(new char[] { ',' }, 6);
//             if (cols.Length < 6) continue;

//             ChatMessage m = new ChatMessage();
//             int.TryParse(cols[0].Trim(), out m.groupId);
//             m.senderName = cols[1].Trim().Trim('"');
//             m.messageType = cols[2].Trim().Trim('"').ToLower();
//             m.messageText = cols[3].Trim().Trim('"');
//             m.mediaUrl = cols[4].Trim().Trim('"');
//             // int.TryParse(cols[5].Trim(), out m.damage);
//             string dmgStr = cols[5].Trim().Replace("\"", "").Replace(" ", "");
//             if (!int.TryParse(dmgStr, out m.damage))
//                 m.damage = 0;


//             result.Add(m);
//         }

//         return result;
//     }



//     // Cargar mensajes: espera cabecera: group_id,sender_name,message_type,message_text,media_url,damage,is_legal
//     // public static List<ChatMessage> LoadMessages(string relativePath)
//     // {
//     //     var result = new List<ChatMessage>();
//     //     string[] lines = ReadLinesFromStreamingAssets(relativePath);
//     //     if (lines.Length <= 1) return result;

//     //     for (int i = 1; i < lines.Length; i++)
//     //     {
//     //         string line = lines[i].Trim();
//     //         if (string.IsNullOrEmpty(line)) continue;

//     //         // Ahora esperamos 7 columnas como máximo
//     //         string[] cols = line.Split(new char[] { ',' }, 7);
//     //         if (cols.Length < 6) continue;

//     //         ChatMessage m = new ChatMessage();
//     //         int.TryParse(cols[0].Trim(), out m.groupId);
//     //         m.senderName = cols[1].Trim().Trim('"');
//     //         m.messageType = cols[2].Trim().Trim('"').ToLower();
//     //         m.messageText = cols[3].Trim().Trim('"');
//     //         m.mediaUrl = cols[4].Trim().Trim('"');

//     //         string dmgStr = cols[5].Trim().Replace("\"", "").Replace(" ", "");
//     //         if (!int.TryParse(dmgStr, out m.damage))
//     //             m.damage = 0;

//     //         // Nueva columna is_legal
//     //         if (cols.Length >= 7)
//     //         {
//     //             string legalStr = cols[6].Trim().ToLower();
//     //             m.isLegal = legalStr == "true" || legalStr == "1" || legalStr == "yes";
//     //         }
//     //         else
//     //         {
//     //             m.isLegal = false; // por defecto, si no está la columna
//     //         }

//     //         result.Add(m);
//     //     }

//     //     return result;
//     // }

// }

public static class CsvLoader
{
    private static string[] ReadLines(string relativePath)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
        if (!File.Exists(fullPath)) return new string[0];
        string raw = File.ReadAllText(fullPath);
        return raw.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
    }

    public static List<ChatGroup> LoadGroups(string path)
    {
        var result = new List<ChatGroup>();
        string[] lines = ReadLines(path);
        if (lines.Length <= 1) return result;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(new[] { ',' }, 5);
            if (cols.Length < 4) continue;

            var g = new ChatGroup();
            int.TryParse(cols[0].Trim(), out g.groupId);
            g.groupName = cols[1].Trim().Trim('"');
            g.category = cols[2].Trim().Trim('"').ToLower();
            g.description = cols[3].Trim().Trim('"');
            result.Add(g);
        }
        return result;
    }

    public static List<ChatMessage> LoadMessages(string path)
    {
        var result = new List<ChatMessage>();
        string[] lines = ReadLines(path);
        if (lines.Length <= 1) return result;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(new[] { ',' }, 7);
            if (cols.Length < 6) continue;

            var m = new ChatMessage();
            int.TryParse(cols[0].Trim(), out m.groupId);
            m.senderName = cols[1].Trim().Trim('"');
            m.messageType = cols[2].Trim().Trim('"').ToLower();
            m.messageText = cols[3].Trim().Trim('"');
            m.mediaUrl = cols[4].Trim().Trim('"');
            int.TryParse(cols[5].Trim().Trim('"'), out m.value);
            result.Add(m);
        }
        return result;
    }
}