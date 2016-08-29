using UnityEngine;
using System.Collections;

public class Bud : MonoBehaviour {

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
	public void processGrowthCycle() {
		if(isLeaf) {
			GameObject newLeaf = Instantiate(Resources.Load("Bonsai/LeafPrefab"), Vector3.zero, Quaternion.identity, transform.parent) as GameObject;
			newLeaf.transform.localPosition = transform.localPosition;
			newLeaf.transform.localRotation = transform.localRotation;
			newLeaf.transform.Rotate(-90, 0, 0);
			newLeaf.transform.GetComponent<Leaf>().setDepth(depth);

			//newLeaf.transform.GetComponent<Leaf>().setWPosition(w);
			newLeaf.GetComponent<HyperColliderManager>().setW(transform.GetChild(0).GetComponent<HyperObject>().w);
			newLeaf.GetComponent<HyperColliderManager>().WMove(GameObject.FindGameObjectWithTag("Player").GetComponent<HyperCreature>().w);

			newLeaf.transform.GetComponent<Leaf>().setManager(manager);
			transform.parent.GetComponent<Branch>().registerLeafAdded();
			manager.GetComponent<BonsaiManager>().addLeaf();
			Destroy(this.gameObject);
		}
		else {
			GameObject newBranch = Instantiate(Resources.Load("Bonsai/BranchPrefab"), Vector3.zero, Quaternion.identity, transform.parent) as GameObject;
			newBranch.transform.localPosition = transform.localPosition;
			newBranch.transform.localRotation = transform.localRotation;
			newBranch.transform.Rotate(-90, 0, 0);
			newBranch.transform.GetComponent<Branch>().setDepth(depth);

			//newBranch.transform.GetComponent<Branch>().setWPosition(w);
			newBranch.GetComponent<HyperColliderManager>().setW(transform.GetChild(0).GetComponent<HyperObject>().w);
			newBranch.GetComponent<HyperColliderManager>().WMove(GameObject.FindGameObjectWithTag("Player").GetComponent<HyperCreature>().w);

			newBranch.transform.GetComponent<Branch>().setManager(manager);
			transform.parent.GetComponent<Branch>().registerBranchAdded();
			manager.GetComponent<BonsaiManager>().addBranch();
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
	 * Changes the material color of the visual components of this bud
	 */
	void setVisualColor(Color c) {
		transform.GetChild(0).GetComponent<MeshRenderer>().material.color = c;
	}

	/*
	 * Sets the bonsai manager this bud answers to
	 */
	public void setManager(GameObject newManager) {
		manager = newManager;
	}
}
