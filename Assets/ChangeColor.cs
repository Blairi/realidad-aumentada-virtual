using System;
using System.Diagnostics;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    public GameObject model;

    public Material[] materialesTraje;

    public void ChangeColor_BTN()
    {
        // Validamos que haya materiales en la lista y que el modelo exista
        if (materialesTraje.Length == 0 || model == null) return;

        // Obtenemos el Renderer del modelo
        Renderer rend = model.GetComponentInChildren<Renderer>();
        int indiceAleatorio = UnityEngine.Random.Range(0, materialesTraje.Length);
        rend.material = materialesTraje[indiceAleatorio];
    }
}