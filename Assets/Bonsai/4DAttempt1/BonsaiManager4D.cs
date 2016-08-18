using UnityEngine;
using System.Collections;

public class BonsaiManager4D : MonoBehaviour {

	public GameObject branch;
	public GameObject stem;
	public GameObject bud;
	public GameObject leaf;

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
		baseBranch = Instantiate(branch, transform) as GameObject;
		baseBranch.transform.localPosition = Vector3.zero;
		baseBranch.GetComponent<Branch4D>().setcanSnip(false);
		baseBranch.GetComponent<Branch4D>().setDepth(0);
		baseBranch.GetComponent<Branch4D>().setWPosition(3);
		baseBranch.GetComponent<Branch4D>().setManager(this.gameObject);
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)) {
			processGrowthCycle(branch, leaf, bud);
		}
	}

	void processGrowthCycle(GameObject branch, GameObject leaf, GameObject bud) {
		baseBranch.GetComponent<Branch4D>().processGrowthCycle(branch, leaf, bud);
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
}
