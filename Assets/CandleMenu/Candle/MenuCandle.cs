using UnityEngine;
using System.Collections;

public class MenuCandle : MonoBehaviour {

	public MenuAction candleAction;

	bool isLit = false;
	bool actionPerformed = false;

	float lightStartTime;

	float ACTIVATION_DELAY = 2.0f;

	public enum MenuAction {
		QUIT, SAVE, SAVEANDQUIT
	};

	// Use this for initialization
	void Start () {
		lightStartTime = 0;
		deactivateCandle();
	}
	
	// Update is called once per frame
	void Update () {
		if(isLit && !actionPerformed) {
			if(Time.time > lightStartTime + ACTIVATION_DELAY) {
				if(performMenuAction())		//if the action failed, turn deactivate candle
					deactivateCandle();
			}
		}
	}

	void OnTriggerStay(Collider other) {
		if(other.name == "LighterZone" && !isLit) {
			if(other.transform.parent.GetComponent<Lighter>().isActivated())
				activateCandle();
		}
	}

	/*
	 * starts the particle effects and initiates the menu action timer
	 */
	public void activateCandle() {
		transform.GetChild(1).GetComponent<ParticleSystem>().Play();
		transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Play();

		lightStartTime = Time.time;
		actionPerformed = false;
		isLit = true;
	}

	/*
	 * stops the particle effects and deactivate the candle
	 */
	public void deactivateCandle() {
		transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
		transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Stop();

		actionPerformed = false;
		isLit = false;
	}

	/*
	 * Calls the application action associated with this candle's
	 * defined MenuAction
	 */
	bool performMenuAction() {
		bool success = false;

		switch(candleAction) {
			case MenuAction.QUIT:
				success = actionQuit();
				break;
			case MenuAction.SAVE:
				success = actionSave();
				break;
			case MenuAction.SAVEANDQUIT:
				success = actionSaveAndQuit();
				break;
			default:
				success = false;
				break;
		}

		actionPerformed = true;
		return success;
	}

	/*
	 * Quit the game
	 */
	bool actionQuit() {
		Debug.Log("Quitting");
		Application.Quit();
		return true;
	}

	/*
	 * Save the game progress
	 */
	bool actionSave() {
		return true;
	}

	/*
	 * Save and quit the game
	 */
	bool actionSaveAndQuit() {
		return true;
	}
}
