using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Actuatable {
	[SerializeField] protected SpriteRenderer sprite;
	[SerializeField] protected Collider2D col;
	[SerializeField] protected AudioSource sfx;
	
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

	public override void Switch() {
		sprite.enabled = !sprite.enabled;
		col.enabled = !col.enabled;
		if(sfx != null) { sfx.Play(); }
	}
}