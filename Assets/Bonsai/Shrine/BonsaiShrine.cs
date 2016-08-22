using UnityEngine;
using System.Collections;

public class BonsaiShrine : MonoBehaviour {

	int activationStage;
	int points;

	public int maxPoints = 0;

	bool activated;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void addPoints(int p) {
		points += p;
	}
}
