using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Vuforia;

public class Move : MonoBehaviour
{
    public GameObject model;
    public ObserverBehaviour[] ImageTargets;
    public int currentTarget = 0;
    public float speed = 1.0f;
    private bool isMoving = false;

    public Animator animator;

    public void moveToNextMarker()
    {
        if (isMoving || ImageTargets.Length == 0) return;
        StartCoroutine(MoveModel());
    }

    private IEnumerator MoveModel()
    {
        isMoving = true;
        ObserverBehaviour target = GetNextDetectedTarget();

        if (target == null)
        {
            isMoving = false;
            yield break;
        }

        // Hacemos que el modelo rote y mire hacia el target de destino
        model.transform.LookAt(target.transform.position);

        // Encendemos la animación de caminar
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }

        Vector3 startPosition = model.transform.position;
        Vector3 endPosition = target.transform.position;

        float journey = 0;
        while (journey <= 1f)
        {
            journey += Time.deltaTime * speed;
            model.transform.position = Vector3.Lerp(startPosition, endPosition, journey);
            yield return null;
        }

        model.transform.position = endPosition;

        // Apagamos la animación de caminar al llegar al destino
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }

        isMoving = false;
    }

    private ObserverBehaviour GetNextDetectedTarget()
    {
        int checkIndex = (currentTarget + 1) % ImageTargets.Length;

        for (int i = 0; i < ImageTargets.Length; i++)
        {
            ObserverBehaviour target = ImageTargets[checkIndex];

            if (target != null &&
               (target.TargetStatus.Status == Status.TRACKED ||
                target.TargetStatus.Status == Status.EXTENDED_TRACKED))
            {
                currentTarget = checkIndex;
                return target;
            }

            // avanzar al siguiente target si no se encontro
            checkIndex = (checkIndex + 1) % ImageTargets.Length;
        }

        return null;
    }
}