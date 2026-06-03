using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item {
    [field: SerializeField] public KeyData data { get; private set; }
    [SerializeField] private bool consumeOnUse;

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        // Gizmos.DrawWireCube(
        //     transform.position,
        //     new Vector3(((BoxCollider2D)col).size.x, ((BoxCollider2D)col).size.y, 1)
        // );
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (itemState == ItemState.Free && collision.gameObject.layer == LayerMask.NameToLayer("Door")) {
            Debug.Log("key hit a door");
            if (TryOpenDoor(collision)) {
                if (consumeOnUse) {
                    Destroy(gameObject);
                }
            }
        }
    }

    public override void PickUp() {
        base.PickUp();

        rb.isKinematic = false;
        rb.gravityScale = 0;

        col.isTrigger = true;
    }

    public override void Drop() {
        base.Drop();
        rb.gravityScale = 1;

        col.isTrigger = false;
    }

    public override void Use() {
        if (TryOpenDoor()) {
            if (consumeOnUse) {
                Destroy(gameObject);
            }
        }
    }

    private bool TryOpenDoor(Collision2D collision = null) {
        Collider2D collider;
        if (collision != null) {
            collider = collision.collider;
        }
        else {
            collider = Physics2D.OverlapBox(
                transform.position,
                ((BoxCollider2D)col).size,
                transform.eulerAngles.z,
                LayerMask.GetMask("Door")
            );
        }

        if (collider) {
            LockedDoor lockedDoor = collider.gameObject.GetComponent<LockedDoor>();
            bool canOpenDoor = false;
            if (lockedDoor) {
                if (lockedDoor.key && gameObject == lockedDoor.key) {
                    canOpenDoor = true;
                } else if (lockedDoor.keyType == KeyType.Any || data.keyType == KeyType.Skeleton || data.keyType == lockedDoor.keyType) {
                    canOpenDoor = true;
                }
            }

            if (canOpenDoor) {
                // Debug.Log("opening a door");
                lockedDoor.Switch();
                Use();
                itemConsumed?.Invoke();
                return true;
            }
        }

        return false;
    }
}
