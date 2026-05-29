using System;
using System.Collections;
using Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public static GameController Instance { get; private set; }
	public PlayerController Player { get; private set; }
	public StickyHandController StickyHand { get; private set; }
	public float KillLevel { get; private set; }

	[NonSerialized] public GameObject RespawnPoint;
	public Action<GameObject> RespawnPointChanged;

	[field: SerializeField] public LevelData CurrentLevel { get; private set; }
	[field: SerializeField] public Color DefaultInactiveCheckpointColor { get; private set; } = Color.red;
	[field: SerializeField] public Color DefaultActiveCheckpointColor { get; private set; } = Color.green;

	private bool DEBUG = false;

	public void Awake() {
		// Cursor.visible = false;
		#if UNITY_EDITOR
			DEBUG = true;
		#endif


		if (Instance != null && Instance != this) {
			Destroy(this);
			return;
		}

		Instance = this;

		GameObject playerObj = GameObject.Find("Player");
		Player = playerObj.GetComponent<PlayerController>();

		GameObject stickyHandObj = GameObject.Find("Sticky Hand");
		StickyHand = stickyHandObj.GetComponent<StickyHandController>();

		DontDestroyOnLoad(gameObject);
		DontDestroyOnLoad(playerObj);
		DontDestroyOnLoad(GameObject.Find("StickyHandContainer"));
		DontDestroyOnLoad(GameObject.Find("Canvas"));
	}

	public void Start() {
		Player.playerDies += Respawn;
		PostLevelLoad();
	}

	private void ResetLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	private void Respawn() {
		Player.transform.position = RespawnPoint.transform.position;
		Player.Heal(9999);
	}

	private void GotoNextLevel() {
		StartCoroutine(NextLevel());
	}

	private IEnumerator NextLevel() {
		if (CurrentLevel.NextLevel) { // dont do anything if this is the last level
			CurrentLevel = CurrentLevel.NextLevel;
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(CurrentLevel.SceneName);

			while (!asyncLoad.isDone) {
				yield return null;
			}

			PostLevelLoad();
		}
	}

	private void PostLevelLoad() {
		GameObject levelGoal = GameObject.FindWithTag("LevelGoal");
		levelGoal.GetComponent<LevelGoal>().NextLevel += GotoNextLevel;

		GameObject levelStart = GameObject.FindWithTag("LevelStart");
		RespawnPoint = levelStart;
		RespawnPointChanged?.Invoke(levelStart);

		if(!DEBUG) {
			Player.transform.position = levelStart.transform.position;
		}
		Player.FullHeal();
		KillLevel = CurrentLevel.KillLevel;
	}
}