using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EliminarCorreo : MonoBehaviour
{
    [System.Serializable]
    public class PanelButtonPair
    {
        public Button button;         // Botón que representa el correo
        public GameObject panel;      // Panel que muestra el contenido del correo
    }

    [SerializeField] private List<PanelButtonPair> panelButtonPairs = new List<PanelButtonPair>();

    // Elimina el botón de correo y su panel correspondiente
    public void BorrarCorreo(Button botonEliminar)
    {
        // Buscar el par correspondiente al botón eliminar (asumimos que es hijo directo del botón principal)
        // foreach (var pair in panelButtonPairs)
        // {
        //     if (botonEliminar.transform.IsChildOf(pair.button.transform))
        //     {
        //         Destroy(pair.panel);               // Opcional: también elimina el panel
        //         Destroy(pair.button.gameObject);   // Elimina el botón del correo
        //         panelButtonPairs.Remove(pair);     // Limpieza de la lista
        //         break;
        //     }
        // }
        
        foreach (var pair in panelButtonPairs)
        {
            if (botonEliminar.transform.IsChildOf(pair.button.transform))
            {
                // Verificar si el correo es phishing o legítimo
                Correo correoInfo = pair.button.GetComponent<Correo>();
                if (correoInfo != null)
                {
                    if (correoInfo.esPhishing)
                    {
                        // Correcto: eliminó phishing → suma puntos
                        ScoreManager.Instance.AddScore(50);
                    }
                    else
                    {
                        // Error: eliminó correo legítimo → resta puntos
                        ScoreManager.Instance.AddScore(-30);
                    }
                }

                // Eliminar visualmente
                Destroy(pair.panel);
                Destroy(pair.button.gameObject);
                panelButtonPairs.Remove(pair);
                break;
            }
        }
    }
}
