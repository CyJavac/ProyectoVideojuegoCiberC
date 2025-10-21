using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Assets/Scripts/Niveles/Pirateria/ChatDataManager.cs

public class ChatDataManager : MonoBehaviour
{
    [Header("Rutas relativas dentro de StreamingAssets")]
    [Tooltip("Ej: Nivel_Pirateria/TelegramGroups.csv")]
    public string groupsCsvPath;
    public string peliculasCsvPath;
    public string videojuegosCsvPath;

    [HideInInspector] public List<ChatGroup> grupos = new List<ChatGroup>();
    [HideInInspector] public List<ChatMessage> mensajesPeliculas = new List<ChatMessage>();
    [HideInInspector] public List<ChatMessage> mensajesVideojuegos = new List<ChatMessage>();

    void Start()
    {
        LoadAllData();
    }

    public void LoadAllData()
    {
        grupos = CsvLoader.LoadGroups(groupsCsvPath);
        mensajesPeliculas = CsvLoader.LoadMessages(peliculasCsvPath);
        mensajesVideojuegos = CsvLoader.LoadMessages(videojuegosCsvPath);

        Debug.Log($"[ChatDataManager] Grupos: {grupos.Count}, Peliculas: {mensajesPeliculas.Count}, Videojuegos: {mensajesVideojuegos.Count}");
    }

    // Helpers rápidos
    public List<ChatMessage> GetMessagesForGroup(int groupId, bool esPelicula)
    {
        var source = esPelicula ? mensajesPeliculas : mensajesVideojuegos;
        return source.FindAll(m => m.groupId == groupId);
    }

    public ChatGroup GetGroupById(int id) => grupos.Find(g => g.groupId == id);
}
