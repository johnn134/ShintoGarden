using UnityEngine;
using System.Collections;

public class Bud4D : MonoBehaviour {

	GameObject manager;

	int depth = 0;
	int w;

	bool isLeaf;        //Tells whether this bud will grow into a leaf or branch

	static int ID = 0;

	// Use this for initialization
	void Start() {
		//name the bud
		this.gameObject.name = "Bud_" + ID;
		ID++;
	}

	// Update is called once per frame
	void Update() {

	}

	/*
	 * Grows this bud into either a branch or leaf
	 */
	public void processGrowthCycle(GameObject branch, GameObject leaf, GameObject bud) {
		if(isLeaf) {
			GameObject newLeaf = Instantiate(leaf, Vector3.zero, Quaternion.identity, transform.parent) as GameObject;
			newLeaf.transform.localPosition = transform.localPosition;
			newLeaf.transform.localRotation = transform.localRotation;
			newLeaf.transform.Rotate(-90, 0, 0);
			newLeaf.transform.GetComponent<Leaf4D>().setDepth(depth);
			newLeaf.transform.GetComponent<Leaf4D>().setWPosition(w);
			newLeaf.transform.GetComponent<Leaf4D>().setManager(manager);

			transform.parent.GetComponent<Branch4D>().registerLeafAdded();
			manager.GetComponent<BonsaiManager4D>().addLeaf();
			Destroy(this.gameObject);
		}
		else {
			GameObject newBranch = Instantiate(branch, Vector3.zero, Quaternion.identity, transform.parent) as GameObject;
			newBranch.transform.localPosition = transform.localPosition;
			newBranch.transform.localRotation = transform.localRotation;
			newBranch.transform.Rotate(-90, 0, 0);
			newBranch.transform.GetComponent<Branch4D>().setDepth(depth);
			newBranch.transform.GetComponent<Branch4D>().setWPosition(w);
			newBranch.transform.GetComponent<Branch4D>().setManager(manager);

			transform.parent.GetComponent<Branch4D>().registerBranchAdded();
			manager.GetComponent<BonsaiManager4D>().addBranch();
			Destroy(this.gameObject);
		}
	}

	/*
	 * Sets the type of this bud
	 */
	public void setisLeaf(bool isLeaf) {
		this.isLeaf = isLeaf;
	}

	/*
	 * Sets the depth of this bud on the tree
	 */
	public void setDepth(int newDepth) {
		depth = newDepth;
	}

	/*
	 * Sets the w position of this bud and adjusts the color accordingly
	 */
	public void setWPosition(int newW) {
		w = newW;

		this.transform.GetChild(0).GetComponent<HyperObject>().updateMaterialShaderValues(newW);
	}

	/*
	 * Sets the bonsai manager this bud answers to
	 */
	public void setManager(GameObject newManager) {
		manager = newManager;
	}
}
