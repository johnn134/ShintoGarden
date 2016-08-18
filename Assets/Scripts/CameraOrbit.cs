using UnityEngine;
using System.Collections;

public class CameraOrbit : MonoBehaviour {

	public GameObject target;
	public float speed = 0.25f;

	// Use this for initialization
	void Start () {
		transform.LookAt(target.transform);
	}
	
	// Update is called once per frame
	void Update () {
		//Orbits the camera around a given point
		transform.Translate(Vector3.left * Time.deltaTime * speed);
		transform.LookAt(target.transform);

		//Scrolls the camera towards or away from the given point
		if(Input.GetAxis("Mouse ScrollWheel") != 0) {
			transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel"));
		}
	}
}
