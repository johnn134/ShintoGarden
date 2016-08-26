using UnityEngine;
using System.Collections;

public class BonsaiShrine : MonoBehaviour {

	public GameObject LevelOneLights;
	public GameObject LevelTwoLights;
	public GameObject LevelThreeLights;
	public GameObject LevelFourLights;

	public GameObject[] trees;

	int activationStage;

	int[] issues;

	float checkTime;

	bool activated;

	const int MIN_LEAVES = 4;
	const int MIN_BRANCHES = 5;

	const float CHECK_DELAY = 2.5f;
	const float INACTIVE_COLOR_VALUE = 0.1f;
	const float ACTIVE_COLOR_VALUE = 1.0f;

	// Use this for initialization
	void Start () {
		checkTime = 5.0f;

		issues = new int[trees.Length];
		for(int i = 0; i < issues.Length; i++) {
			issues[i] = 0;
		}

		setActivationStage(0);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate() {
		//run tree check
		if(Time.time > checkTime) {
			checkTrees();
			checkTime = Time.time + CHECK_DELAY;
		}
	}

	/*
	 * Determines the state of the bonsai trees in the garden
	 * A tree is in poor health if:
	 * - Dead leaves or branches
	 * - Infested
	 * - Overgrown
	 * 
	 * Requirements for stages:
	 * 0 - Every tree must have at most 3 issues
	 * 1 - Every tree must have at most 2 issues
	 * 2 - Every tree must have at most 1 issue
	 * 3 - All trees have no issues
	 * 4 - All trees meet the specific requirement of the contract
	 */
	void checkTrees() {
		bool contractSatisfied = true;

		for(int i = 0; i < trees.Length; i++) {
			GameObject g = trees[i];
			issues[i] = 0;

			if(g.GetComponent<BonsaiManager>() != null) {
				//Overgrown check
				if(g.GetComponent<BonsaiManager>().getNumLeaves() >= g.GetComponent<BonsaiManager>().maxLeaves ||
				   g.GetComponent<BonsaiManager>().getNumBranches() >= g.GetComponent<BonsaiManager>().maxBranches) {
					issues[i] += 1;
				}

				//Infestation check
				if(g.GetComponent<BonsaiManager>().getNumInfestedBranches() > 0) {
					issues[i] += 1;
				}

				//Dead leaves and branches check
				if(g.GetComponent<BonsaiManager>().getNumDeadLeaves() > 0 || 
				   g.GetComponent<BonsaiManager>().getNumDeadBranches() > 0) {
					issues[i] += 1;
				}
					
				//Undergrowth check
				if(g.GetComponent<BonsaiManager>().getNumLeaves() < MIN_LEAVES ||
				   g.GetComponent<BonsaiManager>().getNumBranches() < MIN_BRANCHES) {
					issues[i] = 3;	//undergrowth turns off the shrine
				}

				//contract requirement check
				contractSatisfied = false;	/*** Change this to work with the required contract ***/
			}
		}

		int stage = 4;

		for(int i = 0; i < issues.Length; i++) {
			int val = 3 - issues[i];
			val += contractSatisfied ? 1 : 0;

			stage = Mathf.Min(val, stage);
		}

		setActivationStage(stage);
	}

	void setActivationStage(int newStage) {
		activateLights(newStage);
	}

	/*
	 * Activates the lights up to the given level
	 * i.e. lightLevel 2 activates 1 and 2
	 */
	void activateLights(int lightLevel) {
		Color active = Color.white * ACTIVE_COLOR_VALUE;
		Color inactive = Color.white * INACTIVE_COLOR_VALUE;

		LevelOneLights.GetComponent<Renderer>().material.color = lightLevel >= 1 ? active : inactive;
		LevelTwoLights.GetComponent<Renderer>().material.color = lightLevel >= 2 ? active : inactive;
		LevelThreeLights.GetComponent<Renderer>().material.color = lightLevel >= 3 ? active : inactive;
		LevelFourLights.GetComponent<Renderer>().material.color = lightLevel >= 4 ? active : inactive;
	}
}
