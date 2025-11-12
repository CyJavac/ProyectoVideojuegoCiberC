using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// public class SiteButton : MonoBehaviour
// {
//     public string siteName = "Sitio sospechoso";
//     public bool esSeguro = false;    // true si es un sitio legal / seguro
//     public int damage = 40;          // cuánto baja la vida si se descarga aquí

//     // referencia al manager del nivel (asignar en inspector o buscar en Awake)
//     public NivelPirateria nivel;

//     private Button btn;

//     private void Awake()
//     {
//         btn = GetComponent<Button>();
//         if (nivel == null)
//         {
//             nivel = FindObjectOfType<NivelPirateria>();
//         }
//         btn.onClick.AddListener(OnClickSite);
//     }

//     void OnClickSite()
//     {
//         nivel.ElegirSitio(esSeguro, damage, siteName);
//     }
// }


// public class SiteButton : MonoBehaviour
// {
//     [Header("Configuración del Sitio")]
//     public string siteName = "Sitio sospechoso";

//     [Tooltip("SI es LEGAL → resta créditos. SI es PIRATA → resta salud")]
//     public bool esLegal = false;  // ← Cambiado: ahora es "legal", no "seguro"

//     [Tooltip("Valor: si es legal = costo en créditos, si es pirata = daño en salud")]
//     public int value = 40;

//     [Header("Referencia")]
//     public NivelPirateria nivel;

//     private Button btn;

//     private void Awake()
//     {
//         btn = GetComponent<Button>();
//         if (nivel == null)
//             nivel = FindObjectOfType<NivelPirateria>();

//         if (nivel == null)
//         {
//             Debug.LogError($"[SiteButton] No se encontró NivelPirateria en la escena.");
//             return;
//         }

//         btn.onClick.RemoveAllListeners();
//         btn.onClick.AddListener(OnClickSite);
//     }

//     void OnClickSite()
//     {
//         if (nivel == null) return;

//         if (esLegal)
//         {
//             Debug.Log($"[SiteButton] Sitio LEGAL clickeado: {siteName} (-{value} créditos)");
//             nivel.ElegirSitioLegal(value, siteName);
//         }
//         else
//         {
//             Debug.Log($"[SiteButton] Sitio PIRATA clickeado: {siteName} (-{value} salud)");
//             nivel.ElegirSitioPirata(value, siteName);
//         }
//     }

//     // Opcional: para depuración en el editor
//     private void OnValidate()
//     {
//         if (string.IsNullOrEmpty(siteName))
//             siteName = name;
//     }
// }


using UnityEngine;
using UnityEngine.UI;

public class SiteButton : MonoBehaviour
{
    [Header("Sitio Pirata")]
    public string siteName = "Sitio Pirata";
    public int damage = 40; // ← Daño a la SALUD

    [Header("Referencia")]
    public NivelPirateria nivel;

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        if (nivel == null)
            nivel = FindObjectOfType<NivelPirateria>();

        if (nivel == null)
        {
            Debug.LogError($"[SiteButton] No se encontró NivelPirateria.");
            return;
        }

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClickSite);
    }

    void OnClickSite()
    {
        if (nivel == null) return;

        Debug.Log($"[SiteButton] Descarga pirata: {siteName} (-{damage} salud)");
        nivel.ElegirSitioPirata(damage, siteName);
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(siteName))
            siteName = gameObject.name;
    }
}


