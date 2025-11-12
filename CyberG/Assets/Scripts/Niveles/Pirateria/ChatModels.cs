using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


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
    public string messageType;
    public string messageText;
    public string mediaUrl;
    public int value; // ← daño (salud) o costo (créditos)
}