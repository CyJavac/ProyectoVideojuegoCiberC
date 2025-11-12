using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChatDataManager : MonoBehaviour
{
    [Header("Configuración del Sitio")]
    public bool isLegalSite = false; // ← AQUÍ SE DEFINE SI ES LEGAL O PIRATA

    [Header("Rutas CSV")]
    public string groupsCsvPath;
    public string peliculasCsvPath;
    public string videojuegosCsvPath;

    [HideInInspector] public List<ChatGroup> grupos = new List<ChatGroup>();
    [HideInInspector] public List<ChatMessage> mensajesPeliculas = new List<ChatMessage>();
    [HideInInspector] public List<ChatMessage> mensajesVideojuegos = new List<ChatMessage>();

    void Start() => LoadAllData();

    public void LoadAllData()
    {
        grupos = CsvLoader.LoadGroups(groupsCsvPath);

        if (!string.IsNullOrEmpty(peliculasCsvPath))
            mensajesPeliculas = CsvLoader.LoadMessages(peliculasCsvPath);

        if (!string.IsNullOrEmpty(videojuegosCsvPath))
            mensajesVideojuegos = CsvLoader.LoadMessages(videojuegosCsvPath);

        Debug.Log($"[ChatDataManager] {name}: " +
                  $"Grupos: {grupos.Count}, " +
                  $"Películas: {mensajesPeliculas.Count}, " +
                  $"Juegos: {mensajesVideojuegos.Count} | " +
                  $"Legal: {isLegalSite}");
    }

    public List<ChatMessage> GetMessagesForGroup(int groupId, string category)
    {
        if (category.Contains("pelic")) return mensajesPeliculas.FindAll(m => m.groupId == groupId);
        return mensajesVideojuegos.FindAll(m => m.groupId == groupId);
    }
}


