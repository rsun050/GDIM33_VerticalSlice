using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCheckpoint : MonoBehaviour {
	[SerializeField] private SpriteRenderer sprite;
	void Start() {
		GameController.Instance.RespawnPointChanged += ChangeActiveState;
	}

	void OnDestroy() {
		GameController.Instance.RespawnPointChanged -= ChangeActiveState;
	}

	void OnTriggerEnter2D(Collider2D col) {
		// this becomes the active respawn point
		GameController.Instance.RespawnPoint = gameObject;
		GameController.Instance.RespawnPointChanged?.Invoke(gameObject);
		// Debug.Log("touched by player");
	}

	void ChangeActiveState(GameObject newSpawnpoint) {
		ChangeColor(gameObject == newSpawnpoint);
	}

	private void ChangeColor(bool active) {
		if(GameController.Instance.CurrentLevel.UseDefaultColors) {
			sprite.color = (active) 
				? GameController.Instance.DefaultActiveCheckpointColor
				: GameController.Instance.DefaultInactiveCheckpointColor
			;	
		} else {
			sprite.color = (active) 
				? GameController.Instance.CurrentLevel.ActiveCheckpointColor
				: GameController.Instance.CurrentLevel.InactiveCheckpointColor
			;	
		}
	}
}
