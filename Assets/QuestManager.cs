using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Conexiones Principales")]
    public Move moveScript;
    public Transform[] imageTargets;

    [Header("Personajes y Objetos")]
    public GameObject npcAtomEve;
    public GameObject npcOliver;

    public ChangeAccesory inventarioScript;

    public int indiceDeLaEspadaEnInventario = 0;
    public int indiceDeLaMochilaEnInventario = 1;

    [Header("Posicionamiento")]
    [Tooltip("Mueve a los personajes respecto al centro de su Image Target.")]
    public Vector3 offsetAtomEve = new Vector3(0.4f, 0f, 0.4f);
    public Vector3 offsetOliver = new Vector3(-0.4f, 0f, 0.4f);

    [Header("Interfaz de Usuario")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;

    private int npcTargetIndex;
    private int accesorioTargetIndex;
    private int oliverTargetIndex;
    private int mochilaTargetIndex;

    private int estadoMision = 0;

    void Start()
    {
        List<int> targetsDisponibles = new List<int>();
        for (int i = 0; i < imageTargets.Length; i++)
        {
            targetsDisponibles.Add(i);
        }

        //// <-- AJUSTE 2: Validación con mensaje visual en el Panel
        //if (targetsDisponibles.Count < 4)
        //{
        //    MostrarTexto("Mark: necesitamos al menos 4 marcadores para completar la misión.");
        //    return;
        //}

        npcTargetIndex = SacarTargetAleatorio(targetsDisponibles);
        accesorioTargetIndex = SacarTargetAleatorio(targetsDisponibles);
        oliverTargetIndex = SacarTargetAleatorio(targetsDisponibles);
        mochilaTargetIndex = SacarTargetAleatorio(targetsDisponibles);

        npcAtomEve.transform.SetParent(imageTargets[npcTargetIndex]);
        npcAtomEve.transform.localPosition = offsetAtomEve;
        npcAtomEve.SetActive(false);

        npcOliver.transform.SetParent(imageTargets[oliverTargetIndex]);
        npcOliver.transform.localPosition = offsetOliver;
        npcOliver.SetActive(false);

        panelDialogo.SetActive(false);
    }

    private int SacarTargetAleatorio(List<int> lista)
    {
        int indexAleatorio = UnityEngine.Random.Range(0, lista.Count);
        int valorTarget = lista[indexAleatorio];
        lista.RemoveAt(indexAleatorio);
        return valorTarget;
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
            HacerQueSeMiren(npcAtomEve);

            MostrarTexto("Atom Eve: ¡Mark! Necesitamos equipo. Busca la espada en otro sector.");
            estadoMision = 1;
        }
        else if (estadoMision == 1 && targetAlcanzado == accesorioTargetIndex)
        {
            if (inventarioScript != null) inventarioScript.DesbloquearYEquiparEspecifico(indiceDeLaEspadaEnInventario);

            MostrarTexto("¡Espada equipada! Ahora vamos a buscar a Oliver.");
            estadoMision = 2;

            // <-- AJUSTE 3: Buscamos el Animator directamente en el modelo de Mark
            if (moveScript.model.TryGetComponent<Animator>(out Animator markAnim))
            {
                markAnim.SetTrigger("anim_victory");
            }

            // animamos a Atom EVe
            if (npcAtomEve.TryGetComponent<Animator>(out Animator atomAnim))
            {
                atomAnim.SetTrigger("eve_celebration");
            }

            StartCoroutine(OcultarPanelConRetraso(3.5f));
        }
        else if (estadoMision == 2 && targetAlcanzado == oliverTargetIndex)
        {
            npcOliver.SetActive(true);
            HacerQueSeMiren(npcOliver);

            MostrarTexto("Oliver: ¡Hermano! Olvidé mi mochila, ¿puedes ayudarme a encontrarla?");
            estadoMision = 3;
        }
        else if (estadoMision == 3 && targetAlcanzado == mochilaTargetIndex)
        {
            if (inventarioScript != null) inventarioScript.DesbloquearYEquiparEspecifico(indiceDeLaMochilaEnInventario);

            MostrarTexto("¡Mochila equipada! El equipo está listo.");
            estadoMision = 4;

            if (moveScript.model.TryGetComponent<Animator>(out Animator markAnim))
            {
                markAnim.SetTrigger("anim_victory");
            }

            // <-- AJUSTE 3: Buscamos el Animator directamente en el modelo de Oliver
            if (npcOliver.TryGetComponent<Animator>(out Animator oliverAnim))
            {
                oliverAnim.SetTrigger("oliver_celebration");
            }

            StartCoroutine(OcultarPanelConRetraso(4.0f));
        }
    }

    private void HacerQueSeMiren(GameObject npc)
    {
        Vector3 posicionMark = moveScript.model.transform.position;
        Vector3 posicionNPC = npc.transform.position;

        Vector3 markMiraHacia = new Vector3(posicionNPC.x, posicionMark.y, posicionNPC.z);
        Vector3 npcMiraHacia = new Vector3(posicionMark.x, posicionNPC.y, posicionMark.z);

        moveScript.model.transform.LookAt(markMiraHacia);
        npc.transform.LookAt(npcMiraHacia);
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