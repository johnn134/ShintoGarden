using UnityEngine;
using System.Collections;

public class Branch : MonoBehaviour {

	GameObject manager; //Reference to the BonsaiManager
	GameObject branch;  //Branch prefab
	GameObject leaf;    //Leaf bundle prefab
	GameObject bud;     //Bud prefab

	int age;            //How many growth cycles has this branch lived through
	int numLeaves;      //Number of leaves on this branch
	int numBranches;    //Number of branches on this branch
	int growthStep;     //Marks which step of the growth order is in effect
	int growthCounter;  //Counter for iterating through branch children
	int w;              //Position on the fourth dimension
	int deathTime;

	float budRange;     //Maximum distance from the center of the branch tip to grow a bud
	float leafRange;    //Stores the radius of the rounded branch tip

	bool canSnip;       //Tells whether this branch can be snipped off
	bool hasDisease;    //Is this branch diseased
	bool hasBugs;       //Does ths branch have bugs on it
	bool runGrowth;     //Tells whether the branch should be growing
	bool isTip;         //True if this branch has no child branches

	int branchMin = 1;              //Minimum number of branch buds that will initially grow
	int branchMax = 3;              //Maximum number of branch buds that can ever grow from this branch
	int leafMin = 0;                //Minimum number of leaf buds that can grow
	int leafMax = 5;                //Maximum number of leaf buds that can ever grow
	int maxPlacementAttempts = 20;  //Maximum attempts for a bud to place itself before giving up
	int branchGrowthCycle = 3;      //Must be at least 1, number of growth cycles between growing new branches
	int leafGrowthCycle = 2;        //Must be at least 1, number of growth cycles between growing new leaves
	int minLeafDepth = 3;           //Must be at least 0, depth of branches before leaves can grow
	int depth = 0;                  //The depth of this branch in the tree

	float budOffset = 60.0f;        //Minimum angle between new branch buds
	float leafOffset = 45.0f;       //Minimum angle between new leaf buds
	float darkVal = 0.1f;
	float darkAlpha = 0.75f;

	static int ID = 0;

	// Use this for initialization
	void Start() {
		age = 0;
		numLeaves = 0;
		numBranches = 0;
		canSnip = true;
		hasDisease = false;
		hasBugs = false;
		isTip = true;

		budRange = transform.GetChild(0).GetChild(2).localScale.x / 2.0f * 0.9f;
		leafRange = transform.GetChild(0).GetChild(2).localScale.x / 2.0f;

		//Name the branch
		this.gameObject.name = "Branch_" + ID;
		ID++;
	}

	// Update is called once per frame
	void Update() {

	}

	void FixedUpdate() {
		//If the tree needs to grow, run the growth process on each part of this branch
		if(runGrowth) {
			if(growthStep >= 5) {   //End the growth cycle
				runGrowth = false;

				//Age the branch
				age++;
			}
			else {  //Process the branch parts in growth order
				bool nextStep = false;

				//Growth order: check for disease, age leaves, evolve buds, grow new buds, process next branch
				switch(growthStep) {
					case 0:
						checkForDisease();
						nextStep = true;
						break;
					case 1:
						nextStep = processLeaves();
						break;
					case 2:
						nextStep = processBuds();
						break;
					case 3:
						growBuds();
						nextStep = true;
						break;
					case 4:
						nextStep = processBranches();
						break;
				}
				if(nextStep) {  //reset counter for next growth step
					growthStep++;
					growthCounter = transform.childCount - 1;
				}
				else {  //decrement growth counter after each child check
					growthCounter--;
				}
			}
		}
	}

	//Clip this branch when clicked
	void OnMouseDown() {
		if(canSnip) {
			transform.parent.GetComponent<Branch>().registerBranchRemoved();
			manager.GetComponent<BonsaiManager>().removeBranch();
			Destroy(this.gameObject);
		}
	}

