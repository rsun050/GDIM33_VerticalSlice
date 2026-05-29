using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActuatorType { Toggle, OnOff, Reverse }
public class Lever : Actuator {
    [SerializeField] private ActuatorType type;
    [SerializeField] private SpriteRenderer sprite;
    
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private int startSprite;
    private int currSprite;

    void Start() {
        sprite.sprite = sprites[startSprite];
        currSprite = startSprite;
    }
    public override void Trigger() {
        switch(type) {
            case ActuatorType.Toggle:
            case ActuatorType.OnOff:
            case ActuatorType.Reverse:
                Toggle();
                break;
        }
    }

    private void Toggle() {
        currSprite = (currSprite + 1) % sprites.Length;
        sprite.sprite = sprites[currSprite];

        foreach(Actuatable actuatee in actuatees) {
            actuatee.gameObject.SetActive(!actuatee.gameObject.activeInHierarchy);        
        }
    }
}
