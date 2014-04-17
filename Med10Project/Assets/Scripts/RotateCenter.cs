using UnityEngine;
using System.Collections;

public class RotateCenter : MonoBehaviour {

	public float rotateSpeed = 0.5f;
	private Vector3 rotateAxis = new Vector3(0.5f, 0.3f, 0.2f);

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(transform.position, new Vector3(0.5f, 0.3f, 0.2f), rotateSpeed);
		rotateAxis = new Vector3(rotateAxis.x+0.1f, 0.3f, 0.2f);
	}
}