	/*
	 * Initiates the growth cycle for this branch and children parts
	 */
	public void processGrowthCycle(GameObject newBranch, GameObject newLeaf, GameObject newBud) {
		if(!runGrowth) {
			//Grab prefabs
			branch = newBranch;
			leaf = newLeaf;
			bud = newBud;

			//Start growth cycle
			runGrowth = true;
			growthStep = 0;
			growthCounter = transform.childCount - 1;

			//Note: the tree is aged at the end of the growth cycle in the fixedUpdate
		}
	}

	/*
	 * Calls the processGrowthCycle on the currently focused branch
	 */
	bool processBranches() {
		if(growthCounter <= 2) {
			return true;
		}

		Transform temp = transform.GetChild(growthCounter);
		if(temp.name.Length >= 6) {
			if(temp.name.Substring(0, 6) == "Branch") {
				temp.GetComponent<Branch>().processGrowthCycle(branch, leaf, bud);
			}
		}

		return false;
	}

	/*
	 * Calls the processGrowthCycle on the currently focused bud
	 */
	bool processBuds() {
		if(growthCounter <= 2) {
			return true;
		}

		Transform temp = transform.GetChild(growthCounter);
		if(temp.name.Length >= 3) {
			if(temp.name.Substring(0, 3) == "Bud") {
				temp.GetComponent<Bud>().processGrowthCycle(branch, leaf, bud);
			}
		}

		return false;
	}

	/*
	 * Calls the processGrowthCycle on the currently focused leaf
	 */
	bool processLeaves() {
		if(growthCounter <= 2) {
			return true;
		}

		Transform temp = transform.GetChild(growthCounter);
		if(temp.name.Length >= 4) {
			if(temp.name.Substring(0, 4) == "Leaf") {
				temp.GetComponent<Leaf>().processGrowthCycle(branch, leaf, bud);
			}
		}

		return false;
	}

	/*
	 * Wrapper for growing new leaf and branch buds on the branch
	 */
	void growBuds() {
		if(!hasDisease) {
			growBranchBuds(bud);
			growLeafBuds(bud);
		}
	}

	/*
	 * Grows new leaf buds on the surface of the branch
	 */
	void growLeafBuds(GameObject bud) {
		float tipPoint = transform.GetChild(1).localPosition.y;

		if(numLeaves < leafMax && age % leafGrowthCycle == 0 && depth >= minLeafDepth) {
			int numBuds = Random.Range(leafMin, leafMax - numLeaves);

			Vector3[] leafPositions = new Vector3[numBuds + numLeaves];
			Quaternion[] leafRotations = new Quaternion[numBuds + numLeaves];

			//Find the rotations of existing branches
			Vector3[] existingPos = getLeafPositions();
			Quaternion[] existingRot = getLeafRotations();

			for(int i = 0; i < numLeaves; i++) {
				leafPositions[i] = existingPos[i];
				leafRotations[i] = existingRot[i];
			}

			//Attempt to create all of the leaf buds
			for(int i = 0; i < numBuds; i++) {
				Vector3 spawnPos = Vector3.zero;
				Quaternion spawnRot = Quaternion.identity;
				int attempts = 0;
				bool foundPos = false;

				float yPos = Random.Range(0, tipPoint + leafRange);

				while(!foundPos && attempts < maxPlacementAttempts) {
					spawnPos = Vector3.zero;    //reset spawn pos

					if(yPos > tipPoint) { //Must place leaves on the sphere surface
						if(yPos == leafRange + tipPoint) { //On the tip of the branch
							spawnPos = new Vector3(0, yPos, 0);
							spawnRot = Quaternion.LookRotation(new Vector3(0, tipPoint, 0) - spawnPos);

							foundPos = true;
							for(int j = 0; j < i + numLeaves; j++) {
								if(leafPositions[j] == spawnPos) {
									foundPos = false;
									yPos = Random.Range(0, tipPoint + leafRange);
									attempts++;
								}
							}
						}
						else {  //on the spherical surface
							float yOffset = yPos - transform.GetChild(1).localPosition.y;
							float xRange = Mathf.Sqrt(leafRange * leafRange - yOffset * yOffset);
							float xPos = Random.Range(-xRange, xRange);
							float zPos = Mathf.Sqrt(leafRange * leafRange - xPos * xPos - yOffset * yOffset);

							spawnPos.x = xPos;
							spawnPos.y = yPos;
							spawnPos.z = zPos;

							spawnRot = Quaternion.LookRotation(new Vector3(0, tipPoint, 0) - spawnPos);

							foundPos = true;
							for(int j = 0; j < i + numLeaves; j++) {
								if(Quaternion.Angle(leafRotations[j], spawnRot) <= leafOffset) {
									foundPos = false;
									attempts++;
								}
							}
						}
					}
					else {  //Must place leaves around the cylinder
						float xPos = Random.Range(-leafRange, leafRange);
						float zOffset = Mathf.Sqrt(leafRange * leafRange - xPos * xPos);
						float zPos = Random.Range(0, 2) == 0 ? -zOffset : zOffset;

						spawnPos.x = xPos;
						spawnPos.y = yPos;
						spawnPos.z = zPos;

						spawnRot = Quaternion.LookRotation(new Vector3(0, yPos, 0) - spawnPos);

						foundPos = true;
						for(int j = 0; j < i + numLeaves; j++) {
							if(Quaternion.Angle(leafRotations[j], spawnRot) <= leafOffset) {
								foundPos = false;
								attempts++;
							}
						}
					}
				}

				if(attempts >= maxPlacementAttempts) {
					Debug.Log("Failed to find a spot for new leaves");
					break;
				}

				//Update pos and rot
				leafPositions[i + numLeaves] = spawnPos;
				leafRotations[i + numLeaves] = spawnRot;

				//Add the bud at the found position and rotation
				addBud(bud, spawnPos, spawnRot, true);
			}
		}
	}

