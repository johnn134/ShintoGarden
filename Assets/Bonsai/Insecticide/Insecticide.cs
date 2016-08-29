using UnityEngine;
using System.Collections;

public class Insecticide : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
		transform.GetChild(1).GetComponent<CapsuleCollider>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(1)) {
			startSpray();
		}
		if(Input.GetMouseButtonUp(1)) {
			endSpray();
		}
	}

	public void startSpray() {
		//Begin Particle Effect
		transform.GetChild(2).GetComponent<ParticleSystem>().Play();

		//Turn on collisions
		transform.GetChild(1).GetComponent<CapsuleCollider>().enabled = true;

		CancelInvoke();
		Invoke("endSpray", .2f);
	}

	public void endSpray() {
		//End Particle Effect
		transform.GetChild(2).GetComponent<ParticleSystem>().Stop();

		//Turn off collisions
		transform.GetChild(1).GetComponent<CapsuleCollider>().enabled = false;
	}
}
