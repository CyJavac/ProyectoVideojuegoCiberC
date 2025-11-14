using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LockScreenController : MonoBehaviour
{
    [Header("Configuración")]
    public string contraseñaCorrecta = "B055man22"; // Cambia según el nivel
    public InputField inputContraseña;              // Arrastra el InputField
    public Button btnAceptar;                       // Arrastra el botón
    public GameObject panelBloqueo;                 // Este mismo panel (opcional)

    private Coroutine errorCoroutine;

    void Start()
    {
        // Asegurar foco
        inputContraseña.text = "";
        inputContraseña.ActivateInputField();

        // Evento del botón
        btnAceptar.onClick.RemoveAllListeners();
        btnAceptar.onClick.AddListener(Validar);
    }

    private void Validar()
    {
        if (inputContraseña.text == contraseñaCorrecta)
        {
            Desbloquear();
        }
        else
        {
            MostrarError();
        }
    }

    private void MostrarError()
    {
        if (errorCoroutine != null) StopCoroutine(errorCoroutine);

        inputContraseña.text = "Contraseña incorrecta";
        inputContraseña.textComponent.color = Color.red;
        inputContraseña.DeactivateInputField();

        errorCoroutine = StartCoroutine(LimpiarError(2f));
    }

    private IEnumerator LimpiarError(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        inputContraseña.text = "";
        inputContraseña.textComponent.color = Color.white;
        inputContraseña.ActivateInputField();
        errorCoroutine = null;
    }

    private void Desbloquear()
    {
        if (errorCoroutine != null) StopCoroutine(errorCoroutine);
        panelBloqueo.SetActive(false); // Cierra este panel
    }
}