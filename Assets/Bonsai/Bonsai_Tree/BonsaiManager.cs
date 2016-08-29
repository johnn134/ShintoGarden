using UnityEngine;
using System.Collections;

public class BonsaiManager : MonoBehaviour {

    public GameObject branch;
    public GameObject bud;
    public GameObject leaf;

	public int maxLeaves = 30;
	public int maxBranches = 30;

	public float growthCycleTime = 60.0f;

    GameObject baseBranch;

	int numLeaves;
	int numBranches;
	int numDeadLeaves;
	int numDeadBranches;
	int numInfestedBranches;

	static int ID = 0;

	// Use this for initialization
	void Start () {
		//initialize variables
		numLeaves = 0;
		numBranches = 1;	//1 for the base branch
		numDeadLeaves = 0;
		numDeadBranches = 0;
		numInfestedBranches = 0;

		//Name the tree
		this.gameObject.name = "BonsaiTree_" + ID;
		ID++;

		//Create the base branch
		baseBranch = Instantiate(Resources.Load("Bonsai/BranchPrefab"), transform) as GameObject;
		baseBranch.transform.localPosition = Vector3.zero;
		baseBranch.GetComponent<Branch>().setcanSnip(false);
		baseBranch.GetComponent<Branch>().setDepth(0);

		//baseBranch.GetComponent<Branch>().setWPosition(3);
		baseBranch.GetComponent<HyperColliderManager>().setW(3);
		baseBranch.GetComponent<HyperColliderManager>().WMove(GameObject.FindGameObjectWithTag("Player").GetComponent<HyperCreature>().w);

		baseBranch.GetComponent<Branch>().setManager(this.gameObject);
		InvokeRepeating("processGrowthCycle", growthCycleTime, growthCycleTime);
	}
	
	// Update is called once per frame
	void Update () {
		//Testing code for growing the tree with the spacebar
		if(Input.GetKeyDown(KeyCode.Space)) {
			processGrowthCycle();
		}

		/*
		//Grow tree on a timer
		if(Time.time % growthCycleTime == 0) {
			processGrowthCycle();
		}
		*/
	}

	void processGrowthCycle() {
		baseBranch.GetComponent<Branch>().processGrowthCycle();
	}

	public void addLeaf() {
		numLeaves++;
	}

	public void removeLeaf() {
		numLeaves--;
	}

	public void addBranch() {
		numBranches++;
	}

	public void removeBranch() {
		numBranches--;
	}

	public void addDeadLeaf() {
		numDeadLeaves++;
	}

	public void removeDeadLeaf() {
		numDeadLeaves--;
	}

	public void addDeadBranch() {
		numDeadBranches++;
	}

	public void removeDeadBranch() {
		numDeadBranches--;
	}

	public void addInfestedBranch() {
		numInfestedBranches++;
	}

	public void removeInfestedBranch() {
		numInfestedBranches--;
	}

	public bool canMakeLeaf() {
		return numLeaves < maxLeaves;
	}

	public bool canMakeBranch() {
		return numBranches < maxBranches;
	}

	public int getNumLeaves() {
		return numLeaves;
	}

	public int getNumBranches() {
		return numBranches;
	}

	public int getNumInfestedBranches() {
		return numInfestedBranches;
	}

	public int getNumDeadLeaves() {
		return numDeadLeaves;
	}

	public int getNumDeadBranches() {
		return numDeadBranches;
	}
}
