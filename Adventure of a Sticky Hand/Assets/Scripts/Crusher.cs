using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  crushers *should* only move horizontally or vertically.
    thwomp style: crushes when detecting a player in the direction of its ending location, then returns to starting location
    cycle: constantly crushing.
*/

public enum CrusherType { Thwomp, Cycle }
public enum CrusherState { DelayCrush, Crush, DelayRise, Rise }
public class Crusher : MonoBehaviour {
    private CrusherState state;
    [SerializeField] private BoxCollider2D hurtbox;
    [SerializeField] private LayerMask crushableObjects;

    [Header("Crusher Settings")]
    [SerializeField] private Vector2 startingPosition;
    [SerializeField] private Vector2 endingPosition;
    private float currentSpeed;
    [SerializeField] private float crushSpeed;
    [SerializeField] private float returnSpeed; // speed it returns to starting position after crushing
    [SerializeField] private float delayBeforeReturn; // how long before it starts to return to starting position
    private float timer;

    
    // Start is called before the first frame update
    void Start() {
        transform.position = startingPosition;
        state = CrusherState.DelayCrush;
        timer = delayBeforeReturn;
    }

    // Update is called once per frame
    void Update() {
        switch(state) {
            case CrusherState.DelayCrush: {
                timer -= Time.deltaTime;
                if(timer < 0f) {
                    currentSpeed = crushSpeed;
                    state = CrusherState.Crush;
                }
                break;
            }
            case CrusherState.Crush: {
                if(CloseEnoughTo(transform.position, endingPosition)) {
                    timer = delayBeforeReturn;
                    state = CrusherState.DelayRise;    
                } else {
                    UpdateCrush();
                }
                
                break;
            }
            case CrusherState.DelayRise: {
                timer -= Time.deltaTime;
                if(timer < 0f) {
                    currentSpeed = returnSpeed;
                    state = CrusherState.Rise;
                }
                break;
            }
            case CrusherState.Rise: {
                if(CloseEnoughTo(transform.position, startingPosition)) {
                    timer = delayBeforeReturn;
                    state = CrusherState.DelayCrush;
                } else {
                    UpdateRise();
                }
                break;
            }
        }
    }

    void UpdateCrush() {
        transform.position = Vector2.Lerp(
            startingPosition, 
            endingPosition, 
            Mathf.SmoothDamp(
                Vector2.Distance(startingPosition, transform.position) / Vector2.Distance(startingPosition, endingPosition),
                1,
                ref currentSpeed,
                1
            )
        );

        if((Vector2)transform.position == endingPosition) {
            
        }
    }

    void UpdateRise() {
        transform.position = Vector2.Lerp(
            endingPosition, 
            startingPosition, 
            Mathf.SmoothDamp(
                Vector2.Distance(endingPosition, transform.position) / Vector2.Distance(startingPosition, endingPosition),
                1,
                ref currentSpeed,
                1
            )
        );
    }

    void OnCollisionEnter2D(Collision2D col) {
        Debug.Log($"Crusher is crushing {col.gameObject.name}");
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + (Vector3)hurtbox.offset, hurtbox.size, 0f, Vector2.down, 0.1f, crushableObjects);

        if(hit) {
            Character _char = hit.rigidbody.gameObject.GetComponent<Character>();
            if(_char) {
                hit.rigidbody.gameObject.GetComponent<Character>().Kill();            
            }

            // maybe crush an item? ehh
        }
    }

    public bool CloseEnoughTo(Vector2 posA, Vector2 posB) {
        return Vector2.Distance(posA, posB) < 0.01f;
    }

}
