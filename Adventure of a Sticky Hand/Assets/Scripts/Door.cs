using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Actuatable {
	[SerializeField] private SpriteRenderer sprite;
	[SerializeField] private Collider2D col;
	[SerializeField] private AudioSource sfx;
	
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