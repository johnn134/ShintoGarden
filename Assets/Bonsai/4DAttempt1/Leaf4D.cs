using UnityEngine;
using System.Collections;

public class Leaf4D : MonoBehaviour {

	GameObject manager; //The tree's bonsai manager

	int age;            //The age of this leaf
	int w;              //The position of this leaf on the fourth dimension
	int deathTime;      //The age at which this leaf died

	int depth = 0;                  //The depth of this leaf on the tree
	int minAcceptableCoverage = 1;  //Number of leaves allowed to overshadow this leaf each growth cycle

	float darkVal = 0.1f;
	float deadAlpha = 0.75f;

	bool canSnip;       //Whether this leaf can be snipped or not
	bool isAlive;       //Whether the leaf is alive or not

	static int ID = 0;

	// Use this for initialization
	void Start() {
		age = 0;
		canSnip = true;
		isAlive = true;

		//name the leaf
		this.gameObject.name = "Leaf_" + ID;
		ID++;
	}

	// Update is called once per frame
	void Update() {

	}

	//Snips this leaf when clicked
	void OnMouseDown() {
		if(canSnip) {
			transform.parent.GetComponent<Branch4D>().registerLeafRemoved();
			manager.GetComponent<BonsaiManager4D>().removeLeaf();
			Destroy(this.gameObject);
		}
	}

	/*
	 * Ages this leaf
	 */
	public void processGrowthCycle(GameObject branch, GameObject leaf, GameObject bud) {
		age++;

		//Check for the leaf to die if alive
		if(isAlive)
			setIsAlive(checkForLife());
	}

	/*
	 * Wrapper for checking if the leaf will remain alive
	 * Leaf will live if one of these is satisfied:
	 * - it is on a branch tip
	 * - its parent branch is higher than its child branches and
	 *   it has an acceptable number of leaves hanging above it
	 * - it is facing above the horizon and
	 *   it has an acceptable number of leaves hanging above it
	 */
	bool checkForLife() {
		return checkForBranchTip() || ((checkIsParentBranchHigher() || checkFacingAboveTheHorizon()) && checkOverhangingLeaves() <= minAcceptableCoverage);
	}

	/*
	 * Checks if the branch this leaf is on has no child branches
	 */
	bool checkForBranchTip() {
		return transform.parent.GetComponent<Branch4D>().getIsTip();
	}

	/*
	 * Checks if the branch this leaf is on is higher than its child branches
	 */
	bool checkIsParentBranchHigher() {
		return transform.parent.GetComponent<Branch4D>().getIsHigherThanChildren();
	}

	/*
	 * Checks whether the leaf is facing above or below the horizon
	 */
	bool checkFacingAboveTheHorizon() {
		return Vector3.Dot(Vector3.up, transform.up) >= 0;
	}

	/*
	 * Checks if there are leaves above the face point on this leaf
	 */
	int checkOverhangingLeaves() {
		return Physics.RaycastAll(transform.GetChild(1).position, Vector3.up, 100.0f).Length;
	}

	/*
	 * Sets the isAlive boolean and changes the leaf to the dead of alive state
	 */
	public void setIsAlive(bool newIsAlive) {
		if(!isAlive && newIsAlive) {    //Reviving
			setWPosition(w);
		}
		else if(isAlive && !newIsAlive) {   //Dying
			//Set the death time
			deathTime = age;

			//Darken the leaf to show it is dead
			Color oldColor = transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
			//setVisualColor(new Color(oldColor.r * darkVal, oldColor.g * darkVal, oldColor.b * darkVal, deadAlpha));
		}

		isAlive = newIsAlive;
	}

	public bool getIsAlive() {
		return isAlive;
	}

	/*
	 * Sets whether this leaf can be snipped
	 */
	public void setcanSnip(bool canSnip) {
		this.canSnip = canSnip;
	}

	/*
	 * Sets the depth of this leaf
	 */
	public void setDepth(int newDepth) {
		depth = newDepth;
	}

	/*
	 * Sets the w position of this leaf and adjusts the color accordingly
	 */
	public void setWPosition(int newW) {
		w = newW;

		this.transform.GetChild(0).GetComponent<HyperObject>().updateMaterialShaderValues(newW);
	}

	/*
	 * Sets the bonsai manager this leaf answers to
	 */
	public void setManager(GameObject newManager) {
		manager = newManager;
	}
}
