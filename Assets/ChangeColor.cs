using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    public GameObject model;
    public Material trajeMaterial;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void ChangeColor_BTN()
    {
        Renderer rend = model.GetComponentInChildren<Renderer>();
        rend.material = trajeMaterial;
    }
}