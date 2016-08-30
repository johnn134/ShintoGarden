using UnityEngine;
using System.Collections;

public class Branch : MonoBehaviour {

	GameObject manager; //Reference to the BonsaiManager

	int age;            	//How many growth cycles has this branch lived through
	int numLeaves;      	//Number of leaves on this branch
	int numBranches;    	//Number of branches on this branch
	int numBugs;
	int growthStep;     	//Marks which step of the growth order is in effect
	int growthCounter;  	//Counter for iterating through branch children
	int leavesDeathTime;	//The age at which all of this branch's leaves died
	int infestationTime;	//The age at which this branch was infested with bugs
	int deathTime;			//The age at which this branch died

	int branchGrowthCycle = 3;      //Must be at least 1, number of growth cycles between growing new branches
	int leafGrowthCycle = 3;        //Must be at least 1, number of growth cycles between growing new leaves
	int depth = 0;              //The depth of this branch in the tree
	int w = 0;              	//Position on the fourth dimension

	float budRange;     	//Maximum distance from the center of the branch tip to grow a bud
	float leafRange;    	//Stores the radius of the rounded branch tip
	bool runGrowth;     	//Tells whether the branch should be growing
	bool isTip;         	//True if this branch has no child branches
	bool isDead;    		//Is this branch diseased
	bool isInfested;    	//Does ths branch have bugs on it
	bool leavesAreDead;		//Are all leaves on this branch dead

	bool canSnip = true;       	//Tells whether this branch can be snipped off

	const int BRANCH_MIN = 1;              	//Minimum number of branch buds that will initially grow
	const int BRANCH_MAX = 3;              	//Maximum number of branch buds that can ever grow from this branch
	const int LEAF_MIN = 0;                	//Minimum number of leaf buds that can grow
	const int LEAF_MAX = 5;                	//Maximum number of leaf buds that can ever grow
	const int BUG_MIN = 3;					//Minimum number of bugs that can infest the branch
	const int BUG_MAX = 5;					//Maximum number of bugs that can infest the branch
	const int MIN_LEAF_DEPTH = 3;			//Must be at least 0, depth of branches before leaves can grow
	const int MAX_PLACEMENT_ATTEMPTS = 20;  //Maximum attempts for a bud to place itself before giving up
	const int TIME_TILL_INFESTATION = 1;	//Must be at least 0, number of growth cycles after leaves have died till infestation
	const int TIME_TILL_DEATH = 2;			//Must be at least 0, number of growth cycles after infestation until death
	const int INFESTATION_COOLDOWN = 2;
	const int INFESTATION_LIFETIME = TIME_TILL_DEATH + 1;	//Must be at least equal to time_till_death, 
															//number of cycles between infestation start and finish

	const float BUD_OFFSET = 60.0f;			//Minimum angle between new branch buds
	const float LEAF_OFFSET = 45.0f;		//Minimum angle between new leaf buds
	const float DARKEN_VALUE = 0.1f;		//Value multiplied to the material color on death
	const float DARKEN_ALPHA = 0.75f;		//Value of material color alpha on death

	static int ID = 0;	//Unique branch identifier

	// Use this for initialization
	void Start() {
		//Initialize variables
		age = 0;
		numLeaves = 0;
		numBranches = 0;
		growthStep = 0;
		numBugs = 0;

		runGrowth = false;
		isTip = true;

		//Infestation and Death variables
		leavesDeathTime = -1;
		infestationTime = -1;
		deathTime = -1;
		isDead = false;
		isInfested = false;
		leavesAreDead = false;

		budRange = transform.GetChild(0).GetChild(2).localScale.x / 2.0f * 0.9f;
		leafRange = transform.GetChild(0).GetChild(2).localScale.x / 2.0f;

		//Name the branch
		this.gameObject.name = "Branch_" + ID;
		ID++;
	}

