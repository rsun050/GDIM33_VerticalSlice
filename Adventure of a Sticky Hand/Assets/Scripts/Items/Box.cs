using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Item {
    // Start is called before the first frame update
    [SerializeField] private PlatformEffector2D effector;
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        // effector.rotationalOffset = -transform.eulerAngles.z;
    }
}
