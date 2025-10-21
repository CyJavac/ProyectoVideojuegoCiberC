using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Assets/Scripts/Niveles/Pirateria/ChatModels.cs


[Serializable]
public class ChatGroup
{
    public int groupId;
    public string groupName;
    public string category;    // "peliculas" o "videojuegos"
    public string description;
}

[Serializable]
public class ChatMessage
{
    public int groupId;
    public string senderName;
    public string messageType; // "text", "link", "image"
    public string messageText;
    public string mediaUrl;    // url o path dentro de StreamingAssets o assets
    public int damage;         // daño aplicado si el jugador "descarga"
}

