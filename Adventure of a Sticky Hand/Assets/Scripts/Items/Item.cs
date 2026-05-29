using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState { Free, Held, Aimed }
public class Item : MonoBehaviour {
    [Tooltip("How much the item should be offset when it's being held by the sticky hand")]
    [SerializeField] private Vector2 holdOffset;

    [Tooltip("Item's primary collider used for collisions with terrain")]
    [SerializeField] protected Collider2D col;

    [Tooltip("All colliders, including triggers etc")]
    [field: SerializeField] public Collider2D[] allColliders { get; private set; }

    [SerializeField] protected Rigidbody2D rb;

    public ItemState itemState { get; protected set; }

    public Action itemConsumed;

    void Start() {
    }

    void Update() {
        if (transform.position.y < GameController.Instance.KillLevel) {
            Destroy(gameObject);
        }
    }

    // stop held object from falling/colliding
    public virtual void PickUp() {
        transform.localPosition = new Vector3(holdOffset.x/* + 0.3f*/, holdOffset.y, 0);
        transform.rotation = Quaternion.identity;

        itemState = ItemState.Held;

        rb.isKinematic = true;
        // rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.rotation = 0f;

        foreach (Collider2D col in allColliders) {
            col.enabled = false;
        }
    }

    // reenable falling/colliding
    public virtual void Drop() {
        itemState = ItemState.Free;

        rb.isKinematic = false;
        // rb.gravityScale = 1;

        foreach (Collider2D col in allColliders) {
            col.enabled = true;
        }
    }

    public virtual void Throw(Vector3 dir) {
        this.Drop();

        // apply some violent force to throw it
        // rb.AddForce(dir, ForceMode2D.Force);
        rb.velocity = dir;
    }

    public virtual void Use() {
    }

    public virtual void Aim() {
        itemState = ItemState.Aimed;
    }
}
