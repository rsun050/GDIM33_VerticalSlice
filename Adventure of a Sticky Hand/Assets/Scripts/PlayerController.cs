using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CapsuleCollider2D col;
    [SerializeField] private Animator animator;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpPower;

    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    private bool canJump;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        RunPlayerHorizontalMovement();
        RunPlayerVerticalMovement();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -1 * groundCheckDistance * transform.up);

        // wallcheck
        Gizmos.DrawCube(transform.position + Mathf.Clamp(transform.localScale.x, -1, 1) * wallCheckDistance * transform.right, new Vector3(col.size.x, col.size.y, 0.01f));
    }

    private void RunPlayerHorizontalMovement() {
        bool walking = false;

        if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            walking = true;
            transform.localScale = new Vector3(-1 * Math.Abs(transform.localScale.x), transform.localScale.y, 1);
            RaycastHit2D wallHit = WallCheck(-1);

            if(wallHit.collider == null) {
                Debug.Log("adding force left");
                rb.AddForce(transform.right * -1 * maxSpeed);            
            }
        } 
        if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            walking = true;
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, 1);
            RaycastHit2D wallHit = WallCheck(1);

            if(wallHit.collider == null) {
                Debug.Log("adding force right");
                rb.AddForce(transform.right * maxSpeed);
            }
        } 

        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -1*maxSpeed, maxSpeed), rb.velocity.y);   
        animator.SetBool("walking", walking);
    }

    private void RunPlayerVerticalMovement() {
        RaycastHit2D groundHit = groundCheck();

        if(canJump && Input.GetKeyDown(KeyCode.Space)) {
            rb.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
            canJump = false;
        } else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            // TODO: platform drop logic
        }

        animator.SetBool("in air", !canJump);
    }

    private RaycastHit2D groundCheck() {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, -1 * transform.up, groundCheckDistance, groundLayers);
        canJump = hitInfo.collider != null;

        return hitInfo;
    }

    // direction: 1 for right, -1 for left
    private RaycastHit2D WallCheck(int direction) {
        RaycastHit2D boxHit = Physics2D.BoxCast(transform.position, col.size, 0f, direction * transform.right, wallCheckDistance, groundLayers);

        return boxHit;
    }
}
