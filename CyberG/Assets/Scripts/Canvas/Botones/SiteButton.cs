using System.Collections;
using System.Collections.Generic;
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


