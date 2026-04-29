using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ItemState { Free, Held, Aimed }
public class Item : MonoBehaviour {
    [Tooltip("How much the item should be offset when it's being held by the sticky hand")]
    [SerializeField] private Vector2 holdOffset;

    [Tooltip("Item's primary collider used for collisions with terrain")]
    [SerializeField] Collider2D col;

    [Tooltip("All colliders, including triggers etc")]
    [field: SerializeField] public Collider2D[] allColliders { get; private set; }

    [SerializeField] private Rigidbody2D rb;

    [field: SerializeField] public ItemState itemState { get; protected set; }

    void Start() {
        Debug.Log($"{gameObject.name}: all colliders: {allColliders[0].GetInstanceID()} + {allColliders[1].GetInstanceID()} + {allColliders[2].GetInstanceID()}");
    }

    void Update() {
        if(transform.position.y < GameController.Instance.KillLevel) {
            Destroy(gameObject);
        }
    }

    // stop held object from falling/colliding
    public virtual void PickUp() {
        transform.localPosition = new Vector3(holdOffset.x, holdOffset.y, 0);

        itemState = ItemState.Held;

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.rotation = 0f;
        
        foreach(Collider2D col in allColliders) {
            col.enabled = false;
        }
    }

    // reenable falling/colliding
    public virtual void Drop() {
        itemState = ItemState.Free;

        rb.isKinematic = false;

        foreach(Collider2D col in allColliders) {
            col.enabled = true;
        }
    }

    public virtual void Use() {
    }
}
