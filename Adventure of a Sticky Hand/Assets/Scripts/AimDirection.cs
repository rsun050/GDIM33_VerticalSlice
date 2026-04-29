using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimDirection : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LineRenderer aimLine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // update the line renderer points based on direction of mouse
    }
}
