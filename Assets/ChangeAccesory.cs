using System;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAccesory : MonoBehaviour
{
    [Header("Modelos 3D")]
    public GameObject[] accesorios;

    [Header("Colección Desbloqueada")]
    public bool[] desbloqueados;

    void Start()
    {
        // apagamos todos los accesorios al inicio
        for (int i = 0; i < accesorios.Length; i++)
        {
            if (accesorios[i] != null)
            {
                accesorios[i].SetActive(false);
            }
        }
    }

    public void EquiparAccesorioAleatorio()
    {
        // Evitamos errores si no hay accesorios configurados
        if (accesorios == null || accesorios.Length == 0 || desbloqueados.Length != accesorios.Length)
            return;

        // 1. Filtramos accesorios que tenemos desbloqueados
        List<int> opcionesValidas = new List<int>();
        for (int i = 0; i < desbloqueados.Length; i++)
        {
            if (desbloqueados[i] == true)
            {
                opcionesValidas.Add(i);
            }
        }

        // Si no hay ninguno desbloqueado, no hacemos nada
        if (opcionesValidas.Count == 0)
            return;

        // 2. Elegimos al azar un índice SOLO de nuestra lista de opciones válidas
        int indiceAleatorio = UnityEngine.Random.Range(0, opcionesValidas.Count);
        int indiceGanador = opcionesValidas[indiceAleatorio];

        // 3. Encendemos el indice y apagamos estrictamente todos los demás
        SincronizarVista(indiceGanador);
    }

    // <-- Desbloquea un ítem y lo equipa al instante (llamada por la narrativa)
    public void DesbloquearYEquiparEspecifico(int indice)
    {
        // Validación de seguridad para no tronar el juego si el índice no existe
        if (accesorios == null || indice < 0 || indice >= desbloqueados.Length) return;

        // 1. Desbloqueamos el ítem en la memoria
        desbloqueados[indice] = true;

        // 2. Encendemos el modelo 3D y apagamos los demás
        SincronizarVista(indice);
    }

    // <-- lógica de SetActive 
    private void SincronizarVista(int indiceAEncender)
    {
        for (int i = 0; i < accesorios.Length; i++)
        {
            if (accesorios[i] != null)
            {
                accesorios[i].SetActive(i == indiceAEncender);
            }
        }
    }
}