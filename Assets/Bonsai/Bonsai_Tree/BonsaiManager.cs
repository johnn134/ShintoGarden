using UnityEngine;
using System.Collections;

public class BonsaiManager : MonoBehaviour {

    public GameObject branch;
    public GameObject bud;
    public GameObject leaf;

	public int maxLeaves = 30;
	public int maxBranches = 30;

    GameObject baseBranch;

	int numLeaves;
	int numBranches;

	static int ID = 0;

	// Use this for initialization
	void Start () {
		//Name the tree
		this.gameObject.name = "BonsaiTree_" + ID;
		ID++;

		//Create the base branch
		baseBranch = Instantiate(Resources.Load("Bonsai/BranchPrefab"), transform) as GameObject;
		baseBranch.transform.localPosition = Vector3.zero;
		baseBranch.GetComponent<Branch>().setcanSnip(false);
		baseBranch.GetComponent<Branch>().setDepth(0);
		baseBranch.GetComponent<Branch>().setWPosition(3);
		baseBranch.GetComponent<Branch>().setManager(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		//Testing code for growing the tree with the spacebar
		if(Input.GetKeyDown(KeyCode.Space)) {
			processGrowthCycle();
		}
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

	public bool canMakeLeaf() {
		return numLeaves < maxLeaves;
	}

	public bool canMakeBranch() {
		return numBranches < maxBranches;
	}
}
