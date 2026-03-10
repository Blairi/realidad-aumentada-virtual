using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAccesory : MonoBehaviour
{
    public GameObject[] accesorios;

    public void EquiparAccesorioAleatorio()
    {
        // Si no hay accesorios en la lista, salimos para evitar errores
        if (accesorios.Length == 0) return;

        // Apagamos todos los accesorios primero para evitar que se encimen
        foreach (GameObject accesorio in accesorios)
        {
            if (accesorio != null)
            {
                accesorio.SetActive(false);
            }
        }

        // Elegimos un índice al azar
        int indice = UnityEngine.Random.Range(0, accesorios.Length);

        // 3. Encendemos solo el accesorio seleccionado
        if (accesorios[indice] != null)
        {
            accesorios[indice].SetActive(true);
        }
    }
}
