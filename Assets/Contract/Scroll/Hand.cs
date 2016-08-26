using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour {

	GameObject scroll;

	bool grabbing;

	float grabTime;

	// Use this for initialization
	void Start () {
		grabbing = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.T) && grabbing && grabTime + 0.1f < Time.time) {
			Debug.Log("Letting go");
			grabbing = false;
			scroll.GetComponent<ScrollDrag>().dropScroll();
		}
	}

	void OnTriggerStay(Collider other) {
		if(Input.GetKeyDown(KeyCode.T) && !grabbing) {
			if(other.GetComponent<ScrollDrag>() != null) {
				if(other.GetComponent<ScrollDrag>().grabScroll(this.gameObject)) {
					Debug.Log("Grabbing");
					grabbing = true;
					scroll = other.gameObject;
					grabTime = Time.time;
				}
			}
		}
	}
}
