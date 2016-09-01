using UnityEngine;
using System.Collections;

public class Ladle : MonoBehaviour {

	GameObject player;

	bool isFull;
	bool nearPlayer;

	void Awake() {
		isFull = false;
		nearPlayer = false;
	}

	// Use this for initialization
	void Start () {
		emptyLadle();
	}

	// Update is called once per frame
	void Update () {
		if(isFull) {
			if(Vector3.Dot(transform.parent.up, Vector3.up) < 0.0f) {
				if(nearPlayer) {
					drinkLadle();
				}
				else {
					emptyLadle();
				}
			}
		}
	}

	public void fillLadle() {
		if(!isFull) {
			Debug.Log("Filling Ladle");
			transform.parent.GetChild(0).GetChild(1).GetComponent<Renderer>().enabled = true;
			isFull = true;
		}
	}

	void emptyLadle() {
		Debug.Log("Emptying Ladle");
		transform.parent.GetChild(0).GetChild(1).GetComponent<Renderer>().enabled = false;
		isFull = false;
	}

	public void drinkLadle() {
		Debug.Log("Drinking Ladle");

		if(player != null) {
			if(player.GetComponent<PlayerDrink>() != null)
				player.GetComponent<PlayerDrink>().registerDrink();
		}

		transform.parent.GetChild(0).GetChild(1).GetComponent<Renderer>().enabled = false;
		isFull = false;
	}

	public void touchingPlayer(GameObject other) {
		player = other;
		nearPlayer = true;
	}

	public void leavingPlayer() {
		nearPlayer = false;
	}
}
