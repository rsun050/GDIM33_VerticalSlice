using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Actuatable {
	[SerializeField] protected SpriteRenderer[] sprites;
	[SerializeField] protected Collider2D[] cols;
	[SerializeField] protected AudioSource sfx;
	
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

	public override void Switch() {
		foreach(SpriteRenderer sprite in sprites) {
			sprite.enabled = !sprite.enabled;
		}

		foreach(Collider2D col in cols) {
			col.enabled = !col.enabled;		
		}
		
		if(sfx != null) { sfx.Play(); }
	}
}