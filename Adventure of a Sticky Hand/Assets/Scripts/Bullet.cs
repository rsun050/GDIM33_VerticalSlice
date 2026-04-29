using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] private float speed;

    void Update() {
        if(transform.position.y < GameController.Instance.KillLevel) {
            Destroy(gameObject);
        }

        if(transform.localScale.x > 0) { // facing right
            transform.Translate(transform.right * speed * Time.deltaTime);        
        } else {
            transform.Translate(transform.right * -1 * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        // TODO: do damage if hit a player/enemy
        Destroy(gameObject);
    }
}
