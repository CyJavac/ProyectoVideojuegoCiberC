using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SiteButton : MonoBehaviour
{
    public string siteName = "Sitio sospechoso";
    public bool esSeguro = false;    // true si es un sitio legal / seguro
    public int damage = 40;          // cuánto baja la vida si se descarga aquí

    // referencia al manager del nivel (asignar en inspector o buscar en Awake)
    public NivelPirateria nivel;

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        if (nivel == null)
        {
            nivel = FindObjectOfType<NivelPirateria>();
        }
        btn.onClick.AddListener(OnClickSite);
    }

    void OnClickSite()
    {
        nivel.ElegirSitio(esSeguro, damage, siteName);
    }
}
