using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] private float speed;
    public Vector3 moveDir;
    public int dir;

    void OnDrawGizmos() {
        // Gizmos.color = Color.green;
        // Gizmos.DrawRay(transform.position, transform.right * 2);
    }

    void Awake() {
        Gun parent = transform.parent.gameObject.GetComponent<Gun>();
        moveDir = parent.dir * transform.right;
        dir = parent.dir;

        transform.parent = null; // FREE MEEEEEEE
        transform.Rotate(moveDir);
    }

    void Update() {
        if (transform.position.y < GameController.Instance.KillLevel) {
            Destroy(gameObject);
        }

        transform.Translate(dir * transform.right * speed * Time.deltaTime, Space.World); // sus // gangalang this is so bad how do i still not know when to use world/self space
    }

    private void OnCollisionEnter2D(Collision2D col) {
        // TODO: do damage if hit a player/enemy
        Destroy(gameObject);
    }
}
