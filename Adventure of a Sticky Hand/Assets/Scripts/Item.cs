using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Tooltip("Item's primary collider used for collisions with terrain")]
    [SerializeField] Collider2D col;

    [Tooltip("All colliders, including triggers etc")]
    [field: SerializeField] public Collider2D[] allColliders { get; private set; }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
