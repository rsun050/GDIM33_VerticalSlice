using TMPro;
using Unity;
using UnityEngine;

public class UIController : MonoBehaviour {
	public static UIController Instance { get; private set; }

	[SerializeField] public TMP_Text ammoUI;

	public void Awake() {
		if(Instance != null && Instance != this) {
			Destroy(this);
			return;
		}

		Instance = this;
	}
	
	public void Start() {
		if(GameController.Instance.StickyHand.holding) {
			if(GameController.Instance.StickyHand.holding.GetComponent<Gun>() != null) {
				GameController.Instance.StickyHand.holding.GetComponent<Gun>().SetAmmoUI();
				ammoUI.gameObject.SetActive(true);
			} else {
				ammoUI.gameObject.SetActive(false);
			}
		}
	}
}