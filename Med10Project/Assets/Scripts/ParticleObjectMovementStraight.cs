using UnityEngine;
using System.Collections;

public class ParticleObjectMovementStraight : MonoBehaviour {

	// Use this for initialization
	void Start () {
		iTween.MoveTo(gameObject, iTween.Hash("position", Vector3.zero, "time", 0.5f, "easetype", iTween.EaseType.easeInBack));
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.zero, "easetype", iTween.EaseType.easeInBack, "time", 0.5f));
		Destroy(gameObject, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
