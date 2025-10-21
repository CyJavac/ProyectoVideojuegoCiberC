using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatPopupsManager : MonoBehaviour
{
    [System.Serializable]
    public class ContactoAmigo
    {
        public string nombre;
        public Sprite fotoPerfil;
        [TextArea(2, 5)]
        public List<string> mensajes;
    }

    [Header("Referencias")]
    [SerializeField] private GameObject popupPrefab;      // Prefab con imagen + nombre + mensaje + botón
    [SerializeField] private Transform popupsParent;      // Contenedor donde aparecen los popups
    [SerializeField] private NivelPirateria nivelPirateria; // Referencia para leer salud
    [SerializeField] private float tiempoEntreMensajes = 7.5f; // Tiempo base entre mensajes

    [Header("Contactos disponibles")]
    [SerializeField] private List<ContactoAmigo> contactos = new List<ContactoAmigo>();

    private bool chatActivo = false;
    private float tiempoActualEntreMensajes;

    void OnEnable()
    {
        chatActivo = true;
        tiempoActualEntreMensajes = tiempoEntreMensajes;
        StartCoroutine(GenerarMensajes());
    }

    void OnDisable()
    {
        chatActivo = false;
        StopAllCoroutines();
    }

    private IEnumerator GenerarMensajes()
    {
        while (chatActivo)
        {
            yield return new WaitForSeconds(tiempoActualEntreMensajes);
            CrearPopupMensaje();
            AjustarFrecuenciaPorSalud();
        }
    }

    void AjustarFrecuenciaPorSalud()
    {
        if (nivelPirateria == null) return;

        float salud = nivelPirateria.saludActual;
        // Mientras menor salud, más rápido llegan los mensajes (mínimo 3 seg)
        tiempoActualEntreMensajes = Mathf.Lerp(3f, tiempoEntreMensajes, salud / nivelPirateria.saludMax);
    }

    void CrearPopupMensaje()
    {
        if (popupPrefab == null || popupsParent == null || contactos.Count == 0)
            return;

        // Elegir amigo y mensaje al azar
        ContactoAmigo amigo = contactos[Random.Range(0, contactos.Count)];
        string texto = amigo.mensajes[Random.Range(0, amigo.mensajes.Count)];

        GameObject nuevoPopup = Instantiate(popupPrefab, popupsParent);
        nuevoPopup.SetActive(true);

        // --- Asignar contenido ---
        TMP_Text nombreTxt = nuevoPopup.transform.Find("NombreTxt")?.GetComponent<TMP_Text>();
        TMP_Text mensajeTxt = nuevoPopup.transform.Find("MensajeTxt")?.GetComponent<TMP_Text>();
        Image fotoImg = nuevoPopup.transform.Find("FotoPerfil")?.GetComponent<Image>();
        Button closeButton = nuevoPopup.GetComponentInChildren<Button>();

        if (nombreTxt != null) nombreTxt.text = amigo.nombre;
        if (mensajeTxt != null) mensajeTxt.text = texto;
        if (fotoImg != null) fotoImg.sprite = amigo.fotoPerfil;

        // --- Botón de cierre manual ---
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => Destroy(nuevoPopup));
        }

        // --- Posición aleatoria en pantalla ---
        RectTransform rect = nuevoPopup.GetComponent<RectTransform>();
        if (rect != null)
        {
            float offsetX = Random.Range(-150f, 150f);
            float offsetY = Random.Range(-100f, 100f);
            rect.anchoredPosition += new Vector2(offsetX, offsetY);
        }

        // NO se autodestruyen: deben ser cerrados por el jugador

        // Orden aleatorio
        nuevoPopup.transform.SetAsLastSibling();
    }
    
    public void ReducirIntervalo(float nuevoTiempo)
    {
        tiempoEntreMensajes = Mathf.Max(2f, nuevoTiempo);
    }
}
