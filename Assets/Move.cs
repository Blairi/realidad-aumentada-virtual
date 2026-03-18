using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using UnityEngine;
using Vuforia;

public class Move : MonoBehaviour
{
    public GameObject model;
    public ObserverBehaviour[] ImageTargets;
    public int currentTarget;
    public float speed = 1.0f;
    private bool isMoving = false;

    public void moveToNextMarker()
    {
        if (isMoving) return;

        StartCoroutine( MoveModel() );
    }

    private IEnumerator MoveModel()
    {
        isMoving = true;
        ObserverBehaviour target = GetNextDetectedTarget();
        if (target == null)
        {
            isMoving = false;
            yield break; // terminar corrutina
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

        currentTarget = (currentTarget + 1) % ImageTargets.Length;
        isMoving = false;
    }

    private ObserverBehaviour GetNextDetectedTarget()
    {
        foreach (ObserverBehaviour target in ImageTargets)
        {
            if (target != null && (target.TargetStatus.Status == Status.TRACKED || target.TargetStatus.Status == Status.EXTENDED_TRACKED))
            {
                return target;
            }
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