	/*
	 * Grows new branch buds on the surface of the tip of the branch
	 */
	void growBranchBuds(GameObject bud) {
		if(numBranches < branchMax && age % branchGrowthCycle == 1) {
			//Note that Range is exculsive for the max so the branchMax must be increased by 1
			int numBuds = numBranches > 0 ? Random.Range(0, branchMax + 1 - numBranches) : Random.Range(branchMin, branchMax + 1);

			Quaternion[] branchRotations = new Quaternion[numBuds + numBranches];

			//Find the positions and rotations of existing branches
			Quaternion[] existingRot = getBranchRotations();

			for(int i = 0; i < numBranches; i++) {
				branchRotations[i] = existingRot[i];
			}

			//Attempt to create all of the branch buds
			for(int i = 0; i < numBuds; i++) {
				Vector3 originPos = transform.GetChild(1).localPosition;
				Vector3 spawnPos = originPos;
				Quaternion spawnRot = Quaternion.identity;

				bool foundPos = false;
				float xOffset, zRange, zOffset;
				int attempts = 0;

				//Attempt to find a position to place the branch bud on the tip of
				//this branch within the allowed attempts limit
				while(!foundPos && attempts < maxPlacementAttempts) {
					spawnPos = originPos;   //Reset the spawn pos to origin

					//Find a Vector3 position on the surface of the rounded tip of the branch
					xOffset = Random.Range(-budRange, budRange);
					zRange = Mathf.Sqrt(budRange * budRange - xOffset * xOffset);
					zOffset = Random.Range(-zRange, zRange);
					spawnPos.x += xOffset;
					spawnPos.z += zOffset;
					spawnPos.y += Mathf.Sqrt(budRange * budRange - zOffset * zOffset - xOffset * xOffset);

					//Create a rotation that points towards the center of the sphere tip from the spawn pos
					spawnRot = Quaternion.LookRotation(transform.GetChild(1).localPosition - spawnPos);

					foundPos = true;
					for(int j = 0; j < i + numBranches; j++) {
						if(Quaternion.Angle(branchRotations[j], spawnRot) <= budOffset) {
							foundPos = false;
							attempts++;
						}
					}
				}

				if(attempts >= maxPlacementAttempts) {
					Debug.Log("Failed to find a spot for new branches");
					break;
				}

				//Update pos and rot
				branchRotations[i + numBranches] = spawnRot;

				//Add the bud at the found position and rotation
				addBud(bud, spawnPos, spawnRot, false);
			}
		}
	}

