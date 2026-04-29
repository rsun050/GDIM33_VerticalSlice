using Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public static GameController Instance { get; private set; }
	public PlayerController Player { get; private set; }
	public StickyHandController StickyHand { get; private set; }
	[field: SerializeField] public float KillLevel { get; private set; }

	[SerializeField] private GameObject spawnPoint;

	public void Awake() {
		Cursor.visible = false;

		if(Instance != null && Instance != this) {
			Destroy(this);
			return;
		}

		Instance = this;

		GameObject playerObj = GameObject.Find("Player");
		Player = playerObj.GetComponent<PlayerController>();

		GameObject stickyHandObj = GameObject.Find("Sticky Hand");
		StickyHand = stickyHandObj.GetComponent<StickyHandController>();
	}

	public void Start() {
		Player.playerDies += ResetLevel;
	}

	private void ResetLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}