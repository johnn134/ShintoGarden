using UnityEngine;
using System.Collections;

public class FourthDimensionController : MonoBehaviour {

	public int userWPosition = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.UpArrow)) {
			Debug.Log("Increasing W Value");
			userWPosition = Mathf.Clamp(userWPosition + 1, 0, 6);
			signalHyperObjects();
		}
		if(Input.GetKeyDown (KeyCode.DownArrow)) {
			Debug.Log("Decreasing W Value");
			userWPosition = Mathf.Clamp(userWPosition - 1, 0, 6);
			signalHyperObjects();
		}
	}

	public void signalHyperObjects() {
		foreach(GameObject g in GameObject.FindObjectsOfType<GameObject>()) {
			if(g.GetComponent<HyperObject>() != null) {
				g.GetComponent<HyperObject>().updateMaterialShaderValues();
			}
		}
	}
}
