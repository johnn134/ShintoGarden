using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class HyperObject : MonoBehaviour {

	public int wPos = 0;
	public GameObject fourDimController;

	const int TRANSPARENT_QUEUE_ORDER = 3000;

	// Use this for initialization
	void Start () {
		//wPos = Random.Range(0, 7);
		fourDimController = GameObject.Find("Controller4D");
		updateMaterialShaderValues();
	}
	
	// Update is called once per frame
	void Update () {
		//updateMaterialShaderValues();
	}

	public void updateMaterialShaderValues() {
		GetComponent<MeshRenderer>().material.SetFloat("_WPos", (float)wPos);
		GetComponent<MeshRenderer>().material.renderQueue = TRANSPARENT_QUEUE_ORDER + getNewOrder();
	}

	public void updateMaterialShaderValues(int newPos) {
		wPos = newPos;
		updateMaterialShaderValues();
	}

	int getNewOrder() {
		if(fourDimController == null)
			return 0;

		int controllerPos = fourDimController.GetComponent<FourthDimensionController>().userWPosition;

		if(controllerPos == 0) {
			return 7 - wPos;
		}
		else if(controllerPos == 1) {
			if(wPos == 1)
				return 7;
			else if(wPos == 0)
				return 6;
			else
				return 7 - wPos;
		}
		else if(controllerPos == 2) {
			if(wPos == 2)
				return 7;
			else if(wPos == 1)
				return 6;
			else if(wPos == 3)
				return 5;
			else if(wPos == 0)
				return 4;
			else
				return 7 - wPos;
		}
		else if(controllerPos == 3) {
			if(wPos == 3)
				return 7;
			else if(wPos == 2)
				return 6;
			else if(wPos == 4)
				return 5;
			else if(wPos == 1)
				return 4;
			else if(wPos == 5)
				return 3;
			else if(wPos == 0)
				return 2;
			else
				return 1;
		}
		else if(controllerPos == 4) {
			if(wPos == 4)
				return 7;
			else if(wPos == 3)
				return 6;
			else if(wPos == 5)
				return 5;
			else if(wPos == 2)
				return 4;
			else if(wPos == 6)
				return 3;
			else
				return 1 + wPos;
		}
		else if(controllerPos == 5) {
			if(wPos == 5)
				return 7;
			else if(wPos == 4)
				return 6;
			else if(wPos == 6)
				return 5;
			else
				return 1 + wPos;
		}
		else if(controllerPos == 6) {
			return 1 + wPos;
		}

		return 0;
	}
}
