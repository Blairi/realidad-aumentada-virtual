using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Conexiones Principales")]
    public Move moveScript; // script de movimiento
    public Transform[] imageTargets;

    [Header("Personajes y Objetos")]
    public GameObject npcAtomEve;
    public GameObject accesorioMundo; 
    public GameObject accesorioJugador;

    [Header("Interfaz de Usuario")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;

    private int npcTargetIndex;
    private int accesorioTargetIndex;

    // 0 = Buscando a Eve, 1 = Buscando Accesorio, 2 = Misión Completada
    private int estadoMision = 0;

    void Start()
    {
        // 1. Sorteo aleatorio de ubicaciones
        npcTargetIndex = Random.Range(0, imageTargets.Length);

        // do-while garantiza que la espada nunca aparezca en el mismo target que Eve
        do
        {
            accesorioTargetIndex = Random.Range(0, imageTargets.Length);
        } while (accesorioTargetIndex == npcTargetIndex);

        // 2. Movemos físicamente y emparentamos los objetos a sus respectivos targets sorteados
        npcAtomEve.transform.SetParent(imageTargets[npcTargetIndex]);
        npcAtomEve.transform.localPosition = Vector3.zero;

        accesorioMundo.transform.SetParent(imageTargets[accesorioTargetIndex]);
        accesorioMundo.transform.localPosition = Vector3.zero;

        // 3. Apagamos todo al inicio para no revelar la ubicación de los mismos.
        npcAtomEve.SetActive(false);
        accesorioMundo.SetActive(false);
        accesorioJugador.SetActive(false);
        panelDialogo.SetActive(false);
    }

    // Nos suscribimos al evento de Move.cs
    void OnEnable()
    {
        if (moveScript != null) moveScript.OnTargetReached += VerificarProgresoMision;
    }

    // Nos desuscribimos para evitar errores de memoria
    void OnDisable()
    {
        if (moveScript != null) moveScript.OnTargetReached -= VerificarProgresoMision;
    }

    private void VerificarProgresoMision(int targetAlcanzado)
    {
        // FASE 1: Encontrar a Atom Eve
        if (estadoMision == 0 && targetAlcanzado == npcTargetIndex)
        {
            npcAtomEve.SetActive(true);
            MostrarTexto("Atom Eve: ¡Mark! Necesitamos equipo. Busca la espada, la he detectado en otro sector.");

            estadoMision = 1; // Avanzamos al siguiente estado de la historia
        }
        // FASE 2: Recoger la Espada
        else if (estadoMision == 1 && targetAlcanzado == accesorioTargetIndex)
        {
            accesorioMundo.SetActive(false); // Desaparece del mundo
            accesorioJugador.SetActive(true); // Aparece en la mano

            MostrarTexto("¡Espada equipada! Estamos listos.");
            estadoMision = 2; // Misión terminada

            // Lanza una animación 
            if (moveScript.animator != null)
            {
                 moveScript.animator.SetTrigger("anim_victory");
            }
        }
    }

    private void MostrarTexto(string mensaje)
    {
        textoDialogo.text = mensaje;
        panelDialogo.SetActive(true);
    }
}