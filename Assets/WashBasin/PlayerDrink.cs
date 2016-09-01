using UnityEngine;
using System.Collections;

public class PlayerDrink : MonoBehaviour {

	int player_periph;

	float drinkStartTime;

	bool isEffected;

	const float VISION_DURATION = 5.0f;

	void Awake() {
		isEffected = false;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(isEffected) {
			if(Time.time > drinkStartTime + VISION_DURATION) {
				removeDrinkEffect();
				isEffected = false;
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.name == "WaterZone") {
			other.GetComponent<Ladle>().touchingPlayer(this.gameObject);
		}
	}

	void OnTriggerExit(Collider other) {
		if(other.name == "WaterZone") {
			other.GetComponent<Ladle>().leavingPlayer();
		}
	}

	public void registerDrink() {
		addDrinkEffect();
		drinkStartTime = Time.time;
		isEffected = true;
	}

	void addDrinkEffect() {
		player_periph = GetComponent<HyperCreature>().w_perif;
		GetComponent<HyperCreature>().w_perif = 7;
	}

	void removeDrinkEffect() {
		GetComponent<HyperCreature>().w_perif = player_periph;
	}
}
