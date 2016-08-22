﻿using UnityEngine;
using System.Collections;

public class Shears : MonoBehaviour {

	bool startedSnip;

	// Use this for initialization
	void Start () {
		startedSnip = false;

		transform.GetChild(1).GetComponent<SphereCollider>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		//Check for turning off collisions for snipping
		if(startedSnip) {
			transform.GetChild(1).GetComponent<SphereCollider>().enabled = false;
			startedSnip = false;
		}

		//Check for input
		if(Input.GetMouseButtonDown(0)) {
			snip();
		}
	}

	void snip() {
		transform.GetChild(1).GetComponent<SphereCollider>().enabled = true;
		startedSnip = true;
	}
}