using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actuatable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // something that can be toggled (eg: open/close door, turn off/on crusher, direction of conveyor belt)
    public virtual void Switch() {
        
    }

    // something that can be triggered once, possibly over and over (eg: like an animation trigger, or a morse code thingy)
    public virtual void Actuate() {
        
    }
}
