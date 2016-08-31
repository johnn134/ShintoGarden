using UnityEngine;
using System.Collections;

public class Lighter : MonoBehaviour {

	bool active = false;

	// Use this for initialization
	void Start () {
		deactivateLighter();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			if(active)
				deactivateLighter();
			else
				activateLighter();
		}
	}

	/*
	 * Starts the fire particles and activates the lighter
	 */
	public void activateLighter() {
		transform.GetChild(1).GetComponent<ParticleSystem>().Play();
		transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Play();

		active = true;
	}

	/*
	 * Stops the fire particles and deactivates the lighter
	 */
	public void deactivateLighter() {
		transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
		transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Stop();

		active = false;
	}

	public bool isActivated() {
		return active;
	}
}
