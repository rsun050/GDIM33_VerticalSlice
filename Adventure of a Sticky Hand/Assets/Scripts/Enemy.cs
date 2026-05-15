using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStatus { Idle, Pursue, Attack, Dead }
public class Enemy : Character {

    [Header("Movement")]
    [SerializeField] private GameObject pos;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float pursueSpeed;

    private float groundCheckDistance = 1.5f;

    [Header("Combat")]
    [SerializeField] private GameObject attackOrigin;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float targetSearchDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float dmg = 1;

    [Header("Components")]
    [SerializeField] private CapsuleCollider2D col;
    [SerializeField] private CapsuleCollider2D corpseCol;


    public EnemyStatus status { get; protected set; }
    private float spd;
    private float dir = 1;
    private bool atCliff;
    private bool atWall;

    private bool canAttack = true;
    private float cooldownTimeRemaining = 0f;

    // SO MANY F* GIZMOS-
    void OnDrawGizmos() {
        if (status != EnemyStatus.Dead) {
            // player (target)check
            Gizmos.color = Color.yellow;
            Vector3 cubeCenter = pos.transform.position + transform.right / 2 * targetSearchDistance * dir;
            Gizmos.DrawWireCube(cubeCenter, new Vector3(targetSearchDistance, col.size.y, 1));

            // attackrange
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackOrigin.transform.position, new Vector3(2.5f, 1, 1));

            // groundcheck
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(
                pos.transform.position,
                (dir * 0.1f * transform.right + -1 * transform.up).normalized * groundCheckDistance
            );

            // wallcheck
            cubeCenter = pos.transform.position + transform.right / 2 * dir;
            Gizmos.DrawWireCube(cubeCenter, new Vector3(col.size.x, col.size.y, 1));

        }
    }

    // Start is called before the first frame update
    void Start() {

    }

    new void Update() {
        if (status != EnemyStatus.Dead) {
            if (cooldownTimeRemaining > 0f) {
                cooldownTimeRemaining -= Time.deltaTime;
            }
            else {
                canAttack = true;

                if (TargetCheck()) {
                    status = EnemyStatus.Pursue;
                    spd = pursueSpeed;

                    if (DistanceToTarget() < attackDistance && canAttack) {
                        Attack();
                    }
                }
                else {
                    status = EnemyStatus.Idle;
                    spd = patrolSpeed;
                }

                atCliff = !GroundCheck();
                atWall = WallCheck();
            }
        }
    }

    void FixedUpdate() { // make this shit a visual scripting graph MWAHAHAHAHAA!!!
        if (status != EnemyStatus.Dead) {
            if (atCliff || atWall) {
                // Debug.Log("enemy should turn around");
                TurnAround();
            }
            else {
                Walk();
            }

            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -1 * spd, spd), rb.velocity.y);
        }
    }

    protected override void Die() {
        if (transform.position.y < GameController.Instance.KillLevel) {
            // fell off the map
            Destroy(gameObject);
            return;
        }
        else {
            status = EnemyStatus.Dead;
            animator.SetBool("dead", true);
            col.enabled = false;
            corpseCol.enabled = true;
        }
    }

    void Walk() {
        rb.AddForce(transform.right * dir * spd, ForceMode2D.Impulse);
    }

    void TurnAround() {
        dir *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        // sprite.flipX = !sprite.flipX;
    }


    // look for ground ahead to walk on
    bool GroundCheck() {
        return Physics2D.Raycast(
            pos.transform.position,
            (dir * 0.1f * transform.right + -1 * transform.up).normalized,
            groundCheckDistance,
            groundLayer
        );
    }

    // look if we're colliding into a wall lmao
    bool WallCheck() {
        return Physics2D.BoxCast(
            pos.transform.position,
            col.size,
            0f,
            dir * transform.right,
            0.3f,
            groundLayer
        );
    }

    // is an attackable character in range?
    bool TargetCheck() {
        return Physics2D.BoxCast(pos.transform.position, col.size, 0f, dir * transform.right, targetSearchDistance, targetLayer);
    }

    float DistanceToTarget() {
        return Vector2.Distance(pos.transform.position, GameController.Instance.Player.transform.position);
    }

    void Attack() {
        status = EnemyStatus.Attack;
        animator.SetTrigger("attack");

        Collider2D hitTarget = Physics2D.OverlapBox(attackOrigin.transform.position, new Vector2(2.5f, 1), 0f, targetLayer);
        if (hitTarget) {
            hitTarget.gameObject.GetComponent<Character>().DealDamage(dmg);
        }

        canAttack = false;
        cooldownTimeRemaining = attackCooldown;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Projectile")) {
            Bullet projData = col.gameObject.GetComponent<Bullet>();
            if (projData) {
                TakeDamage(projData.dmg);
            }
        }
    }
}
