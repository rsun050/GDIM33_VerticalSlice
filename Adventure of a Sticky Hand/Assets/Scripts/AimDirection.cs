using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimDirection : MonoBehaviour {
    [SerializeField] private Camera cam;
    [SerializeField] private LineRenderer aimLine;
    [SerializeField] private float maxLength;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.rotation = Quaternion.identity;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 vecToMousePos = transform.position - mousePos;
        vecToMousePos.z = 0;
        float distToMousePos = Vector3.Magnitude(transform.position - mousePos);

        Vector3 dirToMousePos = transform.position - mousePos;
        dirToMousePos.z = 0;
        dirToMousePos = Vector3.Normalize(dirToMousePos);

        // update the line renderer points based on direction of mouse
        aimLine.SetPosition(1,
            aimLine.GetPosition(0) -
            Mathf.Clamp(distToMousePos, 0f, maxLength) * dirToMousePos
        );
    }

    public Vector3 getDir() {
        Vector3 dir = aimLine.GetPosition(0) - aimLine.GetPosition(1); 
        dir = Vector3.Normalize(dir);

        return dir;
    }
}
