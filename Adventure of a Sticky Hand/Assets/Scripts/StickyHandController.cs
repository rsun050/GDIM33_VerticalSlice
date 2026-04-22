using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;

public class StickyHandController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Camera cam;
    [SerializeField] private LineRenderer arm;
    [SerializeField] private GameObject anchor; // local space
    [SerializeField] private float maxDistance;
    [SerializeField] private float maxSpeed;
    private bool canMove;
    private Vector3 worldAnchor;

    [Header("Colliders")]
    [SerializeField] Collider2D col;
    private Dictionary<int, GameObject> touching;
    private GameObject holding;

    [SerializeField] private TMP_Text debug;

    // Start is called before the first frame update
    void Start() {
        canMove = true;
        touching = new Dictionary<int, GameObject>();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(worldAnchor, maxDistance);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.CompareTag("Item")) {
            touching.Add(col.gameObject.GetInstanceID(), col.gameObject);
            Debug.Log($"can interact with {col.gameObject.name}");
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.CompareTag("Item")) {
            touching.Remove(col.gameObject.GetInstanceID());
            Debug.Log($"can NOT interact with {col.gameObject.name}");
        }
    }

    // Update is called once per frame
    void Update() {
        worldAnchor = anchor.transform.position;
        canMove = !Input.GetKey(KeyCode.LeftShift);

        if(canMove) {
            Move();
        }
        MoveClamp();
        UpdateArm();

        if(Input.GetKeyDown(KeyCode.Mouse0)) {
            LClick();
        }

        if(Input.GetKeyDown(KeyCode.Mouse1)) {
            
        }
    }

    private void Move() {
        // https://www.youtube.com/watch?v=2DHy_l4Ffe0
        transform.position = Vector2.MoveTowards(transform.position, cam.ScreenToWorldPoint(Input.mousePosition), maxSpeed * Time.deltaTime);
    }

    // clamp to anchor
    private void MoveClamp() {
        Vector3 direction = transform.position - worldAnchor;
        direction.z = 0;
        direction = Vector3.Normalize(direction);

        Vector3 furthestAway = worldAnchor + direction * maxDistance; // don't move further than this from anchor

        float minX, maxX, minY, maxY;
        if(direction.x >= 0) { // mouse is right of hand
            minX = worldAnchor.x;
            maxX = furthestAway.x;
        } else { // mouse is left of hand
            minX = furthestAway.x;
            maxX = worldAnchor.x;
        }

        if(direction.y >= 0) { // mouse above hand
            minY = worldAnchor.y;
            maxY = furthestAway.y;
        } else { // mouse below hand
            minY = furthestAway.y;
            maxY = worldAnchor.y;
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
    }

    // update stick hand's arm
    private void UpdateArm() {
        arm.SetPositions(
			new Vector3[] {
                worldAnchor,
                transform.position
            }
        );
    }

    // LClick: trigger actuators, pickup items
    private void LClick() {
        if(holding == null) {
            List<GameObject> gameObjsInContact = new List<GameObject>(touching.Values);
            gameObjsInContact.Sort(CompareGameObjs);

            if(gameObjsInContact.Count < 1) return;

            GameObject clickedObj = gameObjsInContact[0];

            // pick up an item
            if(clickedObj.CompareTag("Item")) {
                holding = clickedObj;

                // lock object to sticky hand
                clickedObj.transform.SetParent(transform);
                clickedObj.transform.localPosition = new Vector3(0,0,0);

                // stop held object from moving/colliding
                clickedObj.GetComponent<Rigidbody2D>().isKinematic = true;
                EventBus.Trigger(EventNames.DisableColliderEvent, clickedObj.GetComponent<Item>().allColliders);
            }
        }
    }

    // RClick: drop items
    private void RClick() {
        if(holding != null) {
            GameObject obj = holding;

            holding = null;
            obj.transform.SetParent(null);
            // obj.GetComponent<Item>().EnableAllColliders();
        }
    }

    // custom comparator: want closest gameobj
    private int CompareGameObjs(GameObject g1, GameObject g2) {
        float g1dist = (g1.transform.position - transform.position).magnitude;
        float g2dist = (g2.transform.position - transform.position).magnitude;

        return (int)(g1dist - g2dist);
    }
}
