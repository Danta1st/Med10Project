using UnityEngine;
using System.Collections;

public class Center : MonoBehaviour {

	private BpmManager bManager;
	private GestureManager gManager;

	void Awake()
	{
		bManager = GameObject.Find("BpmManager").GetComponent<BpmManager>();
		if(bManager == null)
			Debug.LogError("No BpmManager was found in the scene.");
		
		gManager = Camera.main.GetComponent<GestureManager>();
		if(gManager == null)
			Debug.LogError("No GestureManager was found on the main camera.");
	}

	void Start ()
	{
		bManager.OnBeat4th1 += PunchCenter;
		bManager.OnBeat4th3 += PunchCenter;
	}

	private void PunchCenter()
	{
		audio.Play();
		iTween.PunchScale(gameObject, new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
	}
}