	void OnDestroy() {
		if(transform.parent != null) {
			if(transform.parent.GetComponent<Branch>() != null) {
				transform.parent.GetComponent<Branch>().registerBranchRemoved();
			}
		}
		if(manager != null) {
			manager.GetComponent<BonsaiManager>().removeBranch();

			if(isInfested)
				manager.GetComponent<BonsaiManager>().removeInfestedBranch();
			
			if(isDead)
				manager.GetComponent<BonsaiManager>().removeDeadBranch();
		}
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

				//Growth order: check infestation stage, age leaves, evolve buds, grow new buds, process next branch
				switch(growthStep) {
					case 0:
						processInfestation();
						nextStep = true;
						break;
					case 1:
						processLeaves();
						nextStep = true;
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
			Destroy(this.gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.transform.parent != null) {
			if(other.transform.parent.GetComponent<Insecticide>() != null) {	//Insecticide Spray Collision
				if(isInfested) {
					Debug.Log("Spray has removed the infestation");
					removeInfestation();
				}
			}
			else if(other.gameObject.name.Equals("ShearZone")) {
				if(canSnip) {
					Destroy(this.gameObject);
				}
			}
		}
	}

	/*
	 * Initiates the growth cycle for this branch and children parts
	 */
	public void processGrowthCycle() {
		if(!runGrowth) {
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
				temp.GetComponent<Branch>().processGrowthCycle();
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

		if(transform.GetChild(growthCounter).GetComponent<Bud>() != null) {
			transform.GetChild(growthCounter).GetComponent<Bud>().processGrowthCycle();
		}

		return false;
	}

	/*
	 * Calls the processGrowthCycle on the currently focused leaf
	 */
	void processLeaves() {
		for(int i = transform.childCount - 1; i > 2; i--) {
			if(transform.GetChild(i).GetComponent<Leaf>() != null) {
				transform.GetChild(i).GetComponent<Leaf>().processGrowthCycle();
			}
		}
	}

	/*
	 * Wrapper for growing new leaf and branch buds on the branch
	 */
	void growBuds() {
		if(!isDead) {
			if(manager.GetComponent<BonsaiManager>().getNumBranches() < manager.GetComponent<BonsaiManager>().maxBranches) {
				growBranchBuds();
			}
			if(manager.GetComponent<BonsaiManager>().getNumLeaves() < manager.GetComponent<BonsaiManager>().maxLeaves) {
				growLeafBuds();
			}
		}
	}

	/*
	 * Grows new leaf buds on the surface of the branch
	 */
	void growLeafBuds() {
		float tipPoint = transform.GetChild(1).localPosition.y;

		if(numLeaves < LEAF_MAX && age % leafGrowthCycle == 0 && depth >= MIN_LEAF_DEPTH) {
			int numBuds = Random.Range(LEAF_MIN, LEAF_MAX - numLeaves);

			if(manager.GetComponent<BonsaiManager>().getNumLeaves() + numBuds > manager.GetComponent<BonsaiManager>().maxLeaves) {
				numBuds = manager.GetComponent<BonsaiManager>().maxLeaves - manager.GetComponent<BonsaiManager>().getNumLeaves();
			}

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

				while(!foundPos && attempts < MAX_PLACEMENT_ATTEMPTS) {
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
								if(Quaternion.Angle(leafRotations[j], spawnRot) <= LEAF_OFFSET) {
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
							if(Quaternion.Angle(leafRotations[j], spawnRot) <= LEAF_OFFSET) {
								foundPos = false;
								attempts++;
							}
						}
					}
				}

				if(attempts >= MAX_PLACEMENT_ATTEMPTS) {
					Debug.Log("Failed to find a spot for new leaves");
					break;
				}

				//Update pos and rot
				leafPositions[i + numLeaves] = spawnPos;
				leafRotations[i + numLeaves] = spawnRot;

				//Add the bud at the found position and rotation
				addBud(spawnPos, spawnRot, true);
			}
		}
	}

