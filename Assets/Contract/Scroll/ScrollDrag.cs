using UnityEngine;
using System.Collections;

public class ScrollDrag : MonoBehaviour {

	public GameObject target;

	bool grabbed;

	const float UPPER_LIMIT = 0.9f;
	const float LOWER_LIMIT = -0.95f;

	// Use this for initialization
	void Start () {
		grabbed = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(grabbed)
			dragScroll(target);
	}

	/*
	 * Assigns the scroll to be grabbed by the player,
	 * if the scroll is currently active
	 * Inactive scrolls should remain closed and locked
	 */
	public bool grabScroll(GameObject newTarget) {
		if(transform.parent.GetComponent<ContractScroll>().isActive) {
			grabbed = true;

			target = newTarget;

			return true;
		}
		return false;
	}

	/*
	 * Stops tracking the player's controller and closes the scroll
	 */
	public void dropScroll() {
		grabbed = false;

		transform.parent.GetComponent<ContractScroll>().closeScroll();
	}

	/*
	 * When the player grabs the scroll handle ring,
	 * the scroll will follow the y position of the controller within
	 * a limited range of movement
	 */
	void dragScroll(GameObject other) {
		if(!transform.parent.GetComponent<ContractScroll>().getIsScrolling()) {
			//Get a few values for ease of reading
			Transform visualBottom = transform.parent.GetChild(0).GetChild(1);
			Vector3 visualTopPos = transform.parent.GetChild(0).GetChild(0).position;
			Vector3 otherPos = other.transform.position;
			float offset = visualBottom.position.y - transform.position.y;	//between bottom visual and drag point

			//reposition the visual of the bottom of the scroll object
			visualBottom.position = new Vector3(visualBottom.position.x, 
				Mathf.Clamp(otherPos.y + offset, 
					visualTopPos.y + LOWER_LIMIT, 
					visualTopPos.y + UPPER_LIMIT), 
				visualBottom.position.z);
			
			//reposition this point
			transform.position = new Vector3(transform.position.x, 
				Mathf.Clamp(otherPos.y, 
					visualTopPos.y + LOWER_LIMIT - offset, 
					visualTopPos.y + UPPER_LIMIT - offset), 
				transform.position.z);
		}								 
	}
}
