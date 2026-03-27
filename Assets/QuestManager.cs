using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Conexiones Principales")]
    public Move moveScript;
    public Transform[] imageTargets;

    [Header("Personajes y Objetos")]
    public GameObject npcAtomEve;
    public GameObject accesorioMundo;
    public GameObject accesorioJugador;

    [Header("Posicionamiento")] // <-- NUEVO: Control de posición de Eve
    [Tooltip("Mueve a Eve respecto al centro de su Image Target. (X = Derecha/Izquierda, Z = Adelante/Atras)")]
    public Vector3 offsetAtomEve = new Vector3(0.8f, 0f, 0.8f);

    [Header("Interfaz de Usuario")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;

    private int npcTargetIndex;
    private int accesorioTargetIndex;
    private int estadoMision = 0;

    void Start()
    {
        npcTargetIndex = UnityEngine.Random.Range(0, imageTargets.Length);
        do
        {
            accesorioTargetIndex = UnityEngine.Random.Range(0, imageTargets.Length);
        } while (accesorioTargetIndex == npcTargetIndex);

        npcAtomEve.transform.SetParent(imageTargets[npcTargetIndex]);

        // <-- MODIFICADO: Aplicamos el offset en lugar de Vector3.zero
        npcAtomEve.transform.localPosition = offsetAtomEve;

        accesorioMundo.transform.SetParent(imageTargets[accesorioTargetIndex]);
        accesorioMundo.transform.localPosition = Vector3.zero;

        npcAtomEve.SetActive(false);
        accesorioMundo.SetActive(false);
        accesorioJugador.SetActive(false);
        panelDialogo.SetActive(false);
    }

    void OnEnable()
    {
        if (moveScript != null)
        {
            moveScript.OnTargetReached += VerificarProgresoMision;
            moveScript.OnMovementStarted += OcultarPanelInmediato;
        }
    }

    void OnDisable()
    {
        if (moveScript != null)
        {
            moveScript.OnTargetReached -= VerificarProgresoMision;
            moveScript.OnMovementStarted -= OcultarPanelInmediato;
        }
    }

    private void VerificarProgresoMision(int targetAlcanzado)
    {
        if (estadoMision == 0 && targetAlcanzado == npcTargetIndex)
        {
            npcAtomEve.SetActive(true);

            // --- NUEVO: Lógica de Miradas Cruzadas ---
            Vector3 posicionMark = moveScript.model.transform.position;
            Vector3 posicionEve = npcAtomEve.transform.position;

            // Creamos puntos objetivo aplanando el eje Y para evitar inclinaciones extrañas
            Vector3 markMiraHacia = new Vector3(posicionEve.x, posicionMark.y, posicionEve.z);
            Vector3 eveMiraHacia = new Vector3(posicionMark.x, posicionEve.y, posicionMark.z);

            // Aplicamos las rotaciones
            moveScript.model.transform.LookAt(markMiraHacia);
            npcAtomEve.transform.LookAt(eveMiraHacia);
            // ------------------------------------------

            MostrarTexto("¡Mark! Necesitamos equipo. Busca la espada, la he detectado en otro sector.");
            accesorioMundo.SetActive(false);
            estadoMision = 1;
        }
        else if (estadoMision == 1 && targetAlcanzado == accesorioTargetIndex)
        {
            accesorioMundo.SetActive(false);
            accesorioJugador.SetActive(true);

            MostrarTexto("¡Espada equipada! Estamos listos.");
            estadoMision = 2;

            // Lanza una animación de celebración
            if (moveScript.animator != null)
            {
                moveScript.animator.SetTrigger("anim_victory"); 
            }

            StartCoroutine(OcultarPanelConRetraso(3.5f));
        }
    }

    private void MostrarTexto(string mensaje)
    {
        textoDialogo.text = mensaje;
        panelDialogo.SetActive(true);
    }

    private void OcultarPanelInmediato()
    {
        panelDialogo.SetActive(false);
    }

    private IEnumerator OcultarPanelConRetraso(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        panelDialogo.SetActive(false);
    }
}