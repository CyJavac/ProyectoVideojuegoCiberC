using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Configuración del cajón")]
    public Axis openingAxis = Axis.Z;  // Eje de apertura
    public bool invertDirection = false; // Si se abre hacia el lado negativo
    public float maxExtension = 0.4f;     // Cuánto puede abrirse
    [HideInInspector] public Vector3 baseLocalPosition; // Se guarda automáticamente al inicio

    private void Awake()
    {
        baseLocalPosition = transform.localPosition;
    }

    public void SetDrawerPosition(float offset)
    {
        // Limita el offset entre 0 y maxExtension
        offset = Mathf.Clamp(offset, 0f, maxExtension);

        // Calcula desplazamiento según eje seleccionado
        Vector3 localOffset = Vector3.zero;

        switch (openingAxis)
        {
            case Axis.X:
                localOffset = new Vector3(offset, 0, 0);
                break;
            case Axis.Y:
                localOffset = new Vector3(0, offset, 0);
                break;
            case Axis.Z:
                localOffset = new Vector3(0, 0, offset);
                break;
        }

        if (invertDirection) localOffset *= -1;

        transform.localPosition = baseLocalPosition + localOffset;
    }
}