	/*
	 * Grows new branch buds on the surface of the tip of the branch
	 */
	void growBranchBuds() {
		if(numBranches < BRANCH_MAX && age % branchGrowthCycle == 1) {
			//Note that Range is exculsive for the max so the branchMax must be increased by 1
			int numBuds = numBranches > 0 ? Random.Range(0, BRANCH_MAX + 1 - numBranches) : Random.Range(BRANCH_MIN, BRANCH_MAX + 1);

			if(manager.GetComponent<BonsaiManager>().getNumBranches() + numBuds > manager.GetComponent<BonsaiManager>().maxBranches) {
				numBuds = manager.GetComponent<BonsaiManager>().maxBranches - manager.GetComponent<BonsaiManager>().getNumBranches();
			}

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
				while(!foundPos && attempts < MAX_PLACEMENT_ATTEMPTS) {
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
						if(Quaternion.Angle(branchRotations[j], spawnRot) <= BUD_OFFSET) {
							foundPos = false;
							attempts++;
						}
					}
				}

				if(attempts >= MAX_PLACEMENT_ATTEMPTS) {
					Debug.Log("Failed to find a spot for new branches");
					break;
				}

				//Update pos and rot
				branchRotations[i + numBranches] = spawnRot;

				//Add the bud at the found position and rotation
				addBud(spawnPos, spawnRot, false);
			}
		}
	}

	/*
	 * Adds a new bud of the given type at the given position and rotation
	 */
	void addBud(Vector3 pos, Quaternion rot, bool isLeaf) {
		//Instantiate bud
		GameObject newBud = Instantiate(Resources.Load("Bonsai/BudPrefab"), Vector3.zero, Quaternion.identity, transform) as GameObject;	

		//Initialize new bud transform
		newBud.transform.localPosition = pos;
		newBud.transform.localRotation = rot;

		//Initialize new bud variables
		newBud.transform.GetComponent<Bud>().setisLeaf(isLeaf);
		newBud.transform.GetComponent<Bud>().setDepth(depth + 1);

		//newBud.transform.GetComponent<Bud>().setWPosition(Mathf.Clamp(w + Random.Range(-1, 2), 0, 6));   //the w value is clamped between 0 and 6 inclusive
		newBud.transform.GetChild(0).GetComponent<HyperObject>().setW(Mathf.Clamp(GetComponent<HyperColliderManager>().w + Random.Range(-1, 2), 0, 6));
		newBud.transform.GetChild(0).GetComponent<HyperObject>().WMove(GameObject.FindGameObjectWithTag("Player").GetComponent<HyperCreature>().w);

		newBud.transform.GetComponent<Bud>().setManager(manager);
		if(isLeaf)
			manager.GetComponent<BonsaiManager>().addLeaf();
		else
			manager.GetComponent<BonsaiManager>().addBranch();
	}

	/*
	 * Returns an array of branch Gameobjects that are children of this branch
	 */
	GameObject[] getBranchChildren() {
		GameObject[] branchChildren = new GameObject[numBranches];
		int counter = 0;
		foreach (Transform child in transform) {
			if(child.GetComponent<Branch>() != null) {
				branchChildren [counter] = child.gameObject;
				counter++;
			}
		}
		return branchChildren;
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
	 * Returns an array of leaf Gameobjects that are children of this branch
	 */
	GameObject[] getLeafChildren() {
		GameObject[] leafChildren = new GameObject[numLeaves];
		int counter = 0;
		foreach (Transform child in transform) {
			if(child.GetComponent<Leaf>() != null) {
				leafChildren [counter] = child.gameObject;
				counter++;
			}
		}
		return leafChildren;
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
	 * Determines the correct action to take for the current stage of infestation
	 */
	void processInfestation() {
		if(!isDead && !isInfested) {	//Branch is alive and clean
			//Check if all of the branches leaves have died
			bool tempLAD = checkIfAllLeavesAreDead();

			if(!tempLAD && leavesAreDead) {	//leaves were dead but a new leaf grew
				leavesAreDead = false;
				leavesDeathTime = -1;
			}
			else if(tempLAD && !leavesAreDead) {	//leaves have just died
				leavesAreDead = true;
				leavesDeathTime = age;
			}
			else if(tempLAD && leavesAreDead) {	//leaves have been dead
				if(age >= leavesDeathTime + TIME_TILL_INFESTATION) {	//Time to create infestation
					addInfestationToBranch();
				}
			}

			/*** if leaves are alive and they weren't dead then do nothing ***/
		}
		else if(isInfested) {
			//Check for infestation effects
			if(!isDead) {	//is alive and infested
				if(age >= infestationTime + TIME_TILL_DEATH) {	//Time to kill branch
					addDeathToBranch();
				}
			}

			//Check for removing infestation
			if(age >= infestationTime + INFESTATION_LIFETIME) {
				removeInfestation();
			}
			else if(age > infestationTime) {
				spreadInfestation();
			}
		}

		/*** Branch is dead and clean so nothing happens ***/
	}

	/*
	 * Returns true if all child leaves are dead, false if any are alive
	 */
	bool checkIfAllLeavesAreDead() {
		if(numLeaves == 0)
			return false;

		bool allDead = true;

		//Check all child leaves
		for(int i = transform.childCount - 1; i > 2; i--) {
			if(transform.GetChild(i).GetComponent<Leaf>() != null) {
				if(!transform.GetChild(i).GetComponent<Leaf>().getIsDead()) {
					allDead = false;
				}
			}
		}

		return allDead;
	}

	/*
	 * Adds the infestation effect to this branch
	 */
	void addInfestationToBranch() {
		if(!isInfested && age > deathTime + INFESTATION_LIFETIME - TIME_TILL_DEATH + INFESTATION_COOLDOWN) {
			isInfested = true;
			infestationTime = age;

			if(manager != null)
				manager.GetComponent<BonsaiManager>().addInfestedBranch();
			
			addBugs();
		}
	}

	/*
	 * Determines the number of bugs to be added and their positions
	 */
	void addBugs() {
		int numBugs = Random.Range(BUG_MIN, BUG_MAX + 1);	//create a random number of bugs between min and max

		for(int i = 0; i < numBugs; i++) {
			//Find a point on the surface of the cylinder of the branch
			float newX = Random.Range(-leafRange, leafRange);
			float newZRange = Mathf.Sqrt(leafRange * leafRange - newX * newX);
			float newZ = Random.Range(-newZRange, newZRange);

			addBug(new Vector3(newX, 0, newZ));	//create new bug
		}
	}

	/*
	 * Create and attach bug to branch
	 */
	void addBug(Vector3 newPos) {
		GameObject newBug = Instantiate(Resources.Load("Bonsai/BugPrefab"), Vector3.zero, Quaternion.identity, transform) as GameObject;
		newBug.transform.localPosition = newPos;
		newBug.transform.localRotation = Quaternion.identity;

		//newBug.GetComponent<BonsaiBug>().setWPosition(w);
		newBug.GetComponent<HyperColliderManager>().setW(GetComponent<HyperColliderManager>().w);
		newBug.GetComponent<HyperColliderManager>().WMove(GameObject.FindGameObjectWithTag("Player").GetComponent<HyperCreature>().w);

		newBug.GetComponent<BonsaiBug>().setOrigin(newPos.x, transform.GetChild(1).localPosition.y / 2, newPos.z);
		newBug.GetComponent<BonsaiBug>().setMovementRange(transform.GetChild(1).localPosition.y / 2);

		numBugs++;
	}

	/*
	 * Removes the infestation and bugs from this branch
	 */
	void removeInfestation() {
		//Reset variables
		isInfested = false;
		infestationTime = -1;

		if(manager != null)
			manager.GetComponent<BonsaiManager>().removeInfestedBranch();

		//Destroy each bug
		GameObject[] bugs = new GameObject[numBugs];
		int counter = 0;

		for(int i = 0; i < transform.childCount; i++) {
			if(transform.GetChild(i).GetComponent<BonsaiBug>() != null) {
				bugs[counter] = transform.GetChild(i).gameObject;
				counter++;
			}
		}

		for(int i = 0; i < numBugs; i++) {
			bugs[i].transform.parent = null;
			bugs[i].GetComponent<BonsaiBug>().startDeath();
		}
	}

	/*
	 * Spreads this branch's infestation to adjacent branches
	 */
	void spreadInfestation() {
		//Spread to children
		foreach (GameObject g in getBranchChildren()) {
			g.GetComponent<Branch>().addInfestationToBranch();
		}

		//Spread to parent
		if(depth > 1)
			transform.parent.GetComponent<Branch>().addInfestationToBranch();
	}

	/*
	 * Adds the death effect to this branch
	 */
	void addDeathToBranch() {
		isDead = true;
		deathTime = age;

		if(manager != null)
			manager.GetComponent<BonsaiManager>().addDeadBranch();

		for(int i = transform.childCount - 1; i > 2; i--) {
			if(transform.GetChild(i).GetComponent<Leaf>() != null) {
				transform.GetChild(i).GetComponent<Leaf>().makeLeafDead();
			}
		}

		transform.GetChild(0).GetChild(0).GetComponent<HyperObject>().dullCoef = 4;
		transform.GetChild(0).GetChild(1).GetComponent<HyperObject>().dullCoef = 4;
		transform.GetChild(0).GetChild(2).GetComponent<HyperObject>().dullCoef = 4;
		transform.GetChild(0).GetChild(0).GetComponent<HyperObject>().WMove(GameObject.FindGameObjectWithTag("Player").GetComponent<HyperCreature>().w);
		transform.GetChild(0).GetChild(1).GetComponent<HyperObject>().WMove(GameObject.FindGameObjectWithTag("Player").GetComponent<HyperCreature>().w);
		transform.GetChild(0).GetChild(2).GetComponent<HyperObject>().WMove(GameObject.FindGameObjectWithTag("Player").GetComponent<HyperCreature>().w);
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

	/*
	 * Returns isTip
	 */
	public bool getIsTip() {
		return isTip;
	}

	/*
	 * Returns whether the base point of this branch has a greater y value 
	 * than the base point of its children branches
	 */
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
