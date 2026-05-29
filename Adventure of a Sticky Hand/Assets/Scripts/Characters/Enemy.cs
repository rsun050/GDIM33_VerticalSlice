using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStatus { Idle, Walk, Pursue, Attack, AttackCooldown, Dead }
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
    [SerializeField] private float targetSearchDistance; // dist at which enemy sees and will start to pursue
    [SerializeField] private float attackDistance; // dist at which enemy is close enough to attack the player
    [SerializeField] private float attackCooldown; // time between attacks
    [SerializeField] private float dmg = 1;

    [Header("Components")]
    [SerializeField] private CapsuleCollider2D col;
    [SerializeField] private CapsuleCollider2D corpseCol;


    [SerializeField] private EnemyStatus startStatus = EnemyStatus.Walk;
    public EnemyStatus status { get; protected set; }
    private float spd;
    private float dir = 1;
    private bool atCliff;
    private bool atWall;

    private float attackCooldownTimeRemaining = 0f;

    // SO MANY F* GIZMOS-
    void OnDrawGizmos() {
        if (status != EnemyStatus.Dead) {
            // player (target)check
            Gizmos.color = Color.yellow;
            Vector3 cubeCenter = pos.transform.position + transform.right / 2 * targetSearchDistance * dir;
            // Gizmos.DrawWireCube(cubeCenter, new Vector3(targetSearchDistance, col.size.y, 1));
            Gizmos.DrawRay(pos.transform.position, transform.right * dir * targetSearchDistance);

            // attackrange
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackOrigin.transform.position, new Vector3(attackDistance, col.size.y/1.5f, 1));

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
    protected override void Start() {
        base.Start();
        status = startStatus;
        spd = patrolSpeed;
    }

    /*
        IDLE: not moving
        WALK: normal patrolling behaviour
        PURSUE: target in sight, attempt to move into attack range
        ATTACK: in attack range, attack
        ATTACK_COOLDOWN: recently attacked, waiting to attack again
        DEAD: killed

        ANY -> DEAD: killed or falls into void

        IDLE -> PURSUE: sees target
        WALK -> PURSUE: ditto

        PURSUE -> WALK: loses target
        PURSUE -> ATTACK: gets in attack range

        ATTACK -> ATTACK_COOLDOWN

        ATTACK_COOLDOWN -> PURSUE: cooldown ends
    */
    // NO ACTUAL MOVEMENT (RB) CODE
    new void Update() {
        if (status != EnemyStatus.Dead) {
            switch(status) {
                case EnemyStatus.Idle:
                case EnemyStatus.Walk: {
                    if (TargetCheck()) {
                        Pursue();
                    }
                    break;
                }
                case EnemyStatus.Pursue: {
                    // Debug.Log("PURSUING PLAYER");
                    if(TargetCheck()) {
                        if (InAttackRange() && !OnAttackCooldown()) {
                            // Debug.Log("ATTACK!!!");
                            Attack();
                        }                            
                    } else { // lost target
                        status = EnemyStatus.Walk;
                        spd = patrolSpeed;
                    }
                    break;
                }
                case EnemyStatus.AttackCooldown: {
                    attackCooldownTimeRemaining -= Time.deltaTime;

                    if(!OnAttackCooldown()) {
                        status = EnemyStatus.Pursue;
                    }
                    break;
                }
            }
            
            RaycastHit2D _groundCheck = GroundCheck();
            atCliff = !_groundCheck || _groundCheck.collider.gameObject.CompareTag("PainGround");
            atWall = WallCheck();
        }
    }

    // movement
    void FixedUpdate() { // make this shit a visual scripting graph MWAHAHAHAHAA!!!
        switch(status) {
            case EnemyStatus.Pursue:
            case EnemyStatus.Walk: {
                if (atCliff || atWall) {
                    // Debug.Log("enemy should turn around");
                    TurnAround();
                }
                else {
                    Walk();
                }

                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -1 * spd, spd), rb.velocity.y);
                break;                    
            }
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

    void Pursue() {
        status = EnemyStatus.Pursue;
        spd = pursueSpeed;
    }

    void TurnAround() {
        dir *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        // sprite.flipX = !sprite.flipX;
    }

    // look for ground ahead to walk on
    RaycastHit2D GroundCheck() {
        RaycastHit2D hit = Physics2D.Raycast(
            pos.transform.position,
            (dir * 0.1f * transform.right + -1 * transform.up).normalized,
            groundCheckDistance,
            groundLayer
        );

        // if(!hit) { Debug.Log("no ground ahead!"); }
        return hit;
    }

    // look if we're colliding into a wall lmao
    bool WallCheck() {
        RaycastHit2D hit = Physics2D.BoxCast(
            pos.transform.position,
            col.size,
            0f,
            dir * transform.right,
            0.3f,
            groundLayer
        );

        // if(hit) { Debug.Log("wall ahead!"); }
        return hit;
    }

    // is an attackable character in view? (shouldn't be able to see past solid terrains like doors, ground, etc)
    bool TargetCheck() {
        // RaycastHit2D hit = Physics2D.BoxCast(pos.transform.position, col.size, 0f, dir * transform.right, targetSearchDistance, targetLayer);
        RaycastHit2D hit = Physics2D.Raycast(pos.transform.position, dir * transform.right, targetSearchDistance, targetLayer);

        if(hit) {
            if(hit.collider.gameObject.CompareTag("Player")) {
                return true;                
            }
        }

        return false;
    }

    float DistanceToTarget() {
        return Vector2.Distance(pos.transform.position, GameController.Instance.Player.transform.position);
    }

    bool InAttackRange() {
        Collider2D hitTarget = Physics2D.OverlapBox(attackOrigin.transform.position, new Vector2(2.5f, 1), 0f, targetLayer);
        return DistanceToTarget() < attackDistance;
    }

    void Attack() {
        Debug.Log("attacking");
        status = EnemyStatus.Attack;
        animator.SetTrigger("attack");
        rb.velocity = Vector2.zero;

        Collider2D hitTarget = Physics2D.OverlapBox(attackOrigin.transform.position, new Vector2(2.5f, 1), 0f, targetLayer);
        if (hitTarget) {
            hitTarget.gameObject.GetComponent<Character>().DealDamage(dmg);
        }

        GoOnAttackCooldown();
    }

    bool OnAttackCooldown() {
        return attackCooldownTimeRemaining > 0f;
    }

    void GoOnAttackCooldown() {
        status = EnemyStatus.AttackCooldown;
        attackCooldownTimeRemaining = attackCooldown;        
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