	/*
	 * Finds the Rotations of all existing branches
	 */
	Quaternion[] getBranchRotations() {
		Quaternion[] q = new Quaternion[numBranches];
		int c = 0;

		for(int i = 3; i < transform.childCount; i++) {
			if(transform.GetChild(i).name.Substring(0, 6) == "Branch") {
				q[c] = transform.GetChild(i).localRotation;
				c++;
			}
		}

		return q;
	}

	/*
	 * Finds the Positions of all existing leaves
	 */
	Vector3[] getLeafPositions() {
		Vector3[] q = new Vector3[numLeaves];
		int c = 0;

		for(int i = 3; i < transform.childCount; i++) {
			if(transform.GetChild(i).name.Substring(0, 4) == "Leaf") {
				q[c] = transform.GetChild(i).localPosition;
				c++;
			}
		}

		return q;
	}

	/*
	 * Finds the Rotations of all exiting Leaves
	 */
	Quaternion[] getLeafRotations() {
		Quaternion[] q = new Quaternion[numLeaves];
		int c = 0;

		for(int i = 3; i < transform.childCount; i++) {
			if(transform.GetChild(i).name.Substring(0, 4) == "Leaf") {
				q[c] = transform.GetChild(i).localRotation;
				c++;
			}
		}

		return q;
	}

	/*
	 * Adds a new bud of the given type at the given position and rotation
	 */
	void addBud(GameObject bud, Vector3 pos, Quaternion rot, bool isLeaf) {
		GameObject newBud = Instantiate(bud, Vector3.zero, Quaternion.identity) as GameObject;
		newBud.transform.parent = transform;
		newBud.transform.localPosition = pos;
		newBud.transform.localRotation = rot;
		newBud.transform.GetComponent<Bud>().setisLeaf(isLeaf);
		newBud.transform.GetComponent<Bud>().setDepth(depth + 1);
		newBud.transform.GetComponent<Bud>().setWPosition(Mathf.Clamp(w + Random.Range(-1, 2), 0, 6));   //the w value is clamped between 0 and 6 inclusive
		newBud.transform.GetComponent<Bud>().setManager(manager);
	}

	/*
	 * Checks if this branch will become diseased or 
	 * spread the disease if already diseased
	 */
	void checkForDisease() {
		if(!hasDisease) {
			setHasDisease(checkIfAllLeavesAreDead() || checkIfParentIsDiseased());
		}
		else {
			spreadDisease();
		}
	}

	/*
	 * Checks all child leaves to see if they are dead
	 * if all child leaves are dead then this branch will become diseased
	 */
	bool checkIfAllLeavesAreDead() {
		if(numLeaves == 0)
			return false;

		bool allDead = true;

		//Check all child leaves
		for(int i = transform.childCount - 1; i > 2; i--) {
			if(transform.GetChild(i).name.Length >= 4) {
				if(transform.GetChild(i).name.Substring(0, 4) == "Leaf") {
					if(transform.GetChild(i).GetComponent<Leaf>().getIsAlive()) {
						allDead = false;
					}
				}
			}
		}

		return allDead;
	}

	/*
	 * Checks if the parent branch is diseased
	 */
	bool checkIfParentIsDiseased() {
		if(depth > 0) {
			Branch bm = transform.parent.GetComponent<Branch>();
			if(bm.hasDisease) {
				if(bm.age >= bm.deathTime) {
					return true;
				}
			}
		}
		return false;
	}

	/*
	 * Modifies this branch to move into the diseased or health state from its previous state
	 */
	void setHasDisease(bool newHasDisease) {
		if(!hasDisease && newHasDisease) {  //The branch will be changed to diseased
			deathTime = age + 1;    //We add 1 because the tree is aged at the end of the growth process

			//Darken Branch
			Color old = transform.GetChild(0).GetChild(2).GetComponent<MeshRenderer>().material.color;
			setVisualColor(new Color(old.r * darkVal, old.g * darkVal, old.b * darkVal, darkAlpha));
		}
		else if(hasDisease && !newHasDisease) { //The branch will be changed to healthy
			setWPosition(w);
		}

		hasDisease = newHasDisease;
	}

