using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AccionesCorreo : MonoBehaviour
{
    private Correo correoInfo;
    private bool yaResuelto = false;

    //private Button[] botonesAccion;
    [Header("Botones que se bloquean tras elegir una acción")]
    public List<Button> botonesAccion = new List<Button>(); // 👈 lista vacía en inicio

    private void Awake()
    {
        correoInfo = GetComponent<Correo>();

        // Busca todos los botones hijos en este correo/panel
        //botonesAccion = GetComponentsInChildren<Button>();
    }

    public void Responder()
    {
        if (yaResuelto) return;
        if (correoInfo.esPhishing) ScoreManager.Instance.AddScore(-30);
        else ScoreManager.Instance.AddScore(50);

        FinalizarAccion("Responder");
    }

    public void Reenviar()
    {
        if (yaResuelto) return;
        if (correoInfo.esPhishing) ScoreManager.Instance.AddScore(-30);
        else ScoreManager.Instance.AddScore(50);

        FinalizarAccion("Reenviar");
    }

    public void AbrirEnlace()
    {
        if (yaResuelto) return;
        if (correoInfo.esPhishing) ScoreManager.Instance.AddScore(-50);
        else ScoreManager.Instance.AddScore(50);

        FinalizarAccion("Abrir enlace");
    }

    private void FinalizarAccion(string accion)
    {
        yaResuelto = true;

        // Solo bloquea los botones que arrastres a la lista
        foreach (Button b in botonesAccion)
        {
            b.interactable = false;
        }

        Debug.Log("Acción realizada en este correo: " + accion);
    }
}
