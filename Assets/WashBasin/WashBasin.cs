using UnityEngine;
using System.Collections;

public class WashBasin : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if(other.name == "WaterZone") {
			other.GetComponent<Ladle>().fillLadle();
		}
	}
}