	/*
	 * Spreads the disease to all children and the branch's parent
	 */
	void spreadDisease() {
		//Spread disease to children
		for(int i = transform.childCount - 1; i > 2; i--) {
			/*
			if(transform.GetChild(i).name.Length >= 6) {
				if(transform.GetChild(i).name.Substring(0, 6) == "Branch") {
					transform.GetChild(i).GetComponent<Branch>().setHasDisease(true);
				}
			}
			else*/
			if(transform.GetChild(i).name.Length >= 4) {
				if(transform.GetChild(i).name.Substring(0, 4) == "Leaf") {
					transform.GetChild(i).GetComponent<Leaf>().setIsAlive(false);
				}
			}
			else if(transform.GetChild(i).name.Length >= 3) {
				if(transform.GetChild(i).name.Substring(0, 3) == "Bud") {
					Destroy(transform.GetChild(i).gameObject);
				}
			}
		}

		//Spread disease to parent
		if(depth > 1)
			transform.parent.GetComponent<Branch>().setHasDisease(true);
	}

	/*
	 * Increment the number of leaves on this branch
	 */
	public void registerLeafAdded() {
		numLeaves++;
	}

	/*
	 * Decrement the number of leaves on this branch
	 */
	public void registerLeafRemoved() {
		numLeaves--;
	}

	/*
	 * Increment the number of branches on this branch
	 */
	public void registerBranchAdded() {
		numBranches++;

		isTip = false;
	}

	/*
	 * Decrement the number of branches on this branch
	 */
	public void registerBranchRemoved() {
		numBranches--;

		if(numBranches == 0) {
			isTip = true;
		}
	}

	/*
	 * Sets whether this branch can be snipped
	 */
	public void setcanSnip(bool canSnip) {
		this.canSnip = canSnip;
	}

	/*
	 * Sets the depth of this branch and adjust the growth cycles for its children
	 */
	public void setDepth(int newDepth) {
		depth = newDepth;
		leafGrowthCycle = 2 + Mathf.Max(0, 6 - depth);
		branchGrowthCycle = 3 + Mathf.Max(0, 8 - depth);
	}

	/*
	 * Sets the w position of this branch and adjusts the color accordingly
	 */
	public void setWPosition(int newW) {
		w = newW;

		//Change Material value
		switch(w) {
			case 0:     //red
				setVisualColor(new Color(1.0f, 0.0f, 0.0f, 0.5f));
				break;
			case 1:     //orange
				setVisualColor(new Color(1.0f, 0.5f, 0.0f, 0.5f));
				break;
			case 2:     //yellow
				setVisualColor(new Color(1.0f, 1.0f, 0.0f, 0.5f));
				break;
			case 3:     //green
				setVisualColor(new Color(0.0f, 1.0f, 0.0f, 0.5f));
				break;
			case 4:     //blue
				setVisualColor(new Color(0.0f, 1.0f, 1.0f, 0.5f));
				break;
			case 5:     //indigo
				setVisualColor(new Color(0.0f, 0.0f, 1.0f, 0.5f));
				break;
			case 6:     //violet
				setVisualColor(new Color(1.0f, 0.0f, 1.0f, 0.5f));
				break;
		}
	}

	/*
	 * Changes the material color of the visual components of this branch
	 */
	void setVisualColor(Color c) {
		transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.color = c;
		transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material.color = c;
		transform.GetChild(0).GetChild(2).GetComponent<MeshRenderer>().material.color = c;
	}

	/*
	 * Sets the bonsai manager this branch answers to
	 */
	public void setManager(GameObject newManager) {
		manager = newManager;
	}

	public bool getIsTip() {
		return isTip;
	}

	public bool getIsHigherThanChildren() {
		if(numBranches == 0) {
			return true;
		}
		else {
			for(int i = transform.childCount - 1; i > 2; i--) {
				if(transform.GetChild(i).name.Substring(0, 6) == "Branch") {
					if(transform.GetChild(i).GetChild(1).position.y > transform.GetChild(1).position.y) {
						return false;
					}
				}
			}
		}
		return true;
	}
}
