using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    // [SerializeField] private 

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;
    [SerializeField] private AudioSource sfx;
    [SerializeField] private ParticleSystem vfx;
    [SerializeField] private Actuatable actuatee;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if(transform.position.y < GameController.Instance.KillLevel) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.CompareTag("Projectile")) {
            // activate self animation
            Fall();

            // activate thing this triggers (if any)
            if(actuatee != null) {
                actuatee.Switch();
            }

            // vfx , sfx
            sfx.Play();
            vfx.Play();            
        }
    }

    private void Fall() {
        col.enabled = false;
        rb.isKinematic = false;
    }
}
