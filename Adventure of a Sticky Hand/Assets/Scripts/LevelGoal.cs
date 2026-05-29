using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGoal : MonoBehaviour {
    public Action NextLevel;
    void OnTriggerEnter2D(Collider2D col) {
        // Debug.Log("player hit goal");
        NextLevel?.Invoke();
    }
}
