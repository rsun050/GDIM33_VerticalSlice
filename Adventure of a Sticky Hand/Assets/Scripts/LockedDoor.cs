using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : Door {
	[SerializeField] private SpriteRenderer sprite;
	[SerializeField] private Collider2D col;
	[SerializeField] private AudioSource sfx;

	[Header("Key or KeyType: either accepts a specific key or any key of a matching type. If both are specified, specific key will be given priority.")]
	[SerializeField] private GameObject key;
	[SerializeField] private KeyType keyType;
	
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

	void OnCollisionEnter2D(Collision2D col) {
		if(col.gameObject.GetComponent<Key>() != null) {
			
		}
	}

	public override void Switch() {
		sprite.enabled = !sprite.enabled;
		col.enabled = !col.enabled;
		if(sfx != null) { sfx.Play(); }
	}
}