using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ContractScroll : MonoBehaviour {

	public string contractLevel;

	public bool isActive;

	Vector3 scrollBottomStartPos;
	Vector3 scrollDragPointStartPos;

	bool isScrolling;
	bool playerClosed;

	float scrollingStartTime;

	const float OPENED_Y_POS = -0.8f;
	const float CLOSED_Y_POS = 0.9f;
	const float SCROLL_OPEN_SPEED = 2f;
	const float SCROLL_CLOSE_SPEED = 5.0f;

	// Use this for initialization
	void Start () {
		isScrolling = false;
		playerClosed = false;
		openScroll(isActive);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.UpArrow)) {
			if(isActive)
				openScroll(false);
			else
				openScroll(true);
		}

		if(isScrolling) {
			updateScroll();
		}
	}

	/*
	 * Updates the position of the visuals and dragpoint
	 */
	void updateScroll() {
		Transform child = transform.GetChild(0).GetChild(1);
		Transform dragPoint = transform.GetChild(2);
		Vector3 childStart, childEnd, dragStart, dragEnd;
		float offset = child.localPosition.y - dragPoint.localPosition.y;
		float speed;

		if(isActive) {	//opening
			childStart = new Vector3(child.localPosition.x, CLOSED_Y_POS, child.localPosition.z);
			childEnd = new Vector3(child.localPosition.x, OPENED_Y_POS, child.localPosition.z);
			dragStart = new Vector3(dragPoint.localPosition.x, CLOSED_Y_POS - offset, dragPoint.localPosition.z);
			dragEnd = new Vector3(dragPoint.localPosition.x, OPENED_Y_POS - offset, dragPoint.localPosition.z);
			speed = SCROLL_OPEN_SPEED;
		}
		else {	//closing
			if(playerClosed) {
				childStart = scrollBottomStartPos;
				dragStart = scrollDragPointStartPos;
			}
			else {
				childStart = new Vector3(child.localPosition.x, OPENED_Y_POS, child.localPosition.z);
				dragStart = new Vector3(dragPoint.localPosition.x, OPENED_Y_POS - offset, dragPoint.localPosition.z);
			}
			childEnd = new Vector3(child.localPosition.x, CLOSED_Y_POS, child.localPosition.z);
			dragEnd = new Vector3(dragPoint.localPosition.x, CLOSED_Y_POS - offset, dragPoint.localPosition.z);
			speed = SCROLL_CLOSE_SPEED;
		}

		//update current position
		child.localPosition = Vector3.Lerp(childStart, childEnd, (Time.time - scrollingStartTime) * speed);
		dragPoint.localPosition = Vector3.Lerp(dragStart, dragEnd, (Time.time - scrollingStartTime) * speed);

		//check for end of scrolling
		if(child.localPosition == childEnd) {
			isScrolling = false;

			if(isActive == false) {	//closed the scroll
				//Accept the contract and move to the next level

				/*** To-Do: Implement level changing ***/
			}
		}
	}

	/*
	 * Begins the scrolling transition between open or close
	 * true opens the scroll
	 * false closes the scroll
	 */
	void openScroll(bool newState) {
		scrollingStartTime = Time.time;
		isScrolling = true;
		isActive = newState;
		playerClosed = false;
	}

	/*
	 * public method for closing the scroll by player interaction
	 */
	public void closeScroll() {
		Debug.Log("Closing Scroll");
		scrollBottomStartPos = transform.GetChild(0).GetChild(1).localPosition;
		scrollDragPointStartPos = transform.GetChild(2).localPosition;

		scrollingStartTime = Time.time;
		isScrolling = true;
		isActive = false;
		playerClosed = true;
	}

	/*
	 * loads the contract's associated level
	 */
	void loadLevel() {
		if(contractLevel != null) {
			SceneManager.LoadScene(contractLevel);
		}
	}

	/*
	 * Returns isScrolling
	 */
	public bool getIsScrolling() {
		return isScrolling;
	}
}
