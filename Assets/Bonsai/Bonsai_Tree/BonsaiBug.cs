using UnityEngine;
using System.Collections;

public class BonsaiBug : MonoBehaviour {

	GameObject manager;

	int w;

	float yOrigin;
	float cycleStart;

	float movementRange = 0.1f;

	bool movingUp;

	const float BUG_SPEED = 1.0f;

	static int ID = 0;

	// Use this for initialization
	void Start () {
		//Initialize Variables
		movingUp = true;
		cycleStart = Time.time - Random.Range(0.0f, 1.0f);

		//Name the bug
		this.gameObject.name = "BonsaiBug_" + ID;
		ID++;
	}
	
	// Update is called once per frame
	void Update () {
		float start = movingUp ? yOrigin - movementRange : yOrigin + movementRange;
		float end = movingUp ? yOrigin + movementRange : yOrigin - movementRange;

		transform.localPosition = new Vector3(transform.localPosition.x, 
										  Mathf.Lerp(start, end, (Time.time - cycleStart) * BUG_SPEED), 
										  transform.localPosition.z);

		if(Mathf.Abs(end - transform.localPosition.y) <= 0.01f) {
			movingUp = !movingUp;
			cycleStart = Time.time;
		}
	}

	public void updateYOrigin() {
		yOrigin = transform.localPosition.y;
	}

	public void setYOrigin(float newY) {
		yOrigin = newY;
	}

	public void setMovementRange(float newRange) {
		movementRange = newRange;
	}

	/*
	 * Sets the w position of this leaf and adjusts the color accordingly
	 */
	public void setWPosition(int newW) {
		w = newW;

		assignColorToWPosition();
	}

	/*
	 * Sets the visual color according to the w position
	 */
	void assignColorToWPosition() {
		float cModifier = 1.0f;
		float aModifier = 0.5f;

		//Change Material value
		switch (w) {
			case 0:     //red
				setVisualColor(new Color (1.0f * cModifier, 0.0f, 0.0f, aModifier));
				break;
			case 1:     //orange
				setVisualColor(new Color (1.0f * cModifier, 0.5f * cModifier, 0.0f, aModifier));
				break;
			case 2:     //yellow
				setVisualColor(new Color (1.0f * cModifier, 1.0f * cModifier, 0.0f, aModifier));
				break;
			case 3:     //green
				setVisualColor(new Color (0.0f, 1.0f * cModifier, 0.0f, aModifier));
				break;
			case 4:     //blue
				setVisualColor(new Color (0.0f, 1.0f * cModifier, 1.0f * cModifier, aModifier));
				break;
			case 5:     //indigo
				setVisualColor(new Color (0.0f, 0.0f, 1.0f * cModifier, aModifier));
				break;
			case 6:     //violet
				setVisualColor(new Color (1.0f * cModifier, 0.0f, 1.0f * cModifier, aModifier));
				break;
		}
	}

	/*
	 * Changes the material color of the visual components of this leaf
	 */
	void setVisualColor(Color c) {
		transform.GetChild(0).GetComponent<MeshRenderer>().material.color = c;
	}

	/*
	 * Sets the bonsai manager this leaf answers to
	 */
	public void setManager(GameObject newManager) {
		manager = newManager;
	}
}
