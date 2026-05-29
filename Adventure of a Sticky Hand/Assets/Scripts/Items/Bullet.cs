using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] private float speed;
    [field: SerializeField] public float dmg { get; private set; }
    public Vector3 moveDir;
    public int dir;
    private bool firedWhileAimed;

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.right);
    }

    void Awake() {
        Gun parent = transform.parent.gameObject.GetComponent<Gun>();
        transform.position = parent.transform.position + parent.transform.right * parent.dir;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, parent.dir);
        moveDir = /*parent.dir * */transform.right;
        dir = parent.dir;

        firedWhileAimed = GameController.Instance.StickyHand.holding.GetComponent<Item>().itemState == ItemState.Aimed;

        transform.parent = null; // FREE MEEEEEEE
        transform.Rotate(moveDir);
    }

    void Update() {
        if (transform.position.y < GameController.Instance.KillLevel) {
            Destroy(gameObject);
        }

        if(firedWhileAimed) {
            transform.Translate(/*dir * */transform.right * speed * Time.deltaTime, Space.World);        
        } else {
            transform.Translate(dir * transform.right * speed * Time.deltaTime, Space.World);        
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        // TODO: do damage if hit a player/enemy
        Destroy(gameObject);
    }
}
