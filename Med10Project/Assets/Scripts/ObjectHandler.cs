using UnityEngine;
using System.Collections;

public class ObjectHandler : MonoBehaviour 
{

	#region Privates
	private BpmManager bManager;
	private GestureManager gManager;
	private SpawnManager sManager;
	#endregion
	
	void Awake()
	{
		bManager = GameObject.Find("BpmManager").GetComponent<BpmManager>();
		if(bManager == null)
			Debug.LogError("No BpmManager was found in the scene.");

		gManager = Camera.main.GetComponent<GestureManager>();
		if(gManager == null)
			Debug.LogError("No GestureManager was found on the main camera.");
		
		sManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
		if(sManager == null)
			Debug.LogError("No SpawnManager was found in the scene.");
	}
	
	void Start ()
	{
		audio.Play();
		gManager.OnTap += DestroySelf;
	}
	
	#region Class Methods	
	private void DestroySelf(Vector2 screenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0));
		RaycastHit hitInfo;
		if(Physics.Raycast(ray, out hitInfo))
		{
			if(hitInfo.collider == gameObject.collider)
			{
				sManager.AllowSpawning();
				gManager.OnTap -= DestroySelf;
				Destroy(gameObject);
			}
		}
	}
	#endregion
}
