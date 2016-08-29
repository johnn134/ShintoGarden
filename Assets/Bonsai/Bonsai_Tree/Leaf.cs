using UnityEngine;
using System.Collections;

public class Leaf : MonoBehaviour {

	GameObject manager; //The tree's bonsai manager

	int age;            //The age of this leaf
	int w;              //The position of this leaf on the fourth dimension
	int deathTime;      //The age at which this leaf died

	int depth = 0;                  //The depth of this leaf on the tree
	int minAcceptableCoverage = 1;  //Number of leaves allowed to overshadow this leaf each growth cycle

	float DARKEN_VALUE = 0.1f;
	float DARKEN_ALPHA = 0.75f;

	bool canSnip;       //Whether this leaf can be snipped or not
	bool isDead;       //Whether the leaf is alive or not

	static int ID = 0;

	// Use this for initialization
	void Start() {
		age = 0;
		canSnip = true;
		isDead = false;

		//name the leaf
		this.gameObject.name = "Leaf_" + ID;
		ID++;
	}

	void OnDestroy() {
		if (transform.parent != null)
		{
			if (transform.parent.GetComponent<Branch>() != null)
			{
				transform.parent.GetComponent<Branch>().registerLeafRemoved();
			}
		}
		if (manager != null)
			manager.GetComponent<BonsaiManager>().removeLeaf();
	}

	// Update is called once per frame
	void Update() {

	}

	//Snips this leaf when clicked
	void OnMouseDown() {
		if(canSnip) {
			Destroy(this.gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.transform.parent != null) {
			if(other.transform.parent.GetComponent<Shears>() != null) {
				if(canSnip) {
					Destroy(this.gameObject);
				}
			}
		}
	}

	/*
	 * Ages this leaf
	 */
	public void processGrowthCycle() {
		age++;

		//Check for the leaf to die if alive
		checkForDeath();
	}

	/*
	 * Checks if the leaf will remain alive
	 * Leaf will live if one of these is satisfied:
	 * - it is on a branch tip
	 * - its parent branch is higher than its child branches and
	 *   it has an acceptable number of leaves hanging above it
	 * - it is facing above the horizon and
	 *   it has an acceptable number of leaves hanging above it
	 */
	void checkForDeath() {
		if(!isDead) {
			if(!(checkForBranchTip() || ((checkIsParentBranchHigher() || checkFacingAboveTheHorizon()) && checkOverhangingLeaves() <= minAcceptableCoverage))) {   //Dying
				makeLeafDead();
			}
		}
	}

	public void makeLeafDead() {
		//Set the death time
		deathTime = age;

		//Darken the leaf to show it is dead
		//Color oldColor = transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
		//setVisualColor(new Color (oldColor.r * DARKEN_VALUE, oldColor.g * DARKEN_VALUE, oldColor.b * DARKEN_VALUE, DARKEN_ALPHA));
		transform.GetChild(0).GetComponent<HyperObject>().dullCoef = 4;
		transform.GetChild(0).GetComponent<HyperObject>().WMove(GameObject.FindGameObjectWithTag("Player").GetComponent<HyperCreature>().w);

		isDead = true;
	}

	/*
	 * Checks if the branch this leaf is on has no child branches
	 */
	bool checkForBranchTip() {
		return transform.parent.GetComponent<Branch>().getIsTip();
	}

	/*
	 * Checks if the branch this leaf is on is higher than its child branches
	 */
	bool checkIsParentBranchHigher() {
		return transform.parent.GetComponent<Branch>().getIsHigherThanChildren();
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
	 * Return isDead
	 */
	public bool getIsDead() {
		return isDead;
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

		assignColorToWPosition();
	}

	/*
	 * Sets the visual color according to the w position
	 */
	void assignColorToWPosition() {
		float cModifier = 1.0f;
		float aModifier = 0.5f;

		if(isDead) {
			cModifier = DARKEN_VALUE;
			aModifier = DARKEN_ALPHA;
		}

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
